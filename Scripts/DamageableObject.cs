using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class DamageableObject : MonoBehaviour,IUnitInteractableObject
{

    public Action<GameObject> DamageTaken;  //This gameobject is object that took damage from it.
    [SerializeField] private Transform damageTakenPoint;
    [Header("Player Interaction Settings")]
    [SerializeField] private bool disablePlayerInteraction = false;
    [Range(1,100)][Tooltip("Lower Values Means High Priority")]
    [SerializeField] private int interactionPriority;
    private Health healthref;
    private List<IAttackers> currentAttackers = new List<IAttackers>();

    public List<IUnitInteractableObject> testlist;

    private bool damageTakenAvailable = true;

    public Transform DamageTakenPoint{get{return damageTakenPoint;}}
    public bool DamageTakenAvailable {get {return damageTakenAvailable;}}

    private void Awake() 
    {
        healthref = GetComponent<Health>();
    }

    private void Start() 
    {
        if(!disablePlayerInteraction)
        {
            GetComponentInChildren<ObjectPlayerInteraction>().AddInteraction(this);
        }

        healthref.HealthAtMinimumValue += ObjectWillDestroy;
    }

    public bool AllowPlayerUnitInteraction()
    {
        return !disablePlayerInteraction;
    }

    public void TakeDamage(float val,GameObject attacker) //Change this part to IAttacker
    {
        if(!damageTakenAvailable) return;
        if(attacker!=null) DamageTaken?.Invoke(attacker.gameObject);
        healthref.DecreaseHealth(val);
    }

    public void ObjectWillDestroy()
    {
        //Warn the every attackers.
        damageTakenAvailable = false;   //Health already at zero.
        int size = currentAttackers.Count;
        for(int i = 0; i < size; i++)
        {
            if(currentAttackers[0] != null) 
                currentAttackers[0].TargetDeath(this);
        }
    }

    //Change this system with IAttacker for any type of attacker.
    public void AddAttacker(IAttackers attacker)
    {
        currentAttackers.Add(attacker);
    }

    public void RemoveAttacker(IAttackers attacker)
    {
        currentAttackers.Remove(attacker);   
    }



    public bool SendCursorInformation(RTSUnit unit)
    {
        if(unit == null) return false;
        if(unit.GetComponent<Attacker>())
        {   
                //MouseCursorManager.instance.SetCursor(interactCursorType);
                MouseCursorManager.instance.SetCursor(MouseCursorManager.MouseCursors.Attack);
                return true;
        }
        return false;
    }

    public bool StartUnitsInteractions(RTSUnit unit)
    {
        //Controls the interacted unit has attacker or not.
        if(unit.TryGetComponent<Attacker>(out Attacker attacker))
        {
            attacker.StartAttackTarget(this);
            return true;
        }

        return false;
    }

    public int  GetInteractionPriority()
    {
        return interactionPriority;
    }
}
