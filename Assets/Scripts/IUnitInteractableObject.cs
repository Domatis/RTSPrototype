using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitInteractableObject
{
    bool SendCursorInformation(RTSUnit unit);
    bool StartUnitsInteractions(RTSUnit unit);
    bool AllowPlayerUnitInteraction();
    int  GetInteractionPriority();
}
