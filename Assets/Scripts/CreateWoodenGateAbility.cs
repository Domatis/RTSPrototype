using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreateWoodenGateAbility", menuName = "Abilities/CreateWoodenGateAbility", order = 5)]
public class CreateWoodenGateAbility : Abilities
{
    
    public GameObject woodenGatePrefab;
    public AbilityRequirements[] requirements;


    public override void AbilityAction()
    {
        //Check requirements first.
        for(int i = 0; i < requirements.Length; i++)
        {
            if(!requirements[i].IsRequirementMet())
            {
                Debug.Log("Build Yapımı için yeterli kaynak yok");
                return;
            }
        }


        //It will destroy the current main wooden wall and place the gate prefab.

        Instantiate(woodenGatePrefab,currentParentObject.transform.position,currentParentObject.transform.rotation);
        Destroy(currentParentObject);
    }


    public override string GetToolTipDescription()
    {
        string result = "";
        result += string.Format("{0}\n",base.GetToolTipDescription());
        
        for(int i = 0 ; i < requirements.Length; i++)
        {
            result += requirements[i].GetRequirementToolTipDescription() +" ";
        }
        return result;
    }
}
