using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreateUnitOrdersAbility", menuName = "Abilities/CreateUnitAbility", order = 1)]
public class CreateUnitOrdersAbility : Abilities,IOrders
{
    public GameObject unitPrefab;
    public float creatingTime = 10;
    public AbilityRequirements[] requirements;


    public override void AbilityAction()
    {     
        // Check requirements first.
        for(int i = 0; i < requirements.Length; i++)
        {
            if(!requirements[i].IsRequirementMet())
            {
                return; //Return if no resource for requirements
            }
        }

        //Spend requirements.
        for(int i = 0 ; i < requirements.Length; i++)
        {
            requirements[i].SpendRequirement();
        }

        //Creating unit will occur via orders. Not instantly.
        currentParentObject.GetComponent<CreatingOrdersManager>().StartCreatingOrders(this);
        //Requirement spend.
        
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



    public void CompleteOrder(GameObject parentOrderObject)
    {
        //At this point creating orders happens at buildings only.
        UnitProducingBuildings parentBuilding =  parentOrderObject.GetComponent<UnitProducingBuildings>(); 
         //Instatiate the unit prefabs. 
         GameObject unit = Instantiate(unitPrefab,parentBuilding.unitSpawnPoint.transform.position,Quaternion.identity);
         //After creating the unit send information to mover to target destination.
         unit.GetComponent<Mover>().StartMovementAction(parentBuilding.unitDestinationPoint.transform.position);    
    }

    public Sprite GetOrderSprite()
    {
        return abilityimage;
    }

    public float GetOrderTime()
    {
        return creatingTime;
    }

}
