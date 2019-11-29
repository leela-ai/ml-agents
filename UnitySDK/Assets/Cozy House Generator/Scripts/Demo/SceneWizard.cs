using System;
using System.Collections;
using System.Collections.Generic;
using Cozy_House_Generator.Scripts;
using UnityEngine;

public class SceneWizard : MonoBehaviour
{
    public GameObject[] mainModeGameObjects;
    public GameObject[] characterModeGameObjects;
    private bool isMainCurrentMode;
    public HouseGenerator houseGenerator;

    private void Start()
    {
        EnableMainMode();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            SwitchSceneMode();

        if (!isMainCurrentMode && Input.GetKeyDown(KeyCode.R))
            houseGenerator.GenerateWithRandomSeed();
    }


    public void EnableCharacterMode()
    {
        if (characterModeGameObjects.Length == 0)
        {
            Debug.LogWarning("Scene Wizard: Character objects are empty");
            return;
        }
        
        isMainCurrentMode = false;
        foreach (var o in mainModeGameObjects)
            o.SetActive(false);

        foreach (var o in characterModeGameObjects)
            o.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void EnableMainMode()
    {
        if (mainModeGameObjects.Length == 0)
        {
            Debug.LogWarning("Scene Wizard: Main Mode objects are empty");
            return;
        }
        
        isMainCurrentMode = true;
        foreach (var o in mainModeGameObjects)
            o.SetActive(true);

        foreach (var o in characterModeGameObjects)
            o.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SwitchSceneMode()
    {
        if (isMainCurrentMode)
            EnableCharacterMode();
        else 
            EnableMainMode();
    }
    
}
