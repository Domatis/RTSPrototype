using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DestroyBuildingAbility", menuName = "Abilities/DestroyBuildingAbility", order = 6)]
public class DestroyBuildingAbility : Abilities
{
    public override void AbilityAction()
    {
        List<SelectableObject> selectedObjects = PlayerRTSController.instance.SelectedObjects;
        int initcount = selectedObjects.Count; 

        for(int i=0; i < initcount; i++)
        {
            if(selectedObjects[i] == null) continue; //Null check.
            if(selectedObjects[i].TryGetComponent<Buildings>(out Buildings building))
            {
                building.BuildDestruction();
            }
        }

    }
}
