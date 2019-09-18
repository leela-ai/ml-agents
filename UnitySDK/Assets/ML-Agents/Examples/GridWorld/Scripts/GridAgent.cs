using System;
using UnityEngine;
using System.Linq;
using MLAgents;
using System.Collections.Generic;
using Blocksworld;


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
        blocksworldSMS = new BlocksWorldSensoriMotorSystem();
    }

    public override void CollectObservations()
    {
        // Numeric observations are the positions of the agent, goal and pit

        AddVectorObs(gameObject.transform.position.x);
        AddVectorObs(gameObject.transform.position.z);
        // goal
        AddVectorObs(academy.actorObjs[0].transform.position.x);
        AddVectorObs(academy.actorObjs[0].transform.position.z);
        // pit
        AddVectorObs(academy.actorObjs[1].transform.position.x);
        AddVectorObs(academy.actorObjs[1].transform.position.z);

        /* we could add eye position vector here if we want as a vector obs */

        AddVectorObs(currentAction);

        SetTextObs(sensorObservationsText);

     
    }
   
    BlocksWorldSensoriMotorSystem blocksworldSMS;

    // to be implemented by the developer
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-0.01f);


        Vector3 targetPos = transform.position;
        List<String> actions = new List<String>();
        actions.Add(textAction);

        // returns a string encoding sensors and values e.g., "hp11=0;hp21=1;hp31=0;..."
        sensorObservationsText = blocksworldSMS.stepPhysicalWorld("0", actions);
        Debug.Log(sensorObservationsText);

        /*
         * Here we need to poke into blocksworld to get locations of the hand and the blocks
         * and set their corresponging GameObject transforms to move them to their new locations
         */

        // -->>> grab block and hand locs and set tranforms <<<--
        copyBlocksPositionsToUnity();



    }

    public void copyBlocksPositionsToUnity()
    {
        Vec2 handpos = blocksworldSMS.hand1.getPosition();
        Vec2 eyepos = blocksworldSMS.eye.getPosition();
        Vec2 block1pos = blocksworldSMS.block1.getPosition();
        Vec2 block2pos = blocksworldSMS.block2.getPosition();
        Vec2 block3pos = blocksworldSMS.block3.getPosition();
        Vec2 block4pos = blocksworldSMS.block4.getPosition();

        float dx = -1.0f;
        float dy = -1.0f;
        transform.position = new Vector3(handpos.x + dx, 0, handpos.y + dy);

        // block1
        academy.actorObjs[0].transform.position = new Vector3(block1pos.x + dx, 0, block1pos.y + dy);

        // block2
        academy.actorObjs[1].transform.position = new Vector3(block2pos.x + dx, 0, block2pos.y + dy);

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
