using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildConstruction", menuName = "Abilities/BuildConstructionAbility", order = 1)]
public class BuildConstructionAbility : Abilities
{
    public GameObject buildPlaceObject;
    public AbilityRequirements[] requirements;

    // Build placement için action.
    public override void AbilityAction()
    {
        // Öncelikle gereksinimler kontrol ediliyor herhangi biri uymuyorsa eylem gerçekleşmiyor.
        for(int i = 0; i < requirements.Length; i++)
        {
            if(!requirements[i].IsRequirementMet())
            {
                Debug.Log("Build Yapımı için yeterli kaynak yok");
                return;
            }
        }

        //If requirements are provided.We build the build placing object.
        GameObject placingobject = Instantiate(buildPlaceObject,Vector3.zero,Quaternion.identity);  //Position is not important at first.
        BuildPlaceObject buildplaceRef = placingobject.GetComponent<BuildPlaceObject>();
        //When build is placed on game requirements must spend.
        //Set requirements for place object.
        buildplaceRef.Requirements = requirements;
        buildplaceRef.buildPrefabPlaced += WhenBuildPlaced; //TODO we can do this system with interfaces if there are performance problems.
        //We send the information playercontroller.
        PlayerRTSController.instance.StartBuildPlacement(buildplaceRef);
    }

    public void WhenBuildPlaced(GameObject build)
    {
        //Find all selected builders to build construction on build.
        for(int i =0; i< PlayerRTSController.instance.SelectedPlayerUnits.Count;i++)
        {      
            build.GetComponent<Buildings>().StartUnitsInteractions(PlayerRTSController.instance.SelectedPlayerUnits[i]); 
        }
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




