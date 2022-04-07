using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCapacityManager : MonoBehaviour
{
    public static UnitCapacityManager instance;
    
    [SerializeField] private int startingunitCapacity = 10;
    [SerializeField] private int maxCapacity = 100;

    private int currentMaxCapacity;
    private int filledCapacity;

    private void Awake() 
    {
        instance = this;
    }

    private void Start() 
    {
        filledCapacity = 0;
        currentMaxCapacity = startingunitCapacity;   
        //Update UI.
        GameplayUIManager.instance.UpdateUnitCapacity(filledCapacity,currentMaxCapacity); 
    }

    public void FillCapacity(int value)
    {
        filledCapacity += value;
        //Update UI.
        GameplayUIManager.instance.UpdateUnitCapacity(filledCapacity,currentMaxCapacity);
    }

    public void UndoCapacity(int value)
    {
        filledCapacity -= value;
        filledCapacity = Mathf.Max(0,filledCapacity);
        //Update UI.
        GameplayUIManager.instance.UpdateUnitCapacity(filledCapacity,currentMaxCapacity);
    }

    public void IncreaseMaxCapacity(int value)
    {
        currentMaxCapacity += value;
        currentMaxCapacity = Mathf.Min(currentMaxCapacity,maxCapacity);
        //Update UI.
        GameplayUIManager.instance.UpdateUnitCapacity(filledCapacity,currentMaxCapacity);
    }

    public void DecreaseMaxCapacity(int val)
    {
        currentMaxCapacity -= val;
        currentMaxCapacity = Mathf.Max(0,currentMaxCapacity);
        //Update UI.
        GameplayUIManager.instance.UpdateUnitCapacity(filledCapacity,currentMaxCapacity);

    }

    public bool AreThereEnoughCapacity(int requiredCapacity)
    {
        if((currentMaxCapacity-filledCapacity) >= requiredCapacity) return true;
        //Else
        return false;
    }
}
