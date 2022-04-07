using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler
{
   
    public static MinimapManager instance;

    [SerializeField] private RectTransform mainRtransform;
    [SerializeField] private Terrain mainTerrain;
    [SerializeField] private RectTransform minimapCursor;

    private bool camMoving = false;
   


    private void Awake() 
    {
            instance = this;
    }

    public void OnPointerDown(PointerEventData eventdata)
    {
        
        float xValue;
        float yValue;
        
        RectTransform rtransform = eventdata.pointerCurrentRaycast.gameObject.GetComponent<RectTransform>();


        Vector2 posDelta = eventdata.position - (Vector2)rtransform.position;

        xValue = posDelta.x + (rtransform.rect.width/2);
        yValue = posDelta.y + (rtransform.rect.height/2);

        float camXPos = (xValue/rtransform.rect.width) * mainTerrain.terrainData.size.x;
        float camZPos = (yValue/rtransform.rect.height) * mainTerrain.terrainData.size.z;

        CameraController.instance.SetCamPosition(camXPos,camZPos);

        //Update minimapcursorPos
        minimapCursor.transform.position = eventdata.position;

        CameraController.instance.minimapcontrolOn = true;

        //At this point change the cam position to based on minimap clicked area.
       
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CameraController.instance.minimapcontrolOn = false;
    }

   
   public void OnDrag(PointerEventData eventdata)
   {
       float xValue;
       float yValue;
       if(eventdata == null) return;
       //TODO there is a bug here fix it.
       if(!eventdata.pointerCurrentRaycast.gameObject.TryGetComponent<RectTransform>(out RectTransform rtransform)) return;    

        Vector2 posDelta = eventdata.position - (Vector2)rtransform.position;

        xValue = posDelta.x + (rtransform.rect.width/2);
        yValue = posDelta.y + (rtransform.rect.height/2);

        float camXPos = (xValue/rtransform.rect.width) * mainTerrain.terrainData.size.x;
        float camZPos = (yValue/rtransform.rect.height) * mainTerrain.terrainData.size.z;

        CameraController.instance.SetCamPosition(camXPos,camZPos);
        //Update minimapcursorPos
        minimapCursor.transform.position = eventdata.position;
   }

   public void UpdateminiMapCursorPos(Vector3 deltaCamPos)
   {
       float xPercent = deltaCamPos.x / mainTerrain.terrainData.size.x;
       float yPercent = deltaCamPos.z/mainTerrain.terrainData.size.z;

        Vector3 newDeltaPos = new Vector3(xPercent * mainRtransform.rect.width,yPercent * mainRtransform.rect.height,0);
        minimapCursor.transform.position += newDeltaPos;
   }

    public void SetMinimapCursor(Vector3 camPos)
    {
       float xPercent = camPos.x / mainTerrain.terrainData.size.x;
       float yPercent = camPos.z/mainTerrain.terrainData.size.z;

       Vector3 newPos = new Vector3(xPercent * mainRtransform.rect.width,yPercent * mainRtransform.rect.height,0);

        Vector3 offsetMinimap = new Vector3(mainRtransform.transform.position.x -(mainRtransform.rect.width/2),mainRtransform.transform.position.y - (mainRtransform.rect.height/2),0);


       minimapCursor.transform.position  = newPos + offsetMinimap;
    }
    

}
