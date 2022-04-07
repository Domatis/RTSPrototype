using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitPathPatrolling),typeof(Attacker))]
[RequireComponent(typeof(RTSUnit))]
public class AttackerAIUnitController : MonoBehaviour
{
    [SerializeField] private float chasingDistance = 20f;

    private UnitPathPatrolling pathPatrolling;
    private Attacker attacker;
  
    private bool goingBackToPatrolling = false;
    private bool aiActive  = true;

    private void Awake() 
    {
        attacker = GetComponent<Attacker>();
        pathPatrolling = GetComponent<UnitPathPatrolling>();
    }

    private void Start() 
    {
        attacker.TargetDeathAction += WhenCurrentTargetDeath;   
        GetComponent<RTSUnit>().UnitDeathAction += () => {aiActive = false;};
        pathPatrolling.StartPatrolling(false);
    }


    private void Update() 
    {
        if(!aiActive) return;

        if(attacker.IsAttackinOn && attacker.CurrentTarget != null)
        {
            //While attacking, controls the target distance, compare with max chasing distance, if'its out of chasing distance just return back to patrolling state.
            Vector3 currentDistanceFromPatrolArea = attacker.CurrentTarget.transform.position - attacker.lastPosAttackingStart;
            if(currentDistanceFromPatrolArea.sqrMagnitude >= chasingDistance * chasingDistance)
            {
                pathPatrolling.StartPatrolling(true);
            }
        }

        //When unit is not attacking and not patrolling just going back to patrolling.
        else if(!attacker.IsAttackinOn && !pathPatrolling.IsUnitPatrolling)
        {
            pathPatrolling.StartPatrolling(true);
            attacker.DetectionCollider.DetectionOn = true;
        }
        //TODO we can add another option for ai, while multiple units attacking this unit check all of them and make sure this unit attacks the closest enenmy.
    }

    public void WhenCurrentTargetDeath(bool isFoundAnotherTarget)
    {
       // if(!aiEnabled) return;

        if(!isFoundAnotherTarget)
            pathPatrolling.StartPatrolling(false);
        
    }
}
