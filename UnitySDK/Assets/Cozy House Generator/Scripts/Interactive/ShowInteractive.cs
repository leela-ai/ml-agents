using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInteractive : MonoBehaviour
{
    public GameObject[] objectsToEnable;
    public OpenMe       openMe;
    
    public void StartShow()
    {
        if (objectsToEnable == null || objectsToEnable.Length == 0)
            return;
        
        foreach (var obj in objectsToEnable)
            obj.SetActive(true);
    }

    public void Use()
    {
        if (openMe)
            openMe.Use();
    }
    
    public void StopShow()
    {
        if (objectsToEnable == null || objectsToEnable.Length == 0)
            return;
        
        foreach (var obj in objectsToEnable)
            obj.SetActive(false);
    }
}
