using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public static CameraController instance;




    
    [SerializeField] private Camera overlayCam;
    [Range(0,2)]
    [SerializeField] private float camMovSpeed = 0.5f;
    [SerializeField] private float minFovValue = 40;
    [SerializeField] private float maxFovValue = 70;
    [SerializeField] private float fovStepValue = 1;

    [SerializeField] private float xOffsetMinimapControl = 7f;
    [Space]
    [SerializeField] private GameObject topCamBorderPoint;
    [SerializeField] private GameObject lowerCamBorderPoint;
    [SerializeField] private GameObject leftCamBorderPoint;
    [SerializeField] private GameObject rightCamBorderPoint;

    private Camera cam;

    [HideInInspector] public bool minimapcontrolOn = false;

    private void Awake() 
    {
        instance = this;
        cam = GetComponent<Camera>();
    }

    private void Start() 
    {
       overlayCam.transform.position = transform.position;
       overlayCam.fieldOfView = cam.fieldOfView;
       CalculateAndSetMiniMapCursorPos();
    }

    public void MoveCamera(Vector3 movement)
    {
        if(minimapcontrolOn) return;
        //Clamp the cam position with the given borders.
        Vector3 newDeltaPos = movement * camMovSpeed;
        newDeltaPos.z = Mathf.Min(newDeltaPos.z,topCamBorderPoint.transform.position.z - transform.position.z);
        newDeltaPos.z = Mathf.Max(newDeltaPos.z,lowerCamBorderPoint.transform.position.z - transform.position.z);

        newDeltaPos.x = Mathf.Min(newDeltaPos.x,rightCamBorderPoint.transform.position.x - transform.position.x);
        newDeltaPos.x = Mathf.Max(newDeltaPos.x,leftCamBorderPoint.transform.position.x - transform.position.x);
    
        transform.position  += newDeltaPos;
        overlayCam.transform.position = transform.position;
        //Update minimapCursor pos.
        MinimapManager.instance.UpdateminiMapCursorPos( newDeltaPos);
    }


    public void ChangeCamFov(float val)
    {
        // Decrease fov value.
        if(val == 1)
        {
            //Check already at minimum or not.
            if(cam.fieldOfView <= minFovValue)
            {
                cam.fieldOfView = minFovValue;
                return;
            }

            //Else just decrease.

            cam.fieldOfView -= fovStepValue;
            if(cam.fieldOfView <= minFovValue) cam.fieldOfView = minFovValue;
        }
        // Increase fov value.
        else 
        {

            //Check for already at maximum or not.
            if(cam.fieldOfView >= maxFovValue)
            {
                cam.fieldOfView = maxFovValue;
                return;
            } 

            //Else  just increase.

            cam.fieldOfView += fovStepValue;
            if(cam.fieldOfView >= maxFovValue) cam.fieldOfView = maxFovValue;
        }

        overlayCam.fieldOfView = cam.fieldOfView;        
    }

    public void SetCamPosition(float xVal,float zVal)
    {
        Vector3 camPos = transform.position;

        float zOffset = 2 * Mathf.PI * transform.position.y*((90 - transform.eulerAngles.x)/360);

        //Change values.
        camPos.x = xVal+ xOffsetMinimapControl;
        camPos.z = zVal - zOffset;
        //Update cam pos.
        transform.position = camPos;
        //Update overlay cam.
        overlayCam.transform.position = transform.position;
    }

    private void CalculateAndSetMiniMapCursorPos()
    {
        Vector3 camPos = transform.position;

        float zOffset = 2 * Mathf.PI * transform.position.y*((90 - transform.eulerAngles.x)/360);

        //Change values.
        camPos.x = camPos.x - xOffsetMinimapControl;
        camPos.z = camPos.z + zOffset;
        //Update cam pos.
        MinimapManager.instance.SetMinimapCursor(camPos);
    }


}
