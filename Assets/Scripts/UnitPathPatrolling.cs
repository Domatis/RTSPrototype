using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RTSUnit))]
public class UnitPathPatrolling : MonoBehaviour,IBaseAction
{

    [SerializeField] private float standTimeAtstations = 1.5f;
    [SerializeField] private PathPoints pathPoints;

    private RTSUnit unitRef;

    private int currentIndex;
    private bool movingToStandPoint,patrollingOn,standingAtPoint;
    private float standTimer = 0;
    private Transform currentTarget;

    //That mean this unit going back to patrolling do not interrupt it.
    private bool goingBackToPatrolling = false;
    [HideInInspector] public bool alwaysInterruptable = false;


    public bool IsUnitPatrolling { get{return patrollingOn;}}
    public Transform GetPathCurrentTarget {get {return currentTarget;}}
    public PathPoints PathPoints {get{return pathPoints;} set{pathPoints = value;}}



    private void Awake() 
    {
        unitRef = GetComponent<RTSUnit>();
    }


    private void Start() 
    {
        unitRef.mover.ReachedDestinationEvent += ReachedToDestination;
    }


    public bool InterruptableWithDamageTaken()
    {
        if(alwaysInterruptable) return true;
        return !goingBackToPatrolling;
    }

    public bool InterruptableWithEnemySeen()
    {
        if(alwaysInterruptable) return true;
        return !goingBackToPatrolling;
    }

    private void Update() 
    {

        if(patrollingOn && standingAtPoint)
        {
            standTimer += Time.deltaTime;    
            if(standTimer >= standTimeAtstations)
                MoveToNextStation();
            
            
        }
    }

    private void ReachedToDestination(bool success)
    {
        if(!patrollingOn || !movingToStandPoint) return;

        //That part is patrolling on and unit moving the point.

            //Does not matter we reached to point or not same action will happen for both state.   
            goingBackToPatrolling = false;
            movingToStandPoint  = false;
            standingAtPoint = true;
            standTimer = 0;
        
    }

    public void MoveToNextStation()
    {
        //Check the current index if it's reached to max keep standing at point.

        standTimer = 0;
        int oldIndex = currentIndex;
        currentIndex ++;
        if(currentIndex >= pathPoints.GetStationPoints.Length) currentIndex = 0;
        if(oldIndex == currentIndex)    //That happens only when one point is exist, just keep standing at point.
            return;
        //Else
            standingAtPoint = false;
            movingToStandPoint = true;
            currentTarget = pathPoints.GetStationPoints[currentIndex].transform;  //Update the position with current index.
            unitRef.mover.MoveToDestinationStart(currentTarget.position);
        
        
    }

    public void StartPatrolling(bool goingBackPatrolling)
    {
        if(pathPoints == null) return;
        goingBackToPatrolling = goingBackPatrolling;
        unitRef.GetUnitActionStateManager().SetNewAction(this);
        currentIndex = 0;
        currentTarget = pathPoints.GetStationPoints[currentIndex].transform;
        unitRef.mover.MoveToDestinationStart(currentTarget.position);
        patrollingOn = true;
        movingToStandPoint = true;
        standingAtPoint = false;
    }

    public void CancelAction()
    {
        patrollingOn = false;
        standingAtPoint = false;
        standTimer = 0;
    }
    
}
