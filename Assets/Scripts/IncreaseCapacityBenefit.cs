using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IncreaseCapacity", menuName = "BuildConstructionBenefits/IncreaseCapacity", order = 3)]
public class IncreaseCapacityBenefit : BuildConstructionBenefit
{
    public int capacityValue = 10;

    public override void BenefitAction(Buildings building)
    {
        UnitCapacityManager.instance.IncreaseMaxCapacity(capacityValue);
    }

    public override void UndoBenefitWhenDestroy(Buildings building)
    {
        UnitCapacityManager.instance.DecreaseMaxCapacity(capacityValue);
    }
}
