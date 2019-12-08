using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    private GameObject observedGameObject;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && observedGameObject != null)
            if (observedGameObject.GetComponent<ShowInteractive>())
                observedGameObject.GetComponent<ShowInteractive>().Use();
    }

    void FixedUpdate()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        if (!Physics.Raycast(transform.position, fwd, out var hit, 3))
        {
            StopInteractive();
            observedGameObject = null;
            return;
        }

        if (observedGameObject == hit.transform.gameObject)
            return;

        StopInteractive();
        observedGameObject = hit.transform.gameObject;

        if (observedGameObject == null || observedGameObject.GetComponent<ShowInteractive>() == null)
            return;
        
        observedGameObject.GetComponent<ShowInteractive>().StartShow();
    }

    private void StopInteractive()
    {
        if (observedGameObject != null)
            if (observedGameObject.GetComponent<ShowInteractive>())
                observedGameObject.GetComponent<ShowInteractive>().StopShow();
    }
}
