using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangeAttackType",menuName = "AttackTypes/MeleeAttackType",order = 4 )]
public class MeleeAttackType : BaseAttackType
{
    public override void MakeAttack(DamageableObject target, GameObject parentAttackerObj)
    {
        target.TakeDamage(damage,parentAttackerObj);
    }

}
