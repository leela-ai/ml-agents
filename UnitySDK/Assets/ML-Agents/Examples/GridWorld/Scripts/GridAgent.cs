using System;
using UnityEngine;
using System.Linq;
using MLAgents;
using System.Collections.Generic;


public class GridAgent : Agent
{
    [Header("Specific to GridWorld")]
    private GridAcademy academy;
    public float timeBetweenDecisionsAtInference;
    private float timeSinceDecision;
    public float currentAction;

    [Tooltip("Because we want an observation right before making a decision, we can force " +
             "a camera to render before making a decision. Place the agentCam here if using " +
             "RenderTexture as observations.")]
    public Camera renderCamera;

    [Tooltip("Selecting will turn on action masking. Note that a model trained with action " +
             "masking turned on may not behave optimally when action masking is turned off.")]
    public bool maskActions = true;

    private const int NoAction = 0;  // do nothing!
    private const int Up = 1;
    private const int Down = 2;
    private const int Left = 3;
    private const int Right = 4;
    private const int Grasp = 5;
    private const int Ungrasp = 6;

    public override void InitializeAgent()
    {
        academy = FindObjectOfType(typeof(GridAcademy)) as GridAcademy;
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


        AddVectorObs(currentAction);

        // Mask the necessary actions if selected by the user.
        if (maskActions)
        {
            SetMask();
        }
    }


    private void graspObject()
    {
        Collider[] blockTest = Physics.OverlapBox(transform.position, new Vector3(1.3f, 1.3f, 1.3f));
        foreach (Collider col in blockTest) { 
            if (col.gameObject.CompareTag("pit"))
            {
                Debug.Log("graspObject found overlapping pit object");
                addGraspJoint(col.attachedRigidbody);
            }
        }
    }

   

    //GameObject[] FindGameObjectsWithTag(string tag)

    private void addGraspJoint(Rigidbody r) { 
        var joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = r;
            
    }
    /// <summary>
    /// Applies the mask for the agents action to disallow unnecessary actions.
    /// </summary>
    private void SetMask()
    {
        // Prevents the agent from picking an action that would make it collide with a wall
        var positionX = (int)transform.position.x;
        var positionZ = (int)transform.position.z;
        var maxPosition = academy.gridSize - 1;

        if (positionX == 0)
        {
            SetActionMask(Left);
        }

        if (positionX == maxPosition)
        {
            SetActionMask(Right);
        }

        if (positionZ == 0)
        {
            SetActionMask(Down);
        }

        if (positionZ == maxPosition)
        {
            SetActionMask(Up);
        }
    }


    public void destroySpring()
    {
        SpringJoint spring = gameObject.GetComponent<SpringJoint>();
        if (spring != null)
        {
            Destroy(spring);
        } else
        {
            Debug.Log("spring is null");
        }
    }

    public void createSpring()
    {
        SpringJoint sjoint = gameObject.AddComponent<SpringJoint>();
        GameObject springyThing = academy.actorObjs[(int)academy.resetParameters["numGoals"] + (int)academy.resetParameters["numObstacles"]];
        sjoint.connectedBody = springyThing.GetComponent<Rigidbody>();

        sjoint.damper = 100;
        sjoint.spring = 10000;
    }

    // to be implemented by the developer
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-0.01f);
        currentAction = vectorAction[0];
        int action = Mathf.FloorToInt(currentAction);

        Vector3 targetPos = transform.position;

        


        switch (action)
        {
            case NoAction:
                // do nothing
                break;
            case Right:
                destroySpring();
                targetPos = transform.position + new Vector3(1f, 0, 0f);
                createSpring();
                Debug.Log(string.Format("action = {0}", action));

                break;
            case Left:
                destroySpring();
                targetPos = transform.position + new Vector3(-1f, 0, 0f);
                createSpring();
                Debug.Log(string.Format("action = {0}", action));

                break;
            case Up:
                destroySpring();
                targetPos = transform.position + new Vector3(0f, 0, 1f);
                createSpring();
                Debug.Log(string.Format("action = {0}", action));

                break;
            case Down:
                destroySpring();
                targetPos = transform.position + new Vector3(0f, 0, -1f);
                createSpring();
                Debug.Log(string.Format("action = {0}", action));

                break;
            case Grasp:
                Debug.Log(string.Format("action = {0} Grasp", action));
                graspObject();
                break;
            case Ungrasp:
                Debug.Log(string.Format("action = {0}  Ungrasp", action));
                FixedJoint fJ = gameObject.GetComponent<FixedJoint>();
                Destroy(fJ);
                break;
            default:
                throw new ArgumentException("Invalid action value");
        }

       
        

        Collider[] blockTest = Physics.OverlapBox(targetPos, new Vector3(0.3f, 0.3f, 0.3f));
        if (blockTest.Where(col => col.gameObject.CompareTag("wall")).ToArray().Length == 0)
        {
            transform.position = targetPos;

            if (blockTest.Where(col => col.gameObject.CompareTag("goal")).ToArray().Length == 1)
            {
                Done();
                SetReward(1f);
            }
            if (blockTest.Where(col => col.gameObject.CompareTag("pit")).ToArray().Length == 1)
            {
                Done();
                SetReward(-1f);
            }
        }
        
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
