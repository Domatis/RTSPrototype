using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class Health : MonoBehaviour
{
    
    public Action HealthAtMaximumValue;
    public Action HealthAtMinimumValue;
    
    [SerializeField] private float maximumHealth = 1000f;
    public GameObject healthui;
    [SerializeField] private Image hpimage;
    private float currentHealth;       

    [HideInInspector]
    public int testVal;

    //Bu birim seçildiğinde bu bool işaretlenir.
     private bool selectedHealth = false;
     public bool UISelectedHealth{
         get{return selectedHealth;}
         set
            {
                selectedHealth = value;
                if(value)   GameplayUIManager.instance.UpdateSelectedHealthInfo(((int)currentHealth).ToString() + "/" + maximumHealth.ToString());    
            }
     }


    public float CurrentHealth{get {return currentHealth;}}


    private void Awake() 
    {
        currentHealth = maximumHealth;
        hpimage.fillAmount = currentHealth / maximumHealth;
        healthui.SetActive(false);
    }


    private void Start() 
    {
        SelectableObject selectableObject = GetComponent<SelectableObject>();
        if(selectableObject)
        {
            selectableObject.OnObjectSelection += (state) =>
            {
                healthui.SetActive(true);
                if(state)
                {
                    UISelectedHealth = true;
                }
                
            };
            selectableObject.OnObjectDeselection += () => 
            {
                healthui.SetActive(false);
                UISelectedHealth = false;
            };
        }
         

    }


    public void UpdateHealth(float hpvalue)
    {
        currentHealth = hpvalue;
        // Update unit UI
        hpimage.fillAmount = currentHealth / maximumHealth;
        //Eğer bu birim seçilmişse ana ui'daki health bilgisi güncellenecek.
        if(selectedHealth)  GameplayUIManager.instance.UpdateSelectedHealthInfo(((int)currentHealth).ToString() + "/" + maximumHealth.ToString());
         
    }

    public void MaximizeHealth()
    {
        currentHealth = maximumHealth;
        // Update unit UI
        hpimage.fillAmount = currentHealth / maximumHealth;
        //Eğer bu birim seçilmişse ana ui'daki health bilgisi güncellenecek.
        if(selectedHealth)  GameplayUIManager.instance.UpdateSelectedHealthInfo(((int)currentHealth).ToString() + "/" + maximumHealth.ToString());
    }

    public void IncreaseHealth(float value)
    {
        currentHealth += value;
        if(currentHealth >= maximumHealth)
        {
            currentHealth = maximumHealth;
            HealthAtMaximumValue?.Invoke();
        }
        // Update UI
        hpimage.fillAmount = currentHealth / maximumHealth;
        if(selectedHealth)  GameplayUIManager.instance.UpdateSelectedHealthInfo(((int)currentHealth).ToString() + "/" + maximumHealth.ToString());
    }

    public void DecreaseHealth(float value)
    {
        currentHealth -= value;
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            testVal = 0;
            HealthAtMinimumValue?.Invoke();
        }
        // Update UI
        hpimage.fillAmount = currentHealth / maximumHealth;
        if(selectedHealth)  GameplayUIManager.instance.UpdateSelectedHealthInfo(((int)currentHealth).ToString() + "/" + maximumHealth.ToString());
    }

    public bool IsHealthFull()
    {
        if(currentHealth >= maximumHealth)
            return true;
        else 
            return false;
    }
}
