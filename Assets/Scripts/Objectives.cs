using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Objectives : MonoBehaviour
{
    protected ObjectiveManager.ObjectiveTypes objectiveType;

    public virtual ObjectiveManager.ObjectiveTypes GetObjectiveType()
    {
        return objectiveType;
    }

    public virtual void ObjectiveRegistration()
    {
        ObjectiveManager.instance.RegisterObjective(objectiveType);
    }
}
