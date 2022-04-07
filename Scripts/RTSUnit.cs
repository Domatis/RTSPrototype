using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


[RequireComponent(typeof(SelectableObject))]
[RequireComponent(typeof(Mover),typeof(UnitActionStateManager))]
[RequireComponent(typeof(Rigidbody),typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator)),RequireComponent(typeof(Health))]
public class RTSUnit : MonoBehaviour
{
    public enum UnitTypes {None,Worker,Soldier}
    protected UnitTypes unittype = UnitTypes.Worker;    //TODO remove later if not necessary

    public Action UnitDeathAction;

    [SerializeField] protected Health unitHealth;
    public Mover mover;
    public Animator animator;
    public ObjectAbilityManager abilityManager;
    public AudioSourceManager audioSourceManager;
    [SerializeField] private float warnAllysRadius = 3.5f;
    [SerializeField] private LayerMask layerForWarnAllies;
    [SerializeField] private AudioClipData deathSound;
    
    public UnitTypes unitType {get {return unittype;}}  //TODO remove later if not necessary
    protected UnitActionStateManager actionStateManager;



    public UnitActionStateManager GetUnitActionStateManager()
    {
        return actionStateManager;
    }

    protected virtual void Awake() 
    {
        actionStateManager = GetComponent<UnitActionStateManager>();
    }

    private void Start() 
    {
        
        //When unit has zero health, it's going to die.
        unitHealth.HealthAtMinimumValue += DestroyUnit;

        if(TryGetComponent<DamageableObject>(out DamageableObject damageableObject))
        {
            damageableObject.DamageTaken += WarnAllyAttackersAround;
        }
    }

    //Calling from animation. Every unit must have death animation and function.
    public void UnitDieMethod()
    {
        //Basically destroy it. 
        Destroy(gameObject);    //TODO we can apply pool system later.
    }


    public virtual void DestroyUnit()
    {
        
        //When this unit selected and also information about this unit could be on interface, when this unit dies remove it from actives lists, and close the information ui about it.
        if(TryGetComponent<SelectableObject>(out SelectableObject selectableObject))
        {
            if(selectableObject.IsObjectSelected)
            {
                if(selectableObject.GetSelectionType() == SelectableObjectTypes.playerUnits)
                {
                    PlayerRTSController.instance.RemoveRTSUnitFromList(this);
                }

                if(selectableObject.IsSingleSelected)
                {
                    GameplayUIManager.instance.CloseAbilitiesUI();
                    GameplayUIManager.instance.CloseUIPanel();
                }

                //Multiple Selected units.
                else
                {
                    GameplayUIManager.instance.DeActiveObjectFromSelection(selectableObject.UIElements);
                }
            }
        }
        UnitDeathAction?.Invoke();
        animator.SetBool("Death",true);
    }
    

    //Movement ortak olacağından movement kısmı rts ünit'de kalacak.
    public virtual void MoveToPosition(Vector3 pos)
    {
         mover.StartMovementAction(pos);
    }

    public void WarnAllyAttackersAround(GameObject target)
    {
        if(target == null) return;
        //For a certain layer, this unit will warn ally attackers around it self.
         Collider[] allhits = Physics.OverlapSphere(transform.position,warnAllysRadius,layerForWarnAllies);

        for (int i = 0; i < allhits.Length; i++)
        {
            if(allhits[i].TryGetComponent<Attacker>(out Attacker allyattacker))
            {
                if(allyattacker.gameObject != this && allyattacker.GetComponent<UnitActionStateManager>().GetCurrentAction().InterruptableWithDamageTaken() && allyattacker.attackAvailable)    //Do not warn itself.
                {
                    if(target.TryGetComponent<DamageableObject>(out DamageableObject targetDamageable))
                    {
                        if(targetDamageable.DamageTakenAvailable) allyattacker.StartAttackTarget(targetDamageable);
                    }              
                }
            }
        }
    }

    public void PlayDeathSound()
    {
        audioSourceManager.playAudioClip(deathSound);
    }
}
