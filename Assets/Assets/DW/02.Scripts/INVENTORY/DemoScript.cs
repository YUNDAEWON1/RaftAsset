using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScript : MonoBehaviour
{
   public InventoryManager inventoryManager;
   public Item[] itemsToPickup;

   public void PickupItem(int id)
   {
        bool result=inventoryManager.AddItem(itemsToPickup[id]);
        if(result==true)
        {
            Debug.Log("Item Added");
        }
        else
        {
         Debug.Log("ITEM NOT ADDED");
        }
   }

   public void GetSelectedItem()
   {
      Item receiveItem=inventoryManager.GetSelectedItem(false);
      if(receiveItem!=null){
         Debug.Log("Received item: "+receiveItem);
      }else {
         Debug.Log("No item received!");
      }
   }


   public void UseSelectedItem()
   {
      Item receiveItem=inventoryManager.GetSelectedItem(true);
      if(receiveItem!=null){
         Debug.Log("Use item: "+receiveItem);
      }else {
         Debug.Log("No item Used!");
      }
   }

}
