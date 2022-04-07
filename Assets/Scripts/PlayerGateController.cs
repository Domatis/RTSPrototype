using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class PlayerGateController : MonoBehaviour
{
    
    [SerializeField] private BoxCollider mainCollider;
    [SerializeField] private Animator anim;

    private bool unitDetected = false;
    private bool lastState = false;

    public void StartOpenGate()
    {
        anim.SetBool("Open",true);

    }

    public void StartCloseGate()
    {
        anim.SetBool("Open",false);
    }

    private void Update() 
    {
        if(lastState == unitDetected) return;
        
        if(unitDetected)
            StartOpenGate();

        else    
            StartCloseGate();
        
        lastState = unitDetected;
    }

    private void FixedUpdate() 
    {
        unitDetected = false;
    }

    //Calling from animation.
    public void OpenGate()
    {
        mainCollider.enabled = false;
    }

    //Calling from animation.
    public void CloseGate()
    {
        mainCollider.enabled = true;
    }


    private void OnTriggerStay(Collider other) 
    {
        if(other.gameObject.GetComponent<RTSUnit>())
            unitDetected = true;
    }

}
