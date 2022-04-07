using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{

    public enum ObjectiveTypes {buildDestruction}
    public static ObjectiveManager instance;

    private List<ObjectiveRegister> registeredObjectives = new List<ObjectiveRegister>();

    
    private void Awake() 
    {
        instance = this;
    }

    public void RegisterObjective(ObjectiveTypes type)
    {
        //Check all registered objectives. if it's not exist create new one.
        bool objectiveTypeFound = false;
        for(int i=0; i< registeredObjectives.Count;i++)
        {
            if(registeredObjectives[i].type == type)
            {
                 //If it's already exist just increment the total count of registered objective.
                registeredObjectives[i].totalCount ++;
                objectiveTypeFound = true;
                UpdateUI(i);    //Uptade UI.
                break;
            }
        }

        //Create new one.
        if(!objectiveTypeFound)
        {
            ObjectiveRegister objregister = new ObjectiveRegister(type);
            registeredObjectives.Add(objregister);
            //Uptade UI.
            UpdateUI(registeredObjectives.Count-1);
        }
    }

    public void ObjectiveCompleted(ObjectiveTypes type)
    {
        
        //Find the objective.
        for(int i =0; i < registeredObjectives.Count;i++)
        {
            if(registeredObjectives[i].type == type)
            {
                registeredObjectives[i].currentCompletedObjective++;
                UpdateUI(i);    //Uptade UI.
                break;
            }
        }

        //Check all objectives completed or not.
        bool state = true;
        for(int i =0; i < registeredObjectives.Count;i++)
        {
            if(!registeredObjectives[i].IsObjectiveCompleted())
            {
                state = false;
                break;
            }
        }

        //All objectives completed.
        if(state)
        {
            GameplayManager.instance.GameWin();
        }
    }

    public void UpdateUI(int index)
    {
        string str = "";
        ObjectiveRegister selected = registeredObjectives[index];

        if(selected.type == ObjectiveTypes.buildDestruction)
        {
            str += "Destroy Enemy Spawner Buildings = " +selected.currentCompletedObjective+"/"+selected.totalCount;
            GameplayUIManager.instance.UpdateOrSetObjectiveText(str,index);
        }

        //Could be other objective types.
    }
}

public class ObjectiveRegister
{
    public ObjectiveManager.ObjectiveTypes type;
    public int totalCount;
    public int currentCompletedObjective;

    public ObjectiveRegister(ObjectiveManager.ObjectiveTypes t)
    {
        type = t;
        totalCount = 1;
        currentCompletedObjective = 0;
    }

    public bool IsObjectiveCompleted()
    {
        if(currentCompletedObjective >= totalCount) return true;

        else return false;
    }
}


