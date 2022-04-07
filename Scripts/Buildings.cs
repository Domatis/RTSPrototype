using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BoxCollider),typeof(SelectableObject))]
[RequireComponent(typeof(Health),typeof(ObjectAbilityManager))]
public class Buildings : UnitWorkableObject
{   
    public Action<Buildings> BuildingConstructionFinished;
    public Action<Buildings> BuildingDestructionAction;

    [SerializeField] private BuildConstructionBenefit[] constructionBenefits;
    [SerializeField] private GameObject constructionSite;
    [SerializeField] private Health buildHealth;
    [SerializeField] private float constructSpeedperSec = 5f;
    [SerializeField] private ObjectAbilityManager abilityManager;
    [SerializeField] private bool readyAtStart = false;



    [HideInInspector]
    public bool onConstruction = true;
    [HideInInspector]
    public bool onRepairing;

    private bool objectConstructed = false;

    private void Awake() 
    {
        onConstruction = true;
    }


    protected override void Start() 
    {
        base.Start();
        constructionSite.SetActive(true);   //Make sure it's active.
        abilityManager.ShowAbilities = false;   //At first we'r in construction, we dont show the abilities to ui.
        buildHealth.UpdateHealth(0);

        //Obje seçimi fonksiyonların aksiyonlara eklenmesi.
        SelectableObject tempobj = GetComponent<SelectableObject>();
    
        buildHealth.HealthAtMaximumValue += BuildRepairOrConstructFinished;
        buildHealth.HealthAtMinimumValue += BuildDestruction;
        if(readyAtStart) BuildRepairOrConstructFinished();
    }

    private void Update() 
    {
        if(onConstruction || onRepairing)
        {
            buildHealth.IncreaseHealth(activeWorkers * constructSpeedperSec * Time.deltaTime);
        }
    }

    

    public override bool TryAddWorker(ref int index,IWorkerUnits worker,ref Vector3 pos)
    {
       // Debug.Log("OnConstruction = " + onConstruction);

        if(!AreThereAnySlotForWorker()) return false;

        if(onConstruction)
        {
             return base.TryAddWorker(ref index,worker,ref pos);
        }
        if(IsRepairRequired())
        {
            onRepairing = true;
            return base.TryAddWorker(ref index,worker,ref pos);
        }
        

        return false;
    }

    public override bool SendCursorInformation(RTSUnit unit)
    {

        if(!IsRepairRequired() && !onConstruction) return false;

        if(unit.GetComponent<Builder>())
        {   
                MouseCursorManager.instance.SetCursor(MouseCursorManager.MouseCursors.Construction);
                return true; 
        }
        return false;
    }

    //That method will call when the player click this object with right mouse click.
    public override bool StartUnitsInteractions(RTSUnit unit)
    {
        if(!IsRepairRequired() && !onConstruction) return false;


        //We'r using nested if's because dont want to all controls at once. it can impact to performance.
        Builder builder = unit.GetComponent<Builder>();
        if(builder != null)    //If unit has gatherer object, start resource gathering and return true.
        {    
               
            builder.StartBuildConstruction(this,out bool state);
            if(state) return true;
            else return builder.FindAnotherConstruction(this);
        }

        //If it does not return true it will return false.
        return false;
    }


    public bool IsRepairRequired()
    {
        if(!buildHealth.IsHealthFull())
        {
            return true;
        }

        else return false;
    }


    public void BuildDestruction()
    {

        //Undo all benefits when gained this building constructed.
        for(int i =0; i< constructionBenefits.Length;i++)
        {
            constructionBenefits[i].UndoBenefitWhenDestroy(this);
        }
        // When this building selected an active object in ui, just close the ui.
        if(TryGetComponent<SelectableObject>(out SelectableObject selectableObject))
        {
            if(selectableObject.IsObjectSelected)
            {
                if(selectableObject.IsSingleSelected)
                {
                    GameplayUIManager.instance.CloseAbilitiesUI();
                    GameplayUIManager.instance.CloseUIPanel();
                }

                 //Multiple Selected units.
                else
                {
                    GameplayUIManager.instance.DeActiveObjectFromSelection(selectableObject.UIElements);
                }
            }            
        }
        BuildingDestructionAction?.Invoke(this);
        Destroy(gameObject);
    }

   
    public void BuildRepairOrConstructFinished()
    {

        if(!objectConstructed)
        {
            BuildingConstructionFinished?.Invoke(this);
            for(int i = 0; i < constructionBenefits.Length;i++)
            {
                constructionBenefits[i].BenefitAction(this);
            }
            //Debug.Log("Build Finished");
            objectConstructed = true;
            abilityManager.ShowAbilities = true;
            Destroy(constructionSite);
        }

        //Bina ilk halinde kuruluna kadar seçili olmamalı onun yerine bina inşaat nesnesi seçili olmalı.

        onConstruction = false;
        onRepairing = false;

        //Bina tamamlandığında çalışan işcilere haber verilmesi.
        for(int i = 0 ; i < workerSlots.Length ; i++)
        {
            workerSlots[i].Worker?.CurrentWorksFinished();
        }

        buildHealth.MaximizeHealth();
        // TODO animasyon vs binanın son haline gelmesi görsel olarak.
    }
}
