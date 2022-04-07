using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingsUIElement", menuName = "InformationUIElements/BuildingsUIElement", order = 2)]
public class BuildingsUIElement : InformationUIElements
{
  

    public override void ShowUIElements(bool isSingleSelection)
    {
        //At here send this elements to gameplayuimanager.
        if(isSingleSelection)
        GameplayUIManager.instance.OpenBaseUIPanel(selectionName,selectionIcon);
        else
        {
           GameplayUIManager.instance.ActivateAndAddObjectSelection(this);
        }
    }
}
