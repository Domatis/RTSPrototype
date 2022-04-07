using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;



//Box collider trigger olmalı.
//Rigidbody kinematic olmalı.
[RequireComponent(typeof(BoxCollider),typeof(Rigidbody))]
public class BuildPlaceObject : MonoBehaviour
{
  
    public Action<GameObject> buildPrefabPlaced;       //This gameobject referenceto placed building.
    //Material referansı olacak. "Available" değeri değiştirilecek.
    private bool placementAvailable = true;
    [SerializeField] private GameObject buildPrefab;
    [SerializeField] private Material buildPlacementMaterial;


    private AbilityRequirements[] requirements;

    public AbilityRequirements[] Requirements {set{requirements = value;}}

    private bool firstRequest = true;       // Başlangıçta true olacak gelen istek ilk olacağı için.//TODO remove later.
    public bool PlacementAvailable 
    {
        get
        {   
            // Bina yerleşimi için butona tıklandığında anda yerleşim için kontrol ediliyor, ilk isteğin ignorelanması için kontrol.
            //Check requirements.
            bool requirementEnough = true;
            if(requirements == null)
            {
                Debug.Log("Requirement null = " + gameObject.name);
                return placementAvailable;
            }
            for(int i=0; i< requirements.Length;i++)
            {
                if(!requirements[i].IsRequirementMet())
                {
                    requirementEnough = false;
                    break;
                }
            }

            if(requirementEnough && placementAvailable) return true;
            else return false;
        }
    }

    public void PlaceBuildPrefab(Vector3 pos,Quaternion rotation,out GameObject placedBuild)
    {   
            GameObject build = Instantiate(buildPrefab,pos,buildPrefab.transform.rotation);
            build.transform.rotation = rotation;
            //spend requirements.
            for(int i = 0 ; i < requirements.Length; i++)
            {
                requirements[i].SpendRequirement();
            }
            placedBuild = build;
            buildPrefabPlaced?.Invoke(build);
            Destroy(gameObject);
    }

    private void Awake() 
    {
        buildPlacementMaterial.SetInt("_Available",1);
    }

    private void Start() 
    {
        placementAvailable = true;
    }

    private void OnTriggerStay(Collider other) 
    {
        //Aktif olduğu durumda sürekli kontrol edilecek başka objelerle collide edip edilmediği.
        placementAvailable = false;
        buildPlacementMaterial.SetInt("_Available",0);
    }

    private void OnTriggerExit(Collider other) 
    {
        placementAvailable = true;
        buildPlacementMaterial.SetInt("_Available",1);
    }

}
