using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGatherer : MonoBehaviour,IBaseAction,IWorkerUnits
{


    [SerializeField] private LayerMask resourcesLayer;
    [SerializeField] private float resourceSearchRadius = 5f;
    [Header("Sounds")]
    [SerializeField] private AudioClipData chopTreeSound;
    [SerializeField] private AudioClipData miningSound;
    [SerializeField] private AudioClipData gatheringSound;


    private Resources currentresource;
    private int workerindex;
    private RTSUnit mainunitref;

    private bool movingtoResource = false;

    private void Awake() 
    {
        mainunitref = GetComponent<RTSUnit>();
    }


    private void Start() 
    {
        mainunitref.mover.ReachedDestinationEvent += ReachedToResource;
    }

    

    public void ReachedToResource(bool success)
    {
        if(!movingtoResource) return;

        if(success && currentresource != null)
        {
            GatheringStart();
        }

        //Gatherers did not reach to resource successfully
        else    CancelAction();
    }

    public bool InterruptableWithDamageTaken()
    {
        return false;
    }

    public bool InterruptableWithEnemySeen()
    {
        return false;
    }


    //Animasyon için gatherindexte gold için 0, wood için 1, meal için 2
    public void GatheringStart()
    {
        movingtoResource = false;
        currentresource.WorkerSettled(workerindex);
        
        //Bu noktada yüzlerinin kaynağa dönük olmalarını sağla.
        transform.LookAt(currentresource.transform);


        if(currentresource.resourcetype == ResourceManager.ResourceTypes.gold)
        {
           //Mining animasyonu
           mainunitref.animator.SetBool("ResourceGathering",true);
           mainunitref.animator.SetInteger("ResourceGatherIndex",0);

        }

        else if(currentresource.resourcetype == ResourceManager.ResourceTypes.wood)
        {
            //Wood chop animasyonu
           mainunitref.animator.SetBool("ResourceGathering",true);
           mainunitref.animator.SetInteger("ResourceGatherIndex",1);
        }

        else if(currentresource.resourcetype == ResourceManager.ResourceTypes.meal)
        {
           //Meyve toplama animasyonu
           mainunitref.animator.SetBool("ResourceGathering",true);
           mainunitref.animator.SetInteger("ResourceGatherIndex",2);
        }
    }

    public void StartResourceGathering(Resources resource,out bool gatheringSuccess)
    {
        mainunitref.GetUnitActionStateManager().SetNewAction(this);
        Vector3 workPosition = Vector3.zero;
        gatheringSuccess = resource.TryAddWorker(ref workerindex,this,ref workPosition);  
        if(!gatheringSuccess) return; //Adding worker to current resource was not successfull just return.
 
        currentresource = resource;
        mainunitref.mover.MoveToDestinationStart(workPosition);
        movingtoResource = true;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void CurrentWorksFinished()
    {
        mainunitref.animator.SetBool("ResourceGathering",false);
        movingtoResource = false;
        //At this point search for other resources around.
        if(!FindAnotherResource(currentresource)) currentresource = null;
    }

    //This resource parameter will be reference for same type of resources.
    public bool FindAnotherResource(Resources resourceref)
    {
        Collider[] allhits = Physics.OverlapSphere(resourceref.transform.position,resourceSearchRadius,resourcesLayer);
        List<FoundResources> resources = new List<FoundResources>();

        //Find the propriate resources.
        for(int i = 0; i < allhits.Length;i++)
        {
            if(allhits[i].gameObject.TryGetComponent<Resources>(out Resources resource))
            {
                //Make sure gatherer keep continue gather same resource, and make sure we did'nt collide with same resource objectç
                if(resource.GetResourceType == resourceref.GetResourceType && resourceref != resource && resource.AllowPlayerUnitInteraction())
                {
                    Vector3 distanceVec = resource.transform.position - resourceref.transform.position;
                    float sqrDist = distanceVec.sqrMagnitude; 
                    resources.Add(new FoundResources(resource,sqrDist));
                }
            }
        }

        //Sort the resources with given distances.
        for(int i =0; i < resources.Count;i++)
        {
            bool swapped = false;

            for(int j =0; j< resources.Count - i -1;j++)
            {
                if(resources[j].sqrdistanceToResource > resources[j+1].sqrdistanceToResource)
                {
                    FoundResources temp = resources[j];
                    resources[j] = resources[j+1];
                    resources[j+1] = temp;
                    swapped = true;
                }
            }
            if(!swapped) break; //Early quit if list already sorted.
        }

        //Try all resources from start.
        for(int i =0; i < resources.Count;i++)
        {
            StartResourceGathering(resources[i].resource,out bool state);
            if(state) return true;
            else continue;
        }

        return false;
    }
    

    public void  CancelAction()
    {
        if(currentresource == null) return;
        
        movingtoResource = false;
        mainunitref.animator.SetBool("ResourceGathering",false);    // animasyon iptali.
        currentresource.RemoveWorker(workerindex);
    }

    public void PlayChopTreeSound()
    {
        mainunitref.audioSourceManager.playAudioClip(chopTreeSound);
    }

    public void PlayMiningSound()
    {
        mainunitref.audioSourceManager.playAudioClip(miningSound);
    }

    public void PlayGatheringSound()
    {
        mainunitref.audioSourceManager.playAudioClip(gatheringSound);
    }
}

public struct FoundResources
{
    public FoundResources(Resources r,float distance)
    {
        resource = r;
        sqrdistanceToResource = distance;
    }

    public Resources resource;
    public float  sqrdistanceToResource;
}
