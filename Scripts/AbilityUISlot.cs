using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class AbilityUISlot : MonoBehaviour, IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    public static bool buttonsAvailable;

    [SerializeField] private Image slotImage;

    private Abilities currentability;
    private bool haveAbility = false;

    public Abilities CurrentAbility {get{return currentability;}}

    private void Awake() 
    {
        buttonsAvailable = true;
        slotImage.enabled = false;
    }

    public void SetAbility(Abilities ability)
    {
        currentability = ability;
        slotImage.sprite = ability.abilityimage;
        slotImage.enabled = true;
        haveAbility = true;
    }

    public void RemoveAbility()
    {
        currentability = null;
        slotImage.enabled = false;
        haveAbility = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if(haveAbility && buttonsAvailable)
        {
            currentability.AbilityAction();//Temp
        }
    }

    public void OnPointerEnter(PointerEventData eventdata)
    {
        //Eğer bu slotta ability var ise tooltip gösterilecek.
        if(haveAbility) GameplayUIManager.instance.ShowToolTip(transform.position,currentability.GetToolTipDescription());
    }


    

    public void OnPointerExit(PointerEventData eventdata)
    {
        //Tooltip gösterilmeyecek.
        if(haveAbility) GameplayUIManager.instance.CloseToolTip();
    }
}
