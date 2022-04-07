using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatingOrdersManager : MonoBehaviour
{

   //TODO Performans sorunu yaşanırsa, daha en başta belli sayıda order oluşturup, sürekli yeni yaratmak yerine var olanları değiştirebiliriz, her seferinde bellekte yeni sınıf oluşturup silme olayını önleriz.
   private List<ActiveOrders> orders = new List<ActiveOrders>();
   private bool uiselected = false;
   
   private void Start() 
   {

      if(TryGetComponent<SelectableObject>(out SelectableObject selectableObject))
      {
         selectableObject.OnObjectSelection += UISelectedMethod;
         selectableObject.OnObjectDeselection += UIDeselectedMethod;
      }
   }

   private void Update() 
   {

      //Just increase the order of the first in order.

      if(orders.Count >0)
      {
         orders[0].currentTime += Time.deltaTime;
         if(orders[0].currentTime >= orders[0].orderRef.GetOrderTime())
         {
            //At that point we need to create units and delete this order
            OrderComplete(0);
         }

         else if(uiselected)
         {
            GameplayUIManager.instance.UpdateOrderTimer(orders[0].uiOrderIndex,1-(orders[0].currentTime/orders[0].orderRef.GetOrderTime()));
         }
      }

   }

   public void UISelectedMethod(bool state)
   {
      //Send all order informations to ui.
      if(state)
      {
         uiselected = true;
         for(int i = 0 ; i < orders.Count;i++)
         {
            GameplayUIManager.instance.ActivateOrderSlot(ref orders[i].uiOrderIndex,orders[i].orderRef.GetOrderSprite());
         }
      }
   }

   public void UIDeselectedMethod()
   {
      if(uiselected)
      {
         uiselected = false;
         //Send information to ui.
          for(int i = 0 ; i < orders.Count;i++)
         {
            GameplayUIManager.instance.DeActivateOrderSlot(orders[i].uiOrderIndex);
         }
      }
   }

   public void OrderComplete(int index)
   {
      //Complete the selected order.
      orders[index].orderRef.CompleteOrder(gameObject);

      //send information to ui for deleting order.
      if(uiselected) GameplayUIManager.instance.DeActivateOrderSlot(orders[index].uiOrderIndex);
      //Delete the order.
      orders.RemoveAt(index);
   }



   public void StartCreatingOrders(IOrders order)
   {
      ActiveOrders neworder = new ActiveOrders(order);

      orders.Add(neworder);
      //After created for this info send information to ui.
      if(uiselected)
      {
         GameplayUIManager.instance.ActivateOrderSlot(ref neworder.uiOrderIndex,neworder.orderRef.GetOrderSprite());
      }      
   }
}

public class ActiveOrders
{
   public ActiveOrders(IOrders order)
   {
      orderRef = order;
   }

   public int uiOrderIndex;
   public float currentTime = 0;
   public IOrders orderRef;

}
