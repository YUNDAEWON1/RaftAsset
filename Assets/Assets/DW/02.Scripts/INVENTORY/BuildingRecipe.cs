using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


[CreateAssetMenu]
public class BuildingRecipe : ScriptableObject
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

   public void Build(InventoryManager inventory)
   {
      foreach(ItemAmount itemAmount in Materials)
      {
         inventory.RemoveItem(itemAmount.item,itemAmount.Amount);
      }
   }
}
