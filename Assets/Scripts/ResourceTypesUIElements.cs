using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceTypesUIElement", menuName = "InformationUIElements/ResourceTypesUIElements", order = 4)]
public class ResourceTypesUIElements : ScriptableObject
{
    public string resourceName;
    public Sprite resourceImage;
}
