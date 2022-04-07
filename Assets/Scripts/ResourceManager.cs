using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    //TODO daha sonra bunlar uimanager sınıfı oluşturulup orda kontrol edilebilir.
    [SerializeField] private Text goldinfo;
    [SerializeField] private Text woodinfo;
    [SerializeField] private Text mealinfo;
    [Space]
    [SerializeField] private float startingGoldResource;
    [SerializeField] private float startingWoodResource;
    [SerializeField] private float startingMealResource;

    public enum ResourceTypes{gold,wood,meal}
    public static ResourceManager instance;


    private int currentworkeratGold,currentworkeratWood,currentworkeratMeal = 0;

    private float currentGoldResource,currentWoodResource,currentMealResource;

    private void Awake() 
    {
            instance = this;
    }

    private void Start() 
    {
        //TODO geçici başta belli resource kaynakları olsun diye.

        currentMealResource = startingMealResource;
        currentWoodResource = startingWoodResource;
        currentGoldResource = startingGoldResource;

        goldinfo.text = string.Format("{0} ({1})",(int)currentGoldResource,currentworkeratGold);
        woodinfo.text = string.Format("{0} ({1})",(int)currentWoodResource,currentworkeratWood);
        mealinfo.text = string.Format("{0} ({1})",(int)currentMealResource,currentworkeratMeal);
        
    }

    public bool IsGoldEnough(float value)
    {
        return currentGoldResource >= value ? true : false;
    }

    public bool IsWoodEnough(float value)
    {
        return currentWoodResource >= value ? true : false;
    }

    public bool IsMealEnough(float value)
    {
        return currentMealResource >= value ? true : false;
    }


    public void IncreaseCurrentWorkerCount(ResourceTypes type,int val)
    {

        switch(type)
        {
            case ResourceTypes.gold:
            currentworkeratGold+= val;
            goldinfo.text = string.Format("{0} ({1})",(int)currentGoldResource,currentworkeratGold);    //Update UI
            break;
            case ResourceTypes.wood:
            currentworkeratWood+= val;
            woodinfo.text = string.Format("{0} ({1})",(int)currentWoodResource,currentworkeratWood);
            break;
            case ResourceTypes.meal:
            currentworkeratMeal+= val;
            mealinfo.text = string.Format("{0} ({1})",(int)currentMealResource,currentworkeratMeal);
            break;
        }
    }

    public void DecreaseCurrentWorkerCount(ResourceTypes type,int val)
    {
        switch(type)
        {
            case ResourceTypes.gold:
            currentworkeratGold-= val;
            goldinfo.text = string.Format("{0} ({1})",(int)currentGoldResource,currentworkeratGold);    //Update UI
            break;
            case ResourceTypes.wood:
            currentworkeratWood-= val;
            woodinfo.text = string.Format("{0} ({1})",(int)currentWoodResource,currentworkeratWood);
            break;
            case ResourceTypes.meal:
            currentworkeratMeal-= val;
            mealinfo.text = string.Format("{0} ({1})",(int)currentMealResource,currentworkeratMeal);
            break;
        }
    }



    //TODO Resource ui bilgilerinin güncelleme olayını gameplayuimanager'ın yapmaısnı sağla.
    public void IncreaseResource(float amount,ResourceTypes type)
    {

        //Değer 0 olduğunda aşağıda ki kodlar ve çağırmalar boşa çalışmasın diye.

        if(amount <= 0) return;
        switch(type)
        {
            case ResourceTypes.gold:
            currentGoldResource += amount;
            //UI bilgisi güncelleme.
            goldinfo.text = string.Format("{0} ({1})",(int)currentGoldResource,currentworkeratGold);
            break;
            case ResourceTypes.meal:
            currentMealResource += amount;
            //UI bilgisi güncelleme.
            mealinfo.text = string.Format("{0} ({1})",(int)currentMealResource,currentworkeratMeal);
            break;
            case ResourceTypes.wood:
            currentWoodResource += amount;
            //UI bilgisi güncelleme.
            woodinfo.text = string.Format("{0} ({1})",(int)currentWoodResource,currentworkeratWood);
            break;

        }
    }

    public void DecreaseResource(float amount,ResourceTypes type)
    {
        switch(type)
        {
            //TODO Kaynakları küsüratlı gösterme ui içerisinde.

            //TODO 0'ın altına inme durumunu kontrol et.

            case ResourceTypes.gold:
            currentGoldResource -= amount;
            //UI bilgisi güncelleme.
            goldinfo.text = string.Format("{0} ({1})",(int)currentGoldResource,currentworkeratGold);
            break;
            case ResourceTypes.meal:
            currentMealResource -= amount;
            //UI bilgisi güncelleme.
            mealinfo.text = string.Format("{0} ({1})",(int)currentMealResource,currentworkeratMeal);
            break;
            case ResourceTypes.wood:
            currentWoodResource -= amount;
            //UI bilgisi güncelleme.
            woodinfo.text = string.Format("{0} ({1})",(int)currentWoodResource,currentworkeratWood);
            break;

        }
    }




}
