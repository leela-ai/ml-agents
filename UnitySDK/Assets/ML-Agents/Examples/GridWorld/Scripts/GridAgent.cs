using System;
using UnityEngine;
using MLAgents;
using System.Collections.Generic;
using Blocksworld;

using System.Text;


public class GridAgent : Agent
{
    private float update;


    [Header("Specific to GridWorld")]
    private GridAcademy academy;
    public float timeBetweenDecisionsAtInference;
    private float timeSinceDecision;
    public float currentAction;
    public string sensorObservationsText = "";

    [Tooltip("Because we want an observation right before making a decision, we can force " +
             "a camera to render before making a decision. Place the agentCam here if using " +
             "RenderTexture as observations.")]
    public Camera renderCamera;

    [Tooltip("Selecting will turn on action masking. Note that a model trained with action " +
             "masking turned on may not behave optimally when action masking is turned off.")]
    public bool maskActions = true;

    public override void InitializeAgent()
    {
        academy = FindObjectOfType(typeof(GridAcademy)) as GridAcademy;
        //blocksworldSMS = new BlocksWorldSensoriMotorSystem(academy.gridSize);



    }

    public override void CollectObservations()
    {

    }

    //BlocksWorldSensoriMotorSystem blocksworldSMS;

    // to be implemented by the developer
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //AddReward(-0.01f);

        //Debug.Log("vectorAction[len " + vectorAction.Length+"][0]: "+ vectorAction[0]);
        Vector3 targetPos = transform.position;
        List<String> actions = new List<String>();
        actions.Add(textAction);

        // returns a string encoding sensors and values e.g., "hp11=0;hp21=1;hp31=0;..."
        //sensorObservationsText = blocksworldSMS.stepPhysicalWorld("0", actions);
        //Debug.Log(textAction);

        /*
         * Here we need to poke into blocksworld to get locations of the hand and the blocks
         * and set their corresponging GameObject transforms to move them to their new locations
         */

