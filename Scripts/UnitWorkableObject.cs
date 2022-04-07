using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelectableObject))]
public abstract class UnitWorkableObject : MonoBehaviour,IUnitInteractableObject
{
    [Header("Player Interaction Settings")]
    [SerializeField] private bool disablePlayerUnitInteraction = false;
    [SerializeField] private int interactionPriority;
    [Space]
    [SerializeField] private int maxWorker = 5;
    [SerializeField] protected UnitWorkerSlots[] workerSlots;

    [HideInInspector]
    public int currentWorker = 0;         // Daha en başta komut verildiğinde artan ve azalan aktif workerların tutulduğu değer.
    protected int activeWorkers = 0;      // Workerlar tam olarak slota gelip yerleştikten sonra artacak olan değer.

    private Collider[] hitColliders = new Collider[5];
    private int layerMask;
    public abstract bool SendCursorInformation(RTSUnit unit);
    public abstract bool StartUnitsInteractions(RTSUnit unit);
  

    protected virtual void Start() 
    {   
        //These are the layers work slot will ignore.
        int terrainlayerBit = 1 << LayerMask.NameToLayer("Terrain");
        int enemyDetectionLlayer = 1 << LayerMask.NameToLayer("EnemyObjectDetection");
        int visiblefogLayer = 1 << LayerMask.NameToLayer("VisibleFogObject");
        int visiblefogInteractorlayer = 1 << LayerMask.NameToLayer("VisibleFogInteractor");
        int unitInteractionLayer = 1 << LayerMask.NameToLayer("PlayerUnitInteractionLayer");
        layerMask = terrainlayerBit | enemyDetectionLlayer | visiblefogLayer | visiblefogInteractorlayer | unitInteractionLayer;
        layerMask = ~layerMask;

        if(!disablePlayerUnitInteraction)
        {
            GetComponentInChildren<ObjectPlayerInteraction>().AddInteraction(this);
        }
    }

    public virtual bool AllowPlayerUnitInteraction()
    {
        return !disablePlayerUnitInteraction;
    }

    public virtual bool AreThereAnySlotForWorker ()
    {
        if(currentWorker >= maxWorker)
        {
            //Max sayıda üniteye ulaştıysak false döndürücek.
            //TODO Oyuncuya uyarı mesajı verilebilir.
            return false; 
        }

        else
        {
            // İlk olarak slot yerlerinin hepsinde collider oluşturup bakılacak eğer collider var ise, unavailable yapılacak.
            for(int i = 0 ; i < workerSlots.Length;i ++)
            {
                    int hitCount = Physics.OverlapSphereNonAlloc(workerSlots[i].slotTransform.position,1f,hitColliders,layerMask);
                    if(hitCount >= 1)
                    {
                        for(int j = 0; j < hitCount ;j++)
                        {
                            
                            if(hitColliders[j].gameObject != this.gameObject)
                            {
                                workerSlots[i].Available = false;
                                break;
                            }
                        }
                    }
            }

            for(int i = 0 ; i < workerSlots.Length; i++)
            {
                if(workerSlots[i].Available) return true;   //If atleast one slot available this function returns true.
            }
        }

        //If cant find any slot after check every slot we just return false.
        return false;
    }

    //This method assume that there is atleast one available slot for worker.
    public virtual bool TryAddWorker(ref int index,IWorkerUnits worker,ref Vector3 pos)
    {

        if(!AreThereAnySlotForWorker()) 
        {
            return false;
        }
        
            // Available olan slotlar tek tek kontrol edilip üniteye en yakın slot belirlenip onun pozisyonu döndürülecek.
            int selectedIndex = 0;
            float selectedDistance = Mathf.Infinity;

            for(int i = 0; i < workerSlots.Length;i++)
            {
                if(workerSlots[i].Available)
                {
                    //Uzaklık kontrolü yapılacak.
                    Vector3 vecDistance = workerSlots[i].slotTransform.position - worker.GetPosition();
                    float currentlen = vecDistance.sqrMagnitude;
                    if(currentlen < selectedDistance)
                    {
                        selectedDistance = currentlen;
                        selectedIndex = i;
                    }
                }
            }

            //We found the available slot.    
            pos = workerSlots[selectedIndex].slotTransform.position;
            index = selectedIndex;
            workerSlots[selectedIndex].Available = false;
            workerSlots[selectedIndex].Worker = worker;
            currentWorker ++;   

            return true;
    }
    
    public virtual bool RemoveWorker(int index)
    {
        if(currentWorker <= 0 ) return false;

        currentWorker--;
        workerSlots[index].Available = true;
        workerSlots[index].Worker = null;
        if(workerSlots[index].WorkerSettled)
        {
            activeWorkers--;
            workerSlots[index].WorkerSettled = false;
        }

        return true;
    }


    public virtual void WorkerSettled(int index)
    {
        workerSlots[index].WorkerSettled = true;
        activeWorkers ++;
    }

    public virtual int  GetInteractionPriority()
    {
        return interactionPriority;
    }
}



[System.Serializable]
public class UnitWorkerSlots
{
    public Transform slotTransform;

    private IWorkerUnits currentworker;
    private bool available = true;
    private bool workerSettled = false;

    public bool Available {get {return available;} set {available = value;}}
    public IWorkerUnits Worker {get {return currentworker;} set {currentworker = value;}}
    public bool WorkerSettled {get {return workerSettled;} set {workerSettled = value;}}

}
