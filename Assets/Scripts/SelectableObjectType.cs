using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectableObjectType", menuName = "Gameplay/SelectableObjectType", order = 1)]
public class SelectableObjectType : ScriptableObject
{

    public SelectableObjectTypes objectType;
    [Range(0,1000)] [Tooltip("Lower values are high priority")]
    public int priority = 1;
    public bool multipleSelection = false;
}
