using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorkerUIElements", menuName = "InformationUIElements/WorkerUIElements", order = 1)]
public class WorkerUIElements : InformationUIElements
{
    
    public  override void ShowUIElements(bool isSingleSelection)
    {
        //At here send this elements to gameplayuimanager.
        if(isSingleSelection)
        {
            GameplayUIManager.instance.OpenBaseUIPanel(selectionName,selectionIcon);
            GameplayUIManager.instance.ShowUnitUIElements();
        } 
        else
        {
            GameplayUIManager.instance.ActivateAndAddObjectSelection(this);
        }
    }

}
