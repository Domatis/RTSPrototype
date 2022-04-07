using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildDestructionObjective : Objectives
{
    [SerializeField] private EnemyBuildings building;    

    private void Start() 
    {
        building.buildDesctructionAction += CompleteObjective;
        objectiveType = ObjectiveManager.ObjectiveTypes.buildDestruction;
        ObjectiveRegistration();
    }


    public void CompleteObjective()
    {
        ObjectiveManager.instance.ObjectiveCompleted(objectiveType);
    }

}
