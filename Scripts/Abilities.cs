using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Abilities : ScriptableObject
{
    public Sprite abilityimage;
    [TextArea]
    public string abilityDescription;
    public abstract void AbilityAction();

    protected GameObject currentParentObject;

    public void SetParentObject(GameObject obj)
    {
        currentParentObject = obj;
    }

    public virtual string GetToolTipDescription()
    {
        return abilityDescription;
    }
}



[System.Serializable]
public class AbilityRequirements
{
    public enum AbilityRequirementsTypes {Wood,Gold,Meal,unitCapacity}

    public AbilityRequirementsTypes requirementsType;
    public float minRequirementValue;

    public bool IsRequirementMet()
    {
        bool state = false;

        switch(requirementsType)
        {
            case AbilityRequirementsTypes.Gold:
            //Resource manager değeri kadar gold var mı öğren.
             state = ResourceManager.instance.IsGoldEnough(minRequirementValue);
            break;
            case AbilityRequirementsTypes.Wood:
            //Resource manager değeri kadar wood var mı öğren.
            state = ResourceManager.instance.IsWoodEnough(minRequirementValue);
            break;
            case AbilityRequirementsTypes.Meal:
            //Resource manager değeri kadar meal var mı öğren.
            state = ResourceManager.instance.IsMealEnough(minRequirementValue);
            break;
            case AbilityRequirementsTypes.unitCapacity:
            state = UnitCapacityManager.instance.AreThereEnoughCapacity((int)minRequirementValue);
            break;
        }

        return state;
    }

    public void SpendRequirement()
    {
        switch(requirementsType)
        {
            case AbilityRequirementsTypes.Gold:
            ResourceManager.instance.DecreaseResource(minRequirementValue,ResourceManager.ResourceTypes.gold);
            break;
            case AbilityRequirementsTypes.Wood:
            ResourceManager.instance.DecreaseResource(minRequirementValue,ResourceManager.ResourceTypes.wood);
            break;
            case AbilityRequirementsTypes.Meal:
            ResourceManager.instance.DecreaseResource(minRequirementValue,ResourceManager.ResourceTypes.meal);
            break;
            case AbilityRequirementsTypes.unitCapacity:
            UnitCapacityManager.instance.FillCapacity((int)minRequirementValue);
            break;
        }
    }

    public string GetRequirementToolTipDescription()
    {
        string result = "";

        switch(requirementsType)
        {
            case AbilityRequirementsTypes.Gold:
            if(IsRequirementMet())  result = string.Format("Gold : {0}",minRequirementValue);
            else result = string.Format("Gold : <color=red>{0}</color>",minRequirementValue);
            break;
            case AbilityRequirementsTypes.Wood:
            if(IsRequirementMet())  result = string.Format("Wood: {0}",minRequirementValue);
            else result = string.Format("Wood : <color=red>{0}</color>",minRequirementValue);
            break;
            case AbilityRequirementsTypes.Meal:
            if(IsRequirementMet())  result = string.Format("Meal : {0}",minRequirementValue);
            else result = string.Format("Meal : <color=red>{0}</color>",minRequirementValue);
            break;
            case AbilityRequirementsTypes.unitCapacity:
            if(IsRequirementMet()) result = string.Format("Required Capacity = {0}",minRequirementValue);
            else result = string.Format("Required Capacity : <color=red>{0}</color>",minRequirementValue);
            break;
        }

        return result;
    }
}
