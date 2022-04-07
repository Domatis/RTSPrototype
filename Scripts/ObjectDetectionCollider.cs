using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ObjectDetectionCollider : MonoBehaviour
{
    public Action<GameObject> ObjectDetectedAction;
    public Action<GameObject> ObjectExitAction;


    private bool  detectionOn = true;

    public bool DetectionOn { get {return detectionOn;} set {detectionOn = value;} }

    private void OnTriggerEnter(Collider other) 
    {
        if(!detectionOn) return;
        //This layer only collides with playerobjects.
        detectionOn = false;    //Once player detected once detection will be off, it's need to be set on when needed.
        ObjectDetectedAction?.Invoke(other.gameObject);
    }

    private void OnTriggerExit(Collider other) 
    {
       // if(!detectionOn) return;
        ObjectExitAction?.Invoke(other.gameObject);
    }
}
