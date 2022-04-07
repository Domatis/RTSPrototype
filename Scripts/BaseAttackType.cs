using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAttackType : ScriptableObject
{
   public float attackDistance;
   public float damage;
   public float attackEverySec;
   public AudioClipData attackSound;
   public abstract void MakeAttack(DamageableObject target, GameObject parentAttackerObj);  //Needs to change it to IAttacker.
}
