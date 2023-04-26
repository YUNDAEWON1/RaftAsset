using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
      public static CraftingManager instance;

    public List<CraftingRecipe> craftingRecipes = new List<CraftingRecipe>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CraftItem(Item item1, Item item2, out Item result)
    {
        result = null;

        // 조합 레시피들을 검사하면서 조합 가능한 아이템이 있는지 확인
        foreach (CraftingRecipe recipe in craftingRecipes)
        {
            bool containsItem1 = recipe.Materials.Exists(x => x.item == item1 && x.Amount <= CountItemsInInventory(item1.ID));
            bool containsItem2 = recipe.Materials.Exists(x => x.item == item2 && x.Amount <= CountItemsInInventory(item2.ID));

            if (containsItem1 && containsItem2)
            {
                // 조합 가능한 아이템이 있다면 결과 아이템 생성
                result = Instantiate(recipe.Results[0].item);

                // 인벤토리에서 재료 아이템 제거
                RemoveItemFromInventory(item1.ID, recipe.Materials.Find(x => x.item == item1).Amount);
                RemoveItemFromInventory(item2.ID, recipe.Materials.Find(x => x.item == item2).Amount);

                return true;
            }
        }

        // 조합 가능한 아이템이 없다면 false 반환
        return false;
    }

    private int CountItemsInInventory(int itemId)
    {
        int count = 0;

        foreach (InventorySlot slot in InventoryManager.instance.inventorySlots)
        {
            DraggableItem draggableItem = slot.GetComponentInChildren<DraggableItem>();

            if (draggableItem != null && draggableItem.item.ID == itemId)
            {
                count += draggableItem.count;
            }
        }

        return count;
    }

    private void RemoveItemFromInventory(int itemId, int amount)
    {
        int removedCount = 0;

        foreach (InventorySlot slot in InventoryManager.instance.inventorySlots)
        {
            DraggableItem draggableItem = slot.GetComponentInChildren<DraggableItem>();

            if (draggableItem != null && draggableItem.item.ID == itemId)
            {
                if (draggableItem.count - removedCount >= amount)
                {
                    draggableItem.count -= amount - removedCount;
                    removedCount = amount;
                    draggableItem.RefreshCount();
                }
                else
                {
                    removedCount += draggableItem.count;
                    Destroy(draggableItem.gameObject);
                }
            }

            if (removedCount >= amount)
            {
                break;
            }
        }
    }

}
