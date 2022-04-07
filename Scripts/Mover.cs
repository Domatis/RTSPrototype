using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class Mover : MonoBehaviour,IBaseAction
{
    
    public Action<bool> ReachedDestinationEvent;    //Event end with true if we actually reached, ends with false if we stuck and end the movement.

    [SerializeField] public NavMeshAgent nmagent = null;
    [SerializeField] private float stuckSec = 3f;
    [SerializeField] private Rigidbody rb;
    private Vector3 targetPosition;
    private RTSUnit mainunitref;

    [HideInInspector]
    public bool isMoving = false;
    [HideInInspector]
    public bool reachedToDestination = false;

    //public bool debugTest = false;

    //Variables for stuck check.
    private float stuckTimer = 0;
    private bool cantMove;
    private Vector3 lastPos;

    private void Awake()
    {
        mainunitref = GetComponent<RTSUnit>();
    }


    private void Start() 
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        
        mainunitref.UnitDeathAction += () =>
        {
            if(isMoving) CancelAction();
        };

        SelectableObject selectableObject = GetComponent<SelectableObject>();
        if(selectableObject)
        {
            selectableObject.OnObjectSelection += (state) =>
            {
                if(state) GameplayUIManager.instance.UpdateSelectedSpeedInfo(nmagent.speed.ToString());
            };
        }
    }

    public bool InterruptableWithDamageTaken()
    {
        return !isMoving;   //If unit is moving can not interreupt otherwise it's interruptable.
    }

    public bool InterruptableWithEnemySeen()
    {
        return !isMoving;
    }

    //These functions required for start movement action not the only movement.
    public void StartMovementAction(Vector3 destination)
    {
        mainunitref.GetUnitActionStateManager().SetNewAction(this);
        MoveToDestinationStart(destination);
    }

    public void StartMovementAction(Vector3 destination,float stopdistance)
    {
        mainunitref.GetUnitActionStateManager().SetNewAction(this);
        MoveToDestinationStart(destination,stopdistance);
    }

    //These actions required for when different actions needs and calls the movement ability without start new movement action.
    public void MoveToDestinationStart(Vector3 destination)
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;   //Unlock rotation Y
        lastPos = transform.position;
        // Move to olduğunda öncelikli olarak obstacle nesnesi disable edilecek ve 1 frame bekledikten sonra move olayı başlatılacak.       
        reachedToDestination = false;
        targetPosition = destination;
        nmagent.stoppingDistance = 0;   //Bunun normal durumlarda sıfır olmasından emin olmak için.
        isMoving = true;
        nmagent.isStopped = false;  
        nmagent.destination = targetPosition;
        mainunitref.animator.SetBool("Moving",true);
    }


    //TODO Performans için bu sınıfa daha sonra direkt pozisyona gidilme değil de Transform referansı verilerek anlık onu takip etme methodu yazılabilir.
    //Sürekli takip etme durumlarında bu method sürekli çağırılıyor ve bazı değişimler ve methodlar gereksiz çalışıyor.
    public void MoveToDestinationStart(Vector3 destination,float stopdistance)
    {
        // Move to olduğunda öncelikli olarak obstacle nesnesi disable edilecek ve 1 frame bekledikten sonra move olayı başlatılacak.       
        reachedToDestination = false;
        nmagent.stoppingDistance = stopdistance;
        targetPosition = destination;
        isMoving = true;
        nmagent.isStopped = false;  
        nmagent.destination = targetPosition;
        mainunitref.animator.SetBool("Moving",true);

    }

    private void Update() 
    {

        if(isMoving)
        {
            if(!nmagent.pathPending)    // Target setlendikten sonra, path hemen oluşturulmamış ise distance bilgisi yanlış geliyor o yüzden path oluşturulmasını bekliyoruz.
            {
                if((nmagent.remainingDistance - nmagent.stoppingDistance) <= 0f)   
                {
                     ReachedToDestination(true);
                }

                 //Add an option for when unit delta distance does not change that means it's stuck somewhere wait for a few seconds and stops the moving.

                    Vector3 distanceVec = transform.position - lastPos;
                    float deltamov = distanceVec.sqrMagnitude;
                    float sqrSpeed = (nmagent.speed * Time.deltaTime) * (nmagent.speed * Time.deltaTime);
                    if((deltamov/sqrSpeed) <= 0.1)
                        cantMove = true;
                    else cantMove = false;

                    if(cantMove)
                    {
                        stuckTimer += Time.deltaTime;
                        if(stuckTimer >= stuckSec)
                        {
                            ReachedToDestination(false); //End movement with an error to unit did not reach to actual destination.
                        } 
                    }
                    
                    else stuckTimer = 0;

                lastPos = transform.position;

            }
        }
    }

    public void ReachedToDestination(bool success)
    {
        stuckTimer = 0;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;    //Lock rotation.
        mainunitref.animator.SetBool("Moving",false);
        reachedToDestination = true;
        nmagent.isStopped = true;
        isMoving = false;
        ReachedDestinationEvent?.Invoke(success);
    }


    public void  CancelAction()
    {
        //Animasyon iptali.
        mainunitref.animator.SetBool("Moving",false);
        nmagent.isStopped = true;
        isMoving = false;
    }

}
