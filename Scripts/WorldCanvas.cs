using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//World canvaslar objeler parent olduklarından, obje rotasyonunda etkilenmemeleri için sürekli kendi orijinal rotasyonlarını korumalarını sağlanması için script.
public class WorldCanvas : MonoBehaviour
{
    
    public Quaternion xx;

    private RectTransform rectTransform;
    private Quaternion baseQuaternion;

    private void Awake() 
    {
        rectTransform = GetComponent<RectTransform>();
        baseQuaternion = rectTransform.rotation;
    }

    private void Start() 
    {
        
    }

    private void Update() 
    {
        //gameObject.transform.LookAt(Camera.main.transform);
        rectTransform.rotation = baseQuaternion;
    }


}
