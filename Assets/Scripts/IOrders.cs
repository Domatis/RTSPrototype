using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOrders
{
    void CompleteOrder(GameObject parentOrderObject);
    Sprite GetOrderSprite();
    float GetOrderTime();
}
