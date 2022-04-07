using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoints : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject[] stationPoints;


    public GameObject[] GetStationPoints
    {
        get
        {
            return stationPoints;
        }
    }




}