        // -->>> grab block and hand locs and set tranforms <<<--
        copyBlocksPositionsToUnity(textAction);
    }

    [System.Serializable]
        public struct ObjInfo
    {
        public string name;
        public int x;
        public int y;

	public ObjInfo(string _name, int _x , int _y)
	{
	    name = _name;
	    x = _x;
	    y = _y;
	}
    }

    // convert {"name": "b2", "x": 5, "y": 5} to  a Vec2(5,5);
    public Vec2 makePositionVector(ObjInfo obj)
    {
        Vec2 v = new Vec2();
        v.x =  obj.x;
        v.y = obj.y;
        return v;
    }

    [System.Serializable]
    public class ObjLocs
    {

        public ObjInfo[] objs;

        public static ObjLocs CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ObjLocs>(jsonString);
        }

        public ObjInfo getByName(string name)
        {
            foreach (ObjInfo obj in objs)
            {
                if (obj.name == name)
                {
                    return obj;
                }
            }
	    return new ObjInfo(name, -100, -100);
        }

        // Given JSON input:
        // {"name":"Dr Charles","lives":3,"health":0.8}
        // this example will return a PlayerInfo object with
        // name == "Dr Charles", lives == 3, and health == 0.8f.
    }
    /*
      Incoming textAction contains object locations in json format list of objs:

     [{"name": "h", "x": 4, "y": 3, "color": "rgb(0,0,255)", "shape": null, "texture": "02", "rotation": 0, "grasping": true, "graspedObject": null}, {"name": "j", "x": -100, "y": -100, "color": "rgb(255,165,0)", "shape": null, "texture": "03", "rotation": 0, "grasping": false, "graspedObject": null}, {"name": "v", "x": 0, "y": 4, "color": "rgb(255,255,255)", "shape": "circle", "texture": "00"}, {"name": "b1", "x": 1, "y": 5, "color": "rgb(255,20,147)", "shape": "circle", "texture": "00"},
     {"name": "b2", "x": 5, "y": 5, "color": "rgb(255,255,0)", "shape": "square", "texture": "01"}]

     */



    public void copyBlocksPositionsToUnity(string textAction)
    {
        if (textAction == "")
        {
            return;
        }

        ObjLocs objlocs = ObjLocs.CreateFromJSON(textAction);

        Vec2 handpos = makePositionVector(objlocs.getByName("h"));

        // Move the robot agent itself
        Vector3 currentPos = transform.position;
        float dx = handpos.x - currentPos.x ;
        // Remeber! Y is Z, Z is Y......god damn it.
        float dy = handpos.y - currentPos.z ;
        //Debug.Log("X: New, Curr: " + handpos.x.ToString() + "," + currentPos.x);
        //Debug.Log("Y: New, Curr: " + handpos.y.ToString() + "," + currentPos.y);
        Debug.Log(dx.ToString() + ":" + dy.ToString());
        int _localRot = 0;
        if (dx > 0 & dy == 0.0) {
            _localRot = 90;
        }
        if (dx < 0 & dy == 0.0)
        {
            _localRot = -90;
            //Debug.Log("Turn Left");
        }
        if (dy > 0 & dx == 0.0)
        {
            _localRot = 0;
           // Debug.Log("Turn Top");
        }
        if (dy < 0 & dx == 0.0)
        {
            _localRot = 180;
            //Debug.Log("Turn Bottom");
        }
        //do local rotation. Still need to animate.		
        transform.rotation = Quaternion.Euler(0, _localRot, 0);
        Debug.Log("Rot = " + _localRot.ToString());

        // NOTE: In this gridworld, y is z, and z is y.
        transform.position = new Vector3(handpos.x, 0, handpos.y);


       


        /*(loop for i from 1 to 10 do
                (insert (format "
            Vec2 block%dpos = makePositionVector(objlocs.getByName(\"b%d\"));
            GameObject block%d = academy.actorObjs[%d];
            block%d.transform.position = new Vector3(block%dpos.x + dx, 0, block%dpos.y + dy);
            block%d.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            \n\n"  i i i (- i 1) i i i i)))*/


        Vec2 eyepos = makePositionVector(objlocs.getByName("v"));
        // set the agent cam to follow the agent to get the agent's point of view in the view port
        academy.agentCam.transform.position = academy.trueAgent.transform.position;
        // used to be set to the eye position
        //new Vector3(eyepos.x, 6f, eyepos.y);
        // Console.WriteLine($"eyepos set to  '{eyepos.x} {eyepos.y}'");

        Vec2 targetpos = makePositionVector(objlocs.getByName("toplevel_goal"));
        if (

                academy.targetObj != null)
        {
            academy.targetObj.transform.position = new Vector3(targetpos.x, 1f, targetpos.y);
            //academy.targetObj.transform.position = new Vector3(5f, 1f, 5f);
        }


        try
        {


        List<String> blocknames = new List<String>() {
            "b1", "b2", "b3", "b4", "b5", "b6", "b7" ,"b8", "b9", "b10"
        };

        for (int i = 0; i < blocknames.Count; i++) {
            string blockname = blocknames[i];
            Vec2 blockpos = makePositionVector(objlocs.getByName(blockname));
            GameObject block = academy.actorObjs[i];
            block.transform.position = new Vector3(blockpos.x, 0.25f, blockpos.y);
            //block.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        }



	} catch (Exception e) {
            Console.WriteLine($"copyBlocksPositionsToUnity caught block pos error: '{e}'");
	}


        textAction = "";
    }

    // to be implemented by the developer
    public override void AgentReset()
    {
//        academy.AcademyReset();
    }

    public void FixedUpdate()
    {
        WaitTimeInference();
    }

    void Update()
    {
        update += Time.deltaTime;
        if (update > 1.0f)
        {
            update = 0.0f;
            Debug.Log("Update one sec");
        }
    }
        private void WaitTimeInference()
    {
        if (renderCamera != null)
        {
            renderCamera.Render();
        }

        if (!academy.GetIsInference())
        {
            RequestDecision();
        }
        else
        {
            if (timeSinceDecision >= timeBetweenDecisionsAtInference)
            {
                timeSinceDecision = 0f;
                RequestDecision();
            }
            else
            {
                timeSinceDecision += Time.fixedDeltaTime;
            }
        }
    }
}
