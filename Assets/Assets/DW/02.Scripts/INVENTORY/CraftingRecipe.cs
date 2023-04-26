using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


[Serializable]
public struct ItemAmount
{
    public Item item;
    [Range(1,999)]
    public int Amount;
}

[CreateAssetMenu]
public class CraftingRecipe : ScriptableObject
{
    public List<ItemAmount> Materials;
    public List<ItemAmount> Results;


    [HideInInspector]
    public bool canCraft=false;


public bool CanCraft(InventoryManager inventory)
{
    foreach (ItemAmount itemAmount in Materials)
    {
        if (!inventory.HasItem(itemAmount.item, itemAmount.Amount))
        {
            canCraft=false;
            return false;
        }
        else
        {
            canCraft=true;
        }
    }
    
    return true;
}

public void Craft(InventoryManager inventory)
{
    // 재료 아이템 제거
    foreach (ItemAmount itemAmount in Materials)
    {
        inventory.RemoveItem(itemAmount.item, itemAmount.Amount);
    }

    // 결과 아이템 추가
    foreach (ItemAmount itemAmount in Results)
    {
        inventory.AddItem(itemAmount.item, itemAmount.Amount);
    }
}


}