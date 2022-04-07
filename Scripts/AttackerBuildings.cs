using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AttackerBuildings : MonoBehaviour,IAttackers
{
    
    [SerializeField] private BuildingAttackType attackData;
    [SerializeField] private ObjectDetectionCollider detectionCollider;
    [SerializeField] private GameObject projectileSpawnPoint;
    [SerializeField] private AudioSourceManager audioSourceManager;
    [SerializeField] private AudioClipData attackSoundClipData;

    private List<DamageableObject> currentTargets = new List<DamageableObject>();
    private float attackTimer = 0;
    private bool attackingOn;

    public GameObject GetProjectileSpawnPoint {get{return projectileSpawnPoint;}}
    
    private void Start() 
    {   
        //If this attacker building has player building, it's need start attack when building full constructed.
        if(TryGetComponent<Buildings>(out Buildings building))
        {       
            attackingOn = false;   // At start attacking will be off.
            building.BuildingConstructionFinished +=(currentbuild) =>
            {
                attackingOn = true; //Attacking will be on when build constructed.
            };
        }
        //If there is no player building component just make attacking on.
        else attackingOn = true;

        if(TryGetComponent<SelectableObject>(out SelectableObject selectableObject))
        {
            selectableObject.OnObjectSelection += (state) =>
            {
                if(state)
                {
                    GameplayUIManager.instance.ShowAttackerUIElements(attackData.damage.ToString(),attackData.attackDistance.ToString());
                }
            };
        }

        detectionCollider.ObjectDetectedAction += TargetInRange;
        detectionCollider.ObjectExitAction += TargetOutOfRange;
        SphereCollider detectSphere = detectionCollider.GetComponent<SphereCollider>();     //Buildings detection colliders must have sphere colliders.
        detectSphere.radius = attackData.attackDistance;
    }

    private void Update() 
    {
        if(!attackingOn) return;

        attackTimer += Time.deltaTime;
        if(attackTimer >= attackData.attackEverySec)
        {   
            int selectedIndex = -1;
            float distance = Mathf.Infinity;

            for(int i = 0 ; i < currentTargets.Count; i++)
            {
                //Find the nearest target and attack it.
                if(currentTargets[i] == null) continue;
                Vector3 vecDistance = currentTargets[i].transform.position - transform.position;
                float sqDistance = vecDistance.sqrMagnitude;
                if(sqDistance <= distance)
                {
                    distance = sqDistance;
                    selectedIndex = i;
                }
            }
            if(selectedIndex != -1)
            {
                attackData.MakeAttack(currentTargets[selectedIndex],gameObject);
                audioSourceManager.playAudioClip(attackSoundClipData);
                attackTimer = 0;    //Attack timer reset when make attack.
            } 

            else attackTimer = attackData.attackEverySec; //When time is up, but no target to attack just set the timer the threshhold value.   
            
        }
    }


    public void TargetInRange(GameObject target)
    {
        if(!attackingOn) return;
        if(target.TryGetComponent<DamageableObject>(out DamageableObject targetDamageable))
        {
            targetDamageable.AddAttacker(this);
            currentTargets.Add(targetDamageable);
        }
        detectionCollider.DetectionOn = true;
    }

    public void TargetOutOfRange(GameObject target)
    {
        if(!attackingOn) return;
        if(target.TryGetComponent<DamageableObject>(out DamageableObject targetDamageable))
        {
            targetDamageable.RemoveAttacker(this);
            currentTargets.Remove(targetDamageable);
        }
    }

    public void TargetDeath(DamageableObject target)
    {
        try
        {
            currentTargets.Remove(target);
        }

        catch(Exception e)
        {
            
        }
        
    }

}
