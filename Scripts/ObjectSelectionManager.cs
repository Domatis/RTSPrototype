using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelectionManager : MonoBehaviour
{
    
    public static ObjectSelectionManager instance;

    [SerializeField] private SelectableObjectType[] selectableObjectTypes;

    private void Awake() 
    {
        instance = this;
    }

    public int GetPriorityOfObject(SelectableObjectTypes type)
    {
        
        for(int i = 0 ; i < selectableObjectTypes.Length; i++)
        {
            if(selectableObjectTypes[i].objectType == type)
            {
                return selectableObjectTypes[i].priority;
            }
        }

        //Eğer var olan typelar içinde yok ise error verilecek 
        Debug.LogError("Selection Managerda İlgili Türün Örneği Bilgisi Yok");
        return -1;

    }

    public bool IsMultipleSelectionOn(SelectableObjectTypes type)
    {
        for(int i = 0 ; i < selectableObjectTypes.Length; i++)
        {
            if(selectableObjectTypes[i].objectType == type)
            {
                return selectableObjectTypes[i].multipleSelection;
            }
        }

        //Eğer var olan typelar içinde yok ise error verilecek 
        Debug.LogError("Selection Managerda İlgili Türün Örneği Bilgisi Yok");
        return false;
    }



}
