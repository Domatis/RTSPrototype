using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GuidedProjectile : MonoBehaviour
{
    
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private float distance;

    private DamageableObject currentTarget;
    private GameObject currentAttackerParent;
    
    private bool projectileReady = false;
    private Rigidbody rb;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() 
    {
        //Aim the target and proceed.

        if(projectileReady)
        {

            if(currentTarget == null)   //Target could destroy via other attackers, just checking it.
            {          
                Destroy(gameObject); 
                return;
            }


            //Calculate the distance. 
            Vector3 distanceVector = currentTarget.DamageTakenPoint.position - transform.position;
            //distanceVector = new Vector3(distanceVector.x,0,distanceVector.z);  //We ignore the y axis.
             //Check the distance by sqr magnitude for performance. 
            if(distanceVector.sqrMagnitude <= distance * distance)
            {
                currentTarget.TakeDamage(damage,currentAttackerParent);
                Destroy(gameObject);
            }

            else
            {
                //Calculate direction and set the velocity.
                Vector3 direction = distanceVector.normalized;
                rb.velocity = direction * speed;
                transform.forward = rb.velocity;
            }
        }
    }


    public void SetCurrentTarget(DamageableObject target,float dmg,GameObject parentAttacker)
    {
        currentAttackerParent = parentAttacker;
        damage = dmg;
        currentTarget = target;
        projectileReady = true;
    }
    
}
