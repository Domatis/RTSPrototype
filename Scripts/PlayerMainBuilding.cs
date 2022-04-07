using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMainBuilding : MonoBehaviour
{
    private void Start() 
    {
        GetComponent<Buildings>().BuildingDestructionAction += BuildDestroyMethod;
    }

    public void BuildDestroyMethod(Buildings building)
    {
        GameplayManager.instance.GameLose();
    }
}
