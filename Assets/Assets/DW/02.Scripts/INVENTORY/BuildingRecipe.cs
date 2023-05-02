using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;



public class BildingRecipe : ScriptableObject
{
   public List<ItemAmount> Materials;

   [HideInInspector]
   public bool canBuild=false;


   public bool CanBuild(InventoryManager inventory)
   {
      foreach (ItemAmount itemAmount in Materials)
      {
         if(!inventory.HasItem(itemAmount.item,itemAmount.Amount))
         {
            canBuild=false;
            return false;
         }
         else
         {
            canBuild=true;
         }
      }

      return true;
   }
}
