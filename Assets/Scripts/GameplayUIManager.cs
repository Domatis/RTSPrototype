using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameplayUIManager : MonoBehaviour
{
   
   public static GameplayUIManager instance;

   [SerializeField] private GameObject abilitiesUi;  
   [SerializeField] private OrdersSlot[] ordersSlots; //MAx 10
   [SerializeField] private AbilityUISlot[] abilitySlots;   //Max 9.
   [SerializeField] private ObjectSelectedUISlot[] selectionSlots; //Max 12. 
   [Header("Information elements")]
   [SerializeField] private GameObject tooltip;
   [SerializeField] private Text tooltipText;
   [SerializeField] private GameObject infoUi;
   [SerializeField] private GameObject singleSelectedUIPanel;
   [SerializeField] private GameObject resourceInfo;
   [SerializeField] private GameObject unitInfo;
   [SerializeField] private GameObject attackInfo;
   [Header("Resource Informations")]
    [SerializeField] private Text unityCapacityText;
   [Space]
   public Text selectionText;
   public Image selectionImage;
   public Text selectionHealthInfo;
   [Space]
   public Text resourceText;
   public Image resourceImage;
   public Text resourceQuantity;
   [Space]
   public Text moveSpeedText;
   public Text defText;
   [Space]
   public Text attackValueText;
   public Text rangeValueText;
   [Space]
   public Text objText1;
   public Text objText2;
   public Text objText3;
   [Space]
   [SerializeField] private GameObject mainMenuPanel;
   [SerializeField] private GameObject endGamePanel;
   [SerializeField] private GameObject winGameText;
   [SerializeField] private GameObject loseGameText;
   
   [HideInInspector] public bool menuOpen = false;

    private void Awake() 
    {
        instance = this;
    }

    private void Start() 
    {
        //infoUi.SetActive(false);
        tooltip.SetActive(false);
        mainMenuPanel.SetActive(false);
        endGamePanel.SetActive(false);
        winGameText.SetActive(false);
        loseGameText.SetActive(false);
        CloseAbilitiesUI();
        CloseUIPanel();

        for(int i = 0; i < ordersSlots.Length;i++)
        {
            ordersSlots[i].DeActiveateOrderSlot();
        }
    }



    public void ActivateOrderSlot(ref int index,Sprite icon)
    {
        for(int i = 0 ; i < ordersSlots.Length;i++)
        {
            if(ordersSlots[i].SlotAvailable)
            {   
                ordersSlots[i].ActivateOrder(icon);
                index = i;
                return;
            }
        }
    }

    public void UpdateOrderTimer(int index,float percentage)
    {
         ordersSlots[index].UpdateTimer(percentage);
    }
    

    public void DeActivateOrderSlot(int index)
    {
        ordersSlots[index].DeActiveateOrderSlot();
    }

    public void ActivateAndAddObjectSelection(InformationUIElements element)
    {
        infoUi.SetActive(true);

        //Check all elements for is that type exist.If it's already exist just update the count info 
        for(int i =0; i < selectionSlots.Length;i++)
        {
            if(selectionSlots[i].CurrentInfoElement == element)
            {
                selectionSlots[i].IncrementCountAndUpdateText();
                return;
            }
        }

        //Else activate new slot for this type of element.
        for(int i =0; i < selectionSlots.Length;i++)
        {
            if(selectionSlots[i].CurrentInfoElement == null)
            {
                selectionSlots[i].ActivateSlot(element);
                return;
            }
        }
    }

    public void DeActiveObjectFromSelection(InformationUIElements element)
    {
        //At first just find the same type of element and decrement the value of it.
        for(int i =0; i < selectionSlots.Length;i++)
        {
            if(selectionSlots[i].CurrentInfoElement == element)
            {
                selectionSlots[i].DecrementCountAndUpdateText();
                break;
            }
        }
        //End of the update check there is still objects selected or not if it's not close all ui.

        for(int i =0; i < selectionSlots.Length;i++)
        {
            //At least one slot is active just return.
            if(selectionSlots[i].CurrentInfoElement != null)
                return;
        }

        //Else close the all ui.
        CloseAbilitiesUI();
        CloseUIPanel();
    }

    public void DeActivateObjectAllSelections()
    {
        for(int i =0; i < selectionSlots.Length;i++)
        {
            selectionSlots[i].DeActivateSlot();
        }
    }

   //Açıldıktan sonra ilgili abilityler gönderilecek. 
   //TODO daha sonra abilitylerin barda hangi pozisyonda olmalarının belirlenmesi de seçenek olarak gelebilir.
   public void OpenAbilitiesUi(ObjectAbilities[] abilities,bool isSingleObjectSelected)
   {
       if(abilities != null)
       {
            if(isSingleObjectSelected || !abilitiesUi.activeInHierarchy)    
            {      
                for(int i = 0 ; i < abilities.Length;i++)
                {
                    abilitySlots[abilities[i].order-1].SetAbility(abilities[i].ability);
                }      
            }

            else
            {
                //Find the common abilities and just keep them.
                for(int i=0; i < abilities.Length;i++)
                {
                    if(abilities[i].ability != abilitySlots[abilities[i].order-1].CurrentAbility)
                    {
                        abilitySlots[abilities[i].order-1].RemoveAbility();
                    }
                }
            }
       }
           
        if(!abilitiesUi.activeInHierarchy)  abilitiesUi.SetActive(true);
    
   }

    public void CloseAbilitiesUI()
   {
        for(int i = 0 ; i < abilitySlots.Length; i++)
        {
            abilitySlots[i].RemoveAbility();
        }

       abilitiesUi.SetActive(false);
       tooltip.SetActive(false);
   }

   public void ShowToolTip(Vector3 pos,string content)
   {
       //Tooltip referansı active edilecek belirlenen yerde.
       tooltip.transform.position = pos;
       tooltipText.text = content;
       tooltip.SetActive(true);
   }

   public void CloseToolTip()
   {
       tooltip.SetActive(false);
   }

    public void OpenBaseUIPanel(string name,Sprite iconimage)
    {
        //Debug.Log("Uı opened");
        selectionText.text = name;
        selectionImage.sprite = iconimage;
        infoUi.SetActive(true);
        singleSelectedUIPanel.SetActive(true);
    }

    public void CloseUIPanel()
    {
       // Debug.Log("UI Panel Closed");
       DeActivateObjectAllSelections();
        resourceInfo.SetActive(false);
        unitInfo.SetActive(false);
        attackInfo.SetActive(false);
        infoUi.SetActive(false);
        singleSelectedUIPanel.SetActive(false);
    }

    public void ShowUnitUIElements()
    {
        //Unit kısmı aktif olacak.
        unitInfo.SetActive(true);
    }

    public void ShowResourceUIElements(string resourcename, Sprite resourceicon)
    {   
        //Resource kısmı aktif olacak.

        resourceText.text = resourcename;
        resourceImage.sprite = resourceicon;
        resourceInfo.SetActive(true);
    }


    //TODO Üniteler için attack olayını yazdkıtan sonra burayı değiştir.
    public void ShowAttackerUIElements(string attackVal,string rangeVal)
    {
        attackInfo.SetActive(true);
        attackValueText.text = attackVal;
        rangeValueText.text = rangeVal;
    }

    public void UpdateUnitCapacity(int filledCapacity,int maxCapacity)
    {
        string text = string.Format("{0}/{1}",filledCapacity,maxCapacity);
        unityCapacityText.text  = text;
    }

    public void UpdateSelectedHealthInfo(string txt)
    {
        selectionHealthInfo.text = txt;
    }

    public void UpdateSelectedResourceInfo(string txt)
    {
        resourceQuantity.text = txt;
    }

    public void UpdateSelectedSpeedInfo(string txt)
    {
        moveSpeedText.text = txt;
    }

    public void UpdateSelectedDefInfo(string txt)
    {
        defText.text = txt;
    }

    public void UpdateOrSetObjectiveText(string txt,int index)
    {
        switch(index)
        {
            case 0:
            objText1.text = txt;
            break;
            case 1:
            objText1.text = txt;
            break;
            case 2:
            objText1.text = txt;
            break;
        }
    }

    public void OpenMainMenuPanel()
    {
        menuOpen = true;
        mainMenuPanel.SetActive(true);
        Time.timeScale = 0;
    }

     public void CloseMainMenuButton()
    {
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1;
        menuOpen =false;
    }

    public void OpenGameWinPanel()
    {
        menuOpen = true;
        endGamePanel.SetActive(true);
        winGameText.SetActive(true);
        Time.timeScale = 0;
    }

    public void OpenGameLosePanel()
    {
        menuOpen = true;
        endGamePanel.SetActive(true);
        loseGameText.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartButton()
    {
        //Load the level again.
        SceneManager.LoadScene(1);  // 1 main gameplay scene.
    }

    public void MainMenuButton()
    {
        //Back to main menu scene.
        SceneManager.LoadScene(0);  // 0 menu scene.
    }   

   
}

[System.Serializable]
public class OrdersSlot
{
    public GameObject slotObject;
    public Image orderIcon;
    public Image timerImage;

    private bool slotAvailable = true;

    public bool SlotAvailable 
    {
        get {return slotAvailable;}
    }

    public void ActivateOrder(Sprite icon)
    {
        slotObject.SetActive(true);
        orderIcon.sprite = icon;
        timerImage.fillAmount = 1;
        slotAvailable = false;
    }

    public void DeActiveateOrderSlot()
    {
        slotObject.SetActive(false);
        slotAvailable = true;
    }

    public void UpdateTimer(float percentage)
    {
        timerImage.fillAmount = percentage;
    }
}


