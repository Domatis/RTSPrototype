using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BuildingAttackType",menuName = "AttackTypes/BuildingAttackType",order =4 )]
public class BuildingAttackType : BaseAttackType
{
    public GameObject projectile;

    public override void MakeAttack(DamageableObject target, GameObject parentObj)
    {
        //Instatiate projectile and set the stats.
        AttackerBuildings currentAttacker = parentObj.GetComponent<AttackerBuildings>();

        GameObject prjectile = Instantiate(projectile,currentAttacker.GetProjectileSpawnPoint.transform.position,Quaternion.identity);
        prjectile.GetComponent<GuidedProjectile>().SetCurrentTarget(target,damage,parentObj);
    }

}
