using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangeAttackType",menuName = "AttackTypes/RangeAttackType",order =4 )]
public class RangeAttackType : BaseAttackType
{

    public GameObject projectile;
    public float yOffset = 0;
    public float forwardOffset = 0;

    public override void MakeAttack(DamageableObject target, GameObject parentObj)
    {

        //Instatiate projectile and set the stats.
        GameObject prjectile = Instantiate(projectile,parentObj.transform.position + (Vector3.up * yOffset) + (parentObj.transform.forward * forwardOffset),parentObj.transform.rotation);
        prjectile.GetComponent<GuidedProjectile>().SetCurrentTarget(target,damage,parentObj);
    }
    
}
