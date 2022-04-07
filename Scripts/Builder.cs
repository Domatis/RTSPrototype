using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour,IBaseAction,IWorkerUnits
{
    [SerializeField] private LayerMask buildingsLayer;
    [SerializeField] private float searchBuildRadius = 10f;
    [Space]
    [SerializeField] private AudioClipData constructionSound;

    private Buildings currentBuilding;
    private int workerindex;

    private RTSUnit unitRef;
    private bool movingToConstruct = false;

    private void Awake() 
    {
        unitRef = GetComponent<RTSUnit>();
    }



    private void Start() 
    {
        unitRef.mover.ReachedDestinationEvent += ReachedToConstruct;
    }

    public void ReachedToConstruct(bool success)
    {

        if(!movingToConstruct) return;

        if(success) ConstructionStart();

        //Builders did not reach to consturck successfully
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


    public void  ConstructionStart()
    {
        movingToConstruct = false;
        currentBuilding.WorkerSettled(workerindex);
        transform.LookAt(currentBuilding.transform);
        //Resource toplama animasyonunu başlat.
        unitRef.animator.SetBool("Constructing",true);
    }

    public void StartBuildConstruction(Buildings building,out bool state)
    {
        state = false;

        unitRef.GetUnitActionStateManager().SetNewAction(this);

        Vector3 workerPos = Vector3.zero;

        if(building.TryAddWorker(ref workerindex,this,ref workerPos))
        {
            currentBuilding = building;
            unitRef.mover.MoveToDestinationStart(workerPos);
            movingToConstruct = true;
            state = true;
        }
        
        else
        {
            state = false;
        } 
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void CurrentWorksFinished()
    {
        
        unitRef.animator.SetBool("Constructing",false); 
        movingToConstruct = false;
        if(!FindAnotherConstruction(currentBuilding)) currentBuilding = null;
    }

    public bool FindAnotherConstruction(Buildings buildingref)
    {
        Collider[] allhits = Physics.OverlapSphere(transform.position,searchBuildRadius,buildingsLayer);
        List<FoundBuildings> foundbuildings = new List<FoundBuildings>();

        //Find the propriate buildings.
        for(int i = 0; i < allhits.Length;i++)
        {
            if(allhits[i].gameObject.TryGetComponent<Buildings>(out Buildings building))
            {
                //Make sure the building is need construction and also available to player unit interaction.
                if(building.AllowPlayerUnitInteraction() && building.IsRepairRequired()&& buildingref != building)
                {
                    Vector3 vecDist  = building.transform.position - buildingref.transform.position;
                    float sqrDist = vecDist.sqrMagnitude;
                    foundbuildings.Add(new FoundBuildings(building,sqrDist));
                }
            }
        }

        //Sort the buildings with given distances.
        for(int i =0; i < foundbuildings.Count;i++)
        {
            bool swapped = false;

            for(int j =0; j< foundbuildings.Count - i -1;j++)
            {
                if(foundbuildings[j].sqrdistanceToResource > foundbuildings[j+1].sqrdistanceToResource)
                {
                    FoundBuildings temp = foundbuildings[j];
                    foundbuildings[j] =foundbuildings[j+1];
                    foundbuildings[j+1] = temp;
                    swapped = true;
                }
            }
            if(!swapped) break; //Early quit if list already sorted.
        }


        //Try all buildings from start.
        for(int i=0; i < foundbuildings.Count;i++)
        {
            StartBuildConstruction(foundbuildings[i].building,out bool state);
            if(state) return true;
            else continue;
        }

        return false;

    }

    public void CancelAction()
    {

        if(currentBuilding == null) return;
        unitRef.animator.SetBool("Constructing",false); 
        movingToConstruct = false;
        currentBuilding.RemoveWorker(workerindex);
    }

    public void PlayConstructHitSound()
    {
        unitRef.audioSourceManager.playAudioClip(constructionSound);
    }
}

public struct FoundBuildings
{
    public FoundBuildings(Buildings r,float distance)
    {
        building = r;
        sqrdistanceToResource = distance;
    }

    public Buildings building;
    public float  sqrdistanceToResource;
}
