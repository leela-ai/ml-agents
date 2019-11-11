using System;
using UnityEngine;
using MLAgents;
using System.Collections.Generic;
using Blocksworld;

using System.Text;


public class GridAgent : Agent
{
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
        AddReward(-0.01f);

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
        Vec2 eyepos = makePositionVector(objlocs.getByName("v"));



        // Move the robot agent itself
        float dx = 0.0f;
        float dy = 0.0f;
        transform.position = new Vector3(handpos.x + dx, 0, handpos.y + dy);

        /*(loop for i from 1 to 10 do
                (insert (format "
            Vec2 block%dpos = makePositionVector(objlocs.getByName(\"b%d\"));
            GameObject block%d = academy.actorObjs[%d];
            block%d.transform.position = new Vector3(block%dpos.x + dx, 0, block%dpos.y + dy);
            block%d.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            \n\n"  i i i (- i 1) i i i i)))*/
        try {

	    Vec2 block1pos = makePositionVector(objlocs.getByName("b1"));
	    GameObject block1 = academy.actorObjs[0];
	    block1.transform.position = new Vector3(block1pos.x + dx, 0, block1pos.y + dy);
	    block1.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        


	    Vec2 block2pos = makePositionVector(objlocs.getByName("b2"));
	    GameObject block2 = academy.actorObjs[1];
	    block2.transform.position = new Vector3(block2pos.x + dx, 0, block2pos.y + dy);
	    block2.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        


	    Vec2 block3pos = makePositionVector(objlocs.getByName("b3"));
	    GameObject block3 = academy.actorObjs[2];
	    block3.transform.position = new Vector3(block3pos.x + dx, 0, block3pos.y + dy);
	    block3.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        


	    Vec2 block4pos = makePositionVector(objlocs.getByName("b4"));
	    GameObject block4 = academy.actorObjs[3];
	    block4.transform.position = new Vector3(block4pos.x + dx, 0, block4pos.y + dy);
	    block4.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        


	    Vec2 block5pos = makePositionVector(objlocs.getByName("b5"));
	    GameObject block5 = academy.actorObjs[4];
	    block5.transform.position = new Vector3(block5pos.x + dx, 0, block5pos.y + dy);
	    block5.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        


	    Vec2 block6pos = makePositionVector(objlocs.getByName("b6"));
	    GameObject block6 = academy.actorObjs[5];
	    block6.transform.position = new Vector3(block6pos.x + dx, 0, block6pos.y + dy);
	    block6.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        


	    Vec2 block7pos = makePositionVector(objlocs.getByName("b7"));
	    GameObject block7 = academy.actorObjs[6];
	    block7.transform.position = new Vector3(block7pos.x + dx, 0, block7pos.y + dy);
	    block7.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        


	    Vec2 block8pos = makePositionVector(objlocs.getByName("b8"));
	    GameObject block8 = academy.actorObjs[7];
	    block8.transform.position = new Vector3(block8pos.x + dx, 0, block8pos.y + dy);
	    block8.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        


	    Vec2 block9pos = makePositionVector(objlocs.getByName("b9"));
	    GameObject block9 = academy.actorObjs[8];
	    block9.transform.position = new Vector3(block9pos.x + dx, 0, block9pos.y + dy);
	    block9.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        


	    Vec2 block10pos = makePositionVector(objlocs.getByName("b10"));
	    GameObject block10 = academy.actorObjs[9];
	    block10.transform.position = new Vector3(block10pos.x + dx, 0, block10pos.y + dy);
	    block10.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        
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
