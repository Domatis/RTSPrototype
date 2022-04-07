using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public enum SelectableObjectTypes {Default,playerUnits,Buildings,Resources,enemyUnits} 

public class SelectableObject : MonoBehaviour
{   

    public Action<bool> OnObjectSelection;
    public Action OnObjectDeselection;

    [SerializeField] private SelectableObjectTypes selectionType; 
    [SerializeField] private GameObject visualselectionQuad;
    [SerializeField] private InformationUIElements uIElements;

    private bool isObjectSelected = false;
    private bool isSingleSelected = false;
    private bool objectSelectable = true;

    public bool IsObjectSelected {get{return isObjectSelected;}}
    public bool IsSingleSelected {get{return isSingleSelected;}}
    public bool ObjectSelectable {get {return objectSelectable;} set{objectSelectable = value;} }
    public InformationUIElements UIElements{get {return uIElements;}}




    private void Start() 
    {
        visualselectionQuad.SetActive(false);
    }


    public SelectableObjectTypes GetSelectionType()
    {
        return selectionType;
    }

    public void ObjectSelected(bool singleSelection)
    {
        if(!objectSelectable) return;

        visualselectionQuad.SetActive(true);
        isObjectSelected = true;   
        uIElements.ShowUIElements(singleSelection);
        isSingleSelected = singleSelection;
        OnObjectSelection?.Invoke(singleSelection);
    }

    public void ObjectDeselected()
    {
        visualselectionQuad.SetActive(false);
        isObjectSelected = false;
        isSingleSelected = false;
        OnObjectDeselection?.Invoke();
    }
}
