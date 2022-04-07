using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildConstructionBenefit : ScriptableObject
{
    //Buildings will be parameter maybe in some cases it's could be needed.
    public abstract void BenefitAction(Buildings building);
    public abstract void UndoBenefitWhenDestroy(Buildings building);
    //TODO when building demolished, this benefit may gone also create function for this.
}
