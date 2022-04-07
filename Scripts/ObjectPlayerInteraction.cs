using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlayerInteraction : MonoBehaviour
{
    
    private List<IUnitInteractableObject> interactions = new List<IUnitInteractableObject>();


    public bool StartObjectInteraction(RTSUnit unit)
    {
        if(interactions.Count <= 0) return false;   //If interaction list is empty just return false.

        int currentPriority = interactions[0].GetInteractionPriority();  
        int selectedIndex = 0;
        for(int i = 0; i < interactions.Count;i++)
        {
            if(interactions[i].GetInteractionPriority() < currentPriority)  //Lowest values are high priority.
            {
                selectedIndex = i;
                currentPriority = interactions[i].GetInteractionPriority();
            }
        }

        return interactions[selectedIndex].StartUnitsInteractions(unit);
    }

    public bool SendObjectCursorInformation(RTSUnit unit)
    {
        if(interactions.Count <= 0) return false;   //If interaction list is empty just return false.
        //Finding the highest(numerically lowest one) priority.
        int currentPriority = interactions[0].GetInteractionPriority();  
        int selectedIndex = 0;
        for(int i = 0; i < interactions.Count;i++)
        {
            if(interactions[i].GetInteractionPriority() < currentPriority)  //Lowest values are high priority.
            {
                selectedIndex = i;
                currentPriority = interactions[i].GetInteractionPriority();
            }
        }

        return interactions[selectedIndex].SendCursorInformation(unit);
    }



    public void AddInteraction(IUnitInteractableObject interaction)
    {
        interactions.Add(interaction);
    }

    public void RemoveInteraction(IUnitInteractableObject interaction)
    {
        interactions.Remove(interaction);
    }

}
