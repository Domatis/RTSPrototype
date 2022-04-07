using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WallConstruction", menuName = "Abilities/WallConstructionAbility", order = 2)]
public class WallConstructionAbility : Abilities
{
    
    public GameObject edgeWallObject;
    public GameObject mainWallObject;
    public float edgeDistance;
    public float mainWallDistance;
    public AbilityRequirements[] edgeWallRequirement;
    public AbilityRequirements[] mainWallRequirement;

    public override void AbilityAction()
    {

        // Minimum requirement for wall construction is just build one edge piece.
        for(int i = 0; i < edgeWallRequirement.Length; i++)
        {
            if(!edgeWallRequirement[i].IsRequirementMet())
            {
                Debug.Log("Build Yapımı için yeterli kaynak yok");
                return;
            }
        }

        //If requirements are provided.We build the build placing object.
        GameObject edgePlacingObject = Instantiate(edgeWallObject,Vector3.zero,Quaternion.identity);  //Position is not important at first.
        BuildPlaceObject edgeWallPlaceRef = edgePlacingObject.GetComponent<BuildPlaceObject>();
        BuildPlaceObject mainWallPlaceRef = mainWallObject.GetComponent<BuildPlaceObject>();
        
        PlayerRTSController.instance.StartWallPlacement(edgeWallPlaceRef,mainWallPlaceRef,edgeWallRequirement,mainWallRequirement,edgeDistance,mainWallDistance);
    }

    public override string GetToolTipDescription()
    {
        string result = "";
        result += string.Format("{0}\n",base.GetToolTipDescription());
        
        for(int i = 0 ; i < edgeWallRequirement.Length; i++)
        {
            result += edgeWallRequirement[i].GetRequirementToolTipDescription() +" ";
        }
        return result;
    }

}
