using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionStateManager : MonoBehaviour
{
    public enum ActionStates {move,attack,gatherResources,buildConstruction}  //TODO daha sonra idle eklenebilir duruma göre.

    [HideInInspector]
    public IBaseAction currentbaseaction  = null;


    private void Start() 
    {
        GetComponent<Health>().HealthAtMinimumValue += CancelCurrentAction; 
    }


    public void SetNewAction(IBaseAction action)
    {
        CancelCurrentAction();
        currentbaseaction = action;
    }

    public void CancelCurrentAction()
    {
        currentbaseaction?.CancelAction();
    }

    public IBaseAction GetCurrentAction()
    {
        return currentbaseaction;
    }

}
