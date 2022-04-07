using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InformationUIElements : ScriptableObject
{   
    public string selectionName;
    public Sprite selectionIcon;


    public  abstract void ShowUIElements(bool isSingleSelection);
}
