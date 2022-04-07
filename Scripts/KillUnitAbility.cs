using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KillUnitAbility", menuName = "Abilities/KillUnitAbility", order = 5)]
public class KillUnitAbility : Abilities
{
    
    public override void AbilityAction()
    {
        
        int selectedCount = PlayerRTSController.instance.SelectedPlayerUnits.Count;
        for(int i= 0; i < selectedCount;i++)
        {
            PlayerRTSController.instance.SelectedPlayerUnits[0].DestroyUnit();          
        }
    }

}
