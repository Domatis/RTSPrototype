using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle),typeof(BoxCollider))]
public class Resources : UnitWorkableObject
{
   
    

    public ResourceManager.ResourceTypes resourcetype = ResourceManager.ResourceTypes.gold;
    [SerializeField] private float maxResourceAmount = 1000;
    [SerializeField] private float gatheramountperSecperUnit = 1;

    
    private bool updateMainResourceUI = false;
    private float currentResourceAmount = 0;
    private MouseCursorManager.MouseCursors cursorType;

    public ResourceManager.ResourceTypes GetResourceType {get{return resourcetype;}}

    private void Awake() 
    {
        currentResourceAmount = maxResourceAmount;
    }

    protected override void Start() 
    {
        base.Start();

        //Resourcelar için seçili olduğunda çalışması gereken fonksiyonları selectable aksiyonlarına gönderilmesi.
        SelectableObject  tempobj = GetComponent<SelectableObject>();
        tempobj.OnObjectSelection += ResourceOnObjectSelection;
        tempobj.OnObjectDeselection += () => {updateMainResourceUI = false;};

        //Determine the cursor with the current resource type
        switch(resourcetype)
        {
            case ResourceManager.ResourceTypes.gold:
            cursorType = MouseCursorManager.MouseCursors.CollectGold;
            break;
            case ResourceManager.ResourceTypes.wood:
            cursorType = MouseCursorManager.MouseCursors.CollectWood;
            break;
            case ResourceManager.ResourceTypes.meal:
            cursorType = MouseCursorManager.MouseCursors.CollectMeal;
            break;
        }
    }


    private void Update() 
    {
        //Resourceda kaldıkça resource artması olayını yaz.

        if(currentResourceAmount <= 0)
        {
            ResourceEnding();
            return;
        }

        if(activeWorkers > 0)  //Çalışmayan işçi yok ise, değerler yok yere güncellenmesin.
        {
            currentResourceAmount -= gatheramountperSecperUnit * activeWorkers * Time.deltaTime;
            ResourceManager.instance.IncreaseResource(gatheramountperSecperUnit * activeWorkers * Time.deltaTime,resourcetype);
            //Eğer bu resource seçilmiş ise genel arayüzde ki resource bilgisini güncelleyecek.
            if(updateMainResourceUI)    GameplayUIManager.instance.UpdateSelectedResourceInfo(((int)currentResourceAmount).ToString() + "/" + maxResourceAmount.ToString());
            
        }
    }

    public override bool SendCursorInformation(RTSUnit unit)
    {
        if(unit.GetComponent<ResourceGatherer>())
        {   
                MouseCursorManager.instance.SetCursor(cursorType);
                return true;        
        }
        return false;
    }

    //That method will call when the player click this object with right mouse click.
    public override bool StartUnitsInteractions(RTSUnit unit)
    {
        ResourceGatherer gatherer = unit.GetComponent<ResourceGatherer>();
        if(gatherer != null)    //If unit has gatherer object, start resource gathering and return true.
        {
            gatherer.StartResourceGathering(this,out bool isSuccess);
            if(isSuccess) return true;
            else
            {
                return gatherer.FindAnotherResource(this); //If target unit cant add this resource, try another resource to find and return it's result.
            }
        }

        //If it does not return true it will return false.
        return false;
    }



    public override bool TryAddWorker(ref int index, IWorkerUnits worker, ref Vector3 pos)
    {
        bool state = base.TryAddWorker(ref index, worker, ref pos);

        if(state)
        {
            ResourceManager.instance.IncreaseCurrentWorkerCount(resourcetype,1);
        }
        return state;
    }

    public override bool RemoveWorker(int index)
    {
        bool state = base.RemoveWorker(index);

        if(state)
        {
            ResourceManager.instance.DecreaseCurrentWorkerCount(resourcetype,1);
        }
        return state;
    }
    
    public void ResourceEnding()
    {
        //Kayıtlı workerlara haber verilmesi.
        for(int i = 0 ; i < workerSlots.Length ; i++)
        {
            workerSlots[i].Worker?.CurrentWorksFinished();
        }

        ResourceManager.instance.DecreaseCurrentWorkerCount(resourcetype,currentWorker);

        if(TryGetComponent<SelectableObject>(out SelectableObject selectableObject))
        {
            if(selectableObject.IsSingleSelected)
            {
                GameplayUIManager.instance.CloseAbilitiesUI();
                GameplayUIManager.instance.CloseUIPanel();
            }
        }

        GameObject.Destroy(gameObject);
    }

    public  void ResourceOnObjectSelection(bool uistate)
    {
        GameplayUIManager.instance.OpenAbilitiesUi(null,false);
        if(uistate)
        {   
            //Kendi resource değerinin genel arayüzde ki değeri değiştirilmesi için.
            updateMainResourceUI = true;
            GameplayUIManager.instance.UpdateSelectedResourceInfo(((int)currentResourceAmount).ToString() + "/" + maxResourceAmount.ToString());
            GameplayUIManager.instance.UpdateSelectedHealthInfo("0/0");
        }
    
    }
}

