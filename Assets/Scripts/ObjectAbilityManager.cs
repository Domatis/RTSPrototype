using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectAbilityManager : MonoBehaviour
{
   
    //Max 9 adet olabilir.
    [SerializeField] private ObjectAbilities[] abilities;

    [HideInInspector]
    private bool showAbilities = true;
    private bool objectInSelection = false;

    public bool ShowAbilities 
    {
        set
        {
            showAbilities = value;
            if(objectInSelection)
            {
                if(value)   // Enabling show Abilities.
                {
                    for(int i = 0 ; i < abilities.Length;i++)
                    {
                        abilities[i].ability.SetParentObject(gameObject);
                    }

                    GameplayUIManager.instance.OpenAbilitiesUi(abilities,true);
                }

                else GameplayUIManager.instance.OpenAbilitiesUi(null,false);
            }
        }

        get {return showAbilities;}
    }

    private void Start() 
    {
        SelectableObject selectableObject = GetComponent<SelectableObject>();
        if(selectableObject)
        {
            //If this object selected, we send the ability information to ui.
            selectableObject.OnObjectSelection += (state) =>
            {
                if(state) SendAbilityInformationToUI();
                else    //If multiple selection objects just show the common abilities.
                {
                    GameplayUIManager.instance.OpenAbilitiesUi(abilities,false);
                } 
            };

            selectableObject.OnObjectDeselection += () =>
            {
                objectInSelection = false;
            };
        }
    }

    public void SendAbilityInformationToUI()
    {
        objectInSelection = true;
        if(!showAbilities)
        {
            GameplayUIManager.instance.OpenAbilitiesUi(null,true);
            return;
        }

        //Else
        //Before the send abilities we need to update the parent object value of abilities.
        for(int i = 0 ; i < abilities.Length;i++)
        {
            abilities[i].ability.SetParentObject(gameObject);
        }

        GameplayUIManager.instance.OpenAbilitiesUi(abilities,true);
    }
}

[System.Serializable]
public class ObjectAbilities
{
    public Abilities ability;
    [Range(1,9)]
    public int order;
}


