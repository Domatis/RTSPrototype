using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBuildings : MonoBehaviour
{  

    public Action buildDesctructionAction;

    private void Start() 
    {
        if(TryGetComponent<Health>(out Health health))
        {
            health.HealthAtMinimumValue += BuildDestruction;    
        }

        if(TryGetComponent<SelectableObject>(out SelectableObject selectableObject))
        {
            selectableObject.OnObjectSelection += (state) =>
            {
                GameplayUIManager.instance.OpenAbilitiesUi(null,false);
            };
        }
    }

    public void BuildDestruction()
    {
        buildDesctructionAction?.Invoke();
        Destroy(gameObject);
    }
}
