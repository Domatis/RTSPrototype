using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelectedUISlot : MonoBehaviour
{
    public Image icon;
    public Text countText;

    private InformationUIElements currentElement;
    private int count;

    public InformationUIElements CurrentInfoElement { get {return currentElement;}}


    private void Start() 
    {
        DeActivateSlot();
    }


    public void ActivateSlot(InformationUIElements element)
    {
        gameObject.SetActive(true);
        currentElement = element;
        icon.sprite = element.selectionIcon;
        IncrementCountAndUpdateText();
    }

    public void DeActivateSlot()
    {
        gameObject.SetActive(false);
        currentElement = null;
        count = 0;
    }

    public void IncrementCountAndUpdateText()
    {
        count ++;
        countText.text = count.ToString();
    }

    public void DecrementCountAndUpdateText()
    {
        count --;
        if(count == 0)
        {
            DeActivateSlot();
            return;
        } 
        countText.text = count.ToString();
    }

}
