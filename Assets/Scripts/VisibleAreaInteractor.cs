using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleAreaInteractor : MonoBehaviour
{
    
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private SelectableObject selectableObj;


    private bool objectVisible = false;
    private bool stayingAtVisibleArea;
    private bool lastStayingState;


    private void Start() 
    {
        stayingAtVisibleArea = false;
        lastStayingState = false;

        //At start update the current situation.
        if(!objectVisible)
        {
            MakeObjectInvisible();
        }
    }

    private void FixedUpdate() 
    {
        //Before the stay trigger working, set it to false default. If object does not stay at the visible area trigger will not change it. 
        stayingAtVisibleArea = false;
    }

    private void Update() 
    {
        //If there's no change just return and do nothing.
        if(lastStayingState == stayingAtVisibleArea) return;    

        if(stayingAtVisibleArea)
        {
            MakeObjectVisible();
        }
        else 
        {
            MakeObjectInvisible();
        }
        lastStayingState = stayingAtVisibleArea;
    }

    public void MakeObjectVisible()
    {
        objectVisible = true;

       for(int i =0; i < renderers.Length;i++)
       {
           renderers[i].enabled = true;
       }
       selectableObj.ObjectSelectable = true;
    }

    public void MakeObjectInvisible()
    {  
        objectVisible = false;

        for(int i =0; i < renderers.Length;i++)
        {
           renderers[i].enabled = false;
        }
        selectableObj.ObjectSelectable = false;
        if(selectableObj.IsObjectSelected) selectableObj.ObjectDeselected();
    }

    private void OnTriggerStay(Collider other) 
    {
        stayingAtVisibleArea = true;
    }


}
