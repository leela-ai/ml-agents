﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MLAgents;


public class GridAcademy : Academy
{
    [HideInInspector]
    public List<GameObject> actorObjs;
    [HideInInspector]
    public int[] players;

    public GameObject trueAgent;


    public int gridSize;

    public GameObject camObject;
    Camera cam;
    public Camera agentCam;

    public GameObject agentPref;
    public GameObject block1;
    public GameObject block2;
    public GameObject targetPref;
    public GameObject targetObj;

    public GameObject syntheticItemPref;


    GameObject[] objects;

    GameObject plane;
    GameObject sN;
    GameObject sS;
    GameObject sE;
    GameObject sW;


    public override void InitializeAcademy()
    {
        gridSize = (int)resetParameters["gridSize"];
        cam = camObject.GetComponent<Camera>();

        objects = new GameObject[] { agentPref, block1, block2};

        agentCam = GameObject.Find("agentCam").GetComponent<Camera>();

        actorObjs = new List<GameObject>();

        //target = GameObject.Find("Target");
        targetObj = Instantiate(targetPref);
        targetObj.transform.position = new Vector3(5f, 0.2f, 5f);
        targetObj.transform.localScale += new Vector3(8f,8f,8f);

 


        plane = GameObject.Find("Plane");
        /*
        sN = GameObject.Find("sN");
        sS = GameObject.Find("sS");
        sW = GameObject.Find("sW");
        sE = GameObject.Find("sE");
        */
    }

    public void SetEnvironment()
    {
        /*    cam.transform.position = new Vector3(-((int)resetParameters["gridSize"] - 1) / 2f,
                                                 (int)resetParameters["gridSize"] * 1.25f,
                                                 -((int)resetParameters["gridSize"] - 1) / 2f);
                                                 */
        // This is the main camera. Set it about mid-way in the house
        //float thisPos = ((((int)resetParameters["gridSize"] - 1) / 2f) + 2f);
        //cam.transform.position = new Vector3(thisPos, thisPos, thisPos);
        // set camera rotation so it's looking into the house from the back
        //cam.transform.rotation = Quaternion.Euler(45, 180, 0);

        //cam.orthographicSize = ((int)resetParameters["gridSize"] + 5f) / 2f;

        List<int> playersList = new List<int>();

        for (int i = 0; i < (int)resetParameters["numGoals"]; i++)
        {
            playersList.Add(1);
        }

        for (int i = 0; i < (int)resetParameters["numObstacles"]; i++)
        {
            playersList.Add(2);
        }


        players = playersList.ToArray();

        plane.transform.localScale = new Vector3((gridSize+2) / 10.0f, 1f, (gridSize+2) / 10.0f);
        plane.transform.position = new Vector3((gridSize +2) / 2f, -0.5f, (gridSize+2) / 2f);
        /*
        sN.transform.localScale = new Vector3(1, 1, gridSize + 3);
        sS.transform.localScale = new Vector3(1, 1, gridSize + 3);
        sN.transform.position = new Vector3((gridSize ) / 2f, 0.0f, gridSize+2);
        sS.transform.position = new Vector3((gridSize ) / 2f, 0.0f, -1);
        sE.transform.localScale = new Vector3(1, 1, gridSize + 3);
        sW.transform.localScale = new Vector3(1, 1, gridSize + 3);
        sE.transform.position = new Vector3(gridSize+2, 0.0f, (gridSize ) / 2f);
        sW.transform.position = new Vector3(-1, 0.0f, (gridSize ) / 2f);
        */
        agentCam.orthographicSize = 2f;
        agentCam.transform.position = new Vector3((gridSize - 1) / 2f, gridSize + 1f, (gridSize - 1) / 2f);

    }

    public override void AcademyReset()
    {
        foreach (GameObject actor in actorObjs)
        {
            DestroyImmediate(actor);
        }

        SetEnvironment();

        actorObjs.Clear();

        HashSet<int> numbers = new HashSet<int>();
        while (numbers.Count < players.Length + 1)
        {
            numbers.Add(Random.Range(0, gridSize * gridSize));
        }
        int[] numbersA = Enumerable.ToArray(numbers);


        for (int i = 0; i < players.Length; i++)
        {
            int x = (numbersA[i]) / gridSize;
            int y = (numbersA[i]) % gridSize;
            GameObject actorObj = Instantiate(objects[players[i]]);
            actorObj.transform.position = new Vector3(x, 0f, y);
            actorObjs.Add(actorObj);
        }


        int x_a = (numbersA[players.Length]) / gridSize;
        int y_a = (numbersA[players.Length]) % gridSize;
        trueAgent.transform.position = new Vector3(x_a, 0f, y_a);
    }

    public override void AcademyStep()
    {

    }
}
