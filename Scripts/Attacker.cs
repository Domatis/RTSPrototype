using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//TODO change this class name to AttackerUnit.
public class Attacker : MonoBehaviour, IBaseAction,IAttackers
{
    
    private RTSUnit mainunitref;
    [SerializeField] private BaseAttackType attackType;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float targetCheckRadius = 2f;
    [SerializeField] private ObjectDetectionCollider detectionCollider;

    private bool attackingOn = false;
    private bool chasingToTarget = false;
    private DamageableObject currentTarget;
    [HideInInspector]
    public Vector3 lastPosAttackingStart;
    [HideInInspector]
    public bool attackAvailable = true;

    public Action<bool> TargetDeathAction;  //When current target death, attacker start for another target if it finds this bool will be true otherwise it's will be false.

    public ObjectDetectionCollider DetectionCollider {get {return detectionCollider;}}
    public DamageableObject CurrentTarget {get{return currentTarget;}}

    public  bool IsAttackinOn
    {
        get {return attackingOn;}
    }

    private void Awake() 
    {   
        mainunitref = GetComponent<RTSUnit>();
    }

    public bool InterruptableWithDamageTaken()
    {
        return !attackingOn;
    }

    public bool InterruptableWithEnemySeen()
    {
        return !attackingOn;
    }

    private void Start() 
    {
        if(TryGetComponent<SelectableObject>(out SelectableObject selectableObject))
        {
            selectableObject.OnObjectSelection += (state) =>
            {
                if(state)
                {
                    GameplayUIManager.instance.ShowAttackerUIElements(attackType.damage.ToString(),attackType.attackDistance.ToString());
                }
            };
        }

        //If attacker is damageable, when damage taken it will start attacking which attacks itself.
        if(TryGetComponent<DamageableObject>(out DamageableObject damageableObject))
        {
            damageableObject.DamageTaken += DamageTakenFromAttacker;
        }


        detectionCollider.ObjectDetectedAction += mainunitref.WarnAllyAttackersAround;
        detectionCollider.ObjectDetectedAction += TargetObjectDetectionAction;

        mainunitref.mover.ReachedDestinationEvent += UnitReachedToTarget;

        mainunitref.UnitDeathAction += () => 
        {
            attackAvailable = false;
            if(currentTarget != null) currentTarget.RemoveAttacker(this);
        };
    }


    private void Update() 
    {
        if(attackingOn && chasingToTarget && currentTarget != null) 
        {      
            //Targets can move so we need to update the position every frame.
             mainunitref.mover.MoveToDestinationStart(currentTarget.transform.position,attackType.attackDistance);
        }
    }

    public void UnitReachedToTarget(bool reachedToTarget)
    {
        if(!attackingOn) return;

        // If unit is attacking but it's stuck before reach to target just end attacking.
        if(!reachedToTarget)
        {
            CancelAction();
            return;
        }


        if(chasingToTarget && currentTarget != null)
        {
            StartAttackAnimation();
            chasingToTarget = false;
        }
    }


    public void StartAttackTarget(DamageableObject target)
    {
        lastPosAttackingStart = transform.position;
        mainunitref.GetUnitActionStateManager().SetNewAction(this);
        currentTarget = target;
        chasingToTarget = true;
        attackingOn = true;
        currentTarget.AddAttacker(this);
        mainunitref.mover.MoveToDestinationStart(currentTarget.transform.position,attackType.attackDistance);
    }

    //When the unit in range, units make attack to target.
    public void StartAttackAnimation()
    {
        transform.LookAt(currentTarget.transform);
        mainunitref.animator.SetBool("Attack",true);
    }

    //Add this to the object detection.
    public void TargetObjectDetectionAction(GameObject obj)
    {
        detectionCollider.DetectionOn = true;

        //If current action is interruptable to target seen start attack.
        if(mainunitref.GetUnitActionStateManager().GetCurrentAction().InterruptableWithEnemySeen())
        {
            if(obj.TryGetComponent<DamageableObject>(out DamageableObject target))
            {
                if(!target.DamageTakenAvailable) return;
                detectionCollider.DetectionOn = false;
                StartAttackTarget(target);
            }
        }
    }


    //These method calling from animations.
    public void MakeDamageToTarget()
    {
        if(currentTarget != null) attackType.MakeAttack(currentTarget,gameObject);
    }

    public void DamageTakenFromAttacker(GameObject targetAttacker)
    {
        if(!GetComponent<UnitActionStateManager>().GetCurrentAction().InterruptableWithDamageTaken()) return;   //If we'r already in attacking mode we will ignore the other attackers damage.
        if(targetAttacker.TryGetComponent<DamageableObject>(out DamageableObject target))
        {
            StartAttackTarget(target);
        }
    }

    //These method calling from animations.
    public void EndAttackAnimation()
    {
        mainunitref.animator.SetBool("Attack",false);
        if(currentTarget != null)
        {
            chasingToTarget = true;
            mainunitref.mover.MoveToDestinationStart(currentTarget.transform.position,attackType.attackDistance);
        }
           
        else attackingOn = false;   //Target could die during animation we'r checking it.
    }


    public void TargetDeath(DamageableObject deathTarget)
    {
        try
        {
            currentTarget.RemoveAttacker(this);
            currentTarget = null;
            //Animasyon iptali.
            if(mainunitref != null) mainunitref.animator.SetBool("Attack",false);
            attackingOn = false;
            chasingToTarget = false;
            //This unit will check around itself and looking for any damageable object, if it finds will attack that target.
            Collider[] allhits2 = Physics.OverlapSphere(transform.position,targetCheckRadius,targetLayer.value);

            float currentsqrDistance  = Mathf.Infinity;
            DamageableObject selectedTarget = null;

            for (int i = 0; i < allhits2.Length; i++)
            {
                if(allhits2[i].TryGetComponent<DamageableObject>(out DamageableObject target)) 
                {
                    if(target.DamageTakenAvailable)
                    {
                        Vector3 vecDistance = transform.position - target.transform.position;
                        float sqrDist = vecDistance.sqrMagnitude;
                        if(sqrDist < currentsqrDistance)
                        {
                            currentsqrDistance = sqrDist;
                            selectedTarget = target;             
                        }
                    }
                }
            }

            if(selectedTarget != null)
            {
                StartAttackTarget(selectedTarget);
                detectionCollider.DetectionOn = false;
                TargetDeathAction?.Invoke(true);
                return;
            }
            
            detectionCollider.DetectionOn = true;
            TargetDeathAction?.Invoke(false);

        }
        
        catch(Exception e)
        {
            if(currentTarget != null) currentTarget.RemoveAttacker(this);
            currentTarget = null;
            if(mainunitref != null) mainunitref.animator.SetBool("Attack",false);
            detectionCollider.DetectionOn = true;
            attackingOn = false;
            chasingToTarget = false;
        }
        
    }


    public void  CancelAction()
    {
        mainunitref.animator.SetBool("Attack",false);
        attackingOn = false;
        chasingToTarget = false;
        detectionCollider.DetectionOn = true;
        if(currentTarget == null) return;
        currentTarget.RemoveAttacker(this);
        currentTarget = null;
    }

    public void PlayAttackSound()
    {
        mainunitref.audioSourceManager.playAudioClip(attackType.attackSound);
    }
}
