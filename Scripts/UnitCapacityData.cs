using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCapacityData : MonoBehaviour
{
    
    [SerializeField] private int capacityValue;


    private void Start() 
    {
        if(TryGetComponent<RTSUnit>(out RTSUnit unit))
        {
            unit.UnitDeathAction += () =>
            {
                UnitCapacityManager.instance.UndoCapacity(capacityValue);
            };
        }

    }
}
