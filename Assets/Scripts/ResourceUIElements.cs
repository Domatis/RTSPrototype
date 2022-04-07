using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceUIElements", menuName = "InformationUIElements/ResourceUIElements", order = 3)]
public class ResourceUIElements : InformationUIElements
{
    

    public ResourceTypesUIElements resourceTypesUIElements;

     public override void ShowUIElements(bool isSingleSelection)
    {
        //At here send this elements to gameplayuimanager.
        if(isSingleSelection)
        {
             GameplayUIManager.instance.OpenBaseUIPanel(selectionName,selectionIcon);
            GameplayUIManager.instance.ShowResourceUIElements(resourceTypesUIElements.resourceName,resourceTypesUIElements.resourceImage);
        }
        else
        {
            GameplayUIManager.instance.ActivateAndAddObjectSelection(this);
        }
    }

    
}
