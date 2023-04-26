using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{


    [SerializeField] private List<CraftingRecipe> craftingRecipes=new List<CraftingRecipe>();
    public static InventoryManager instance;
    public Item[] startItems;

    public int maxItems=10;
    public InventorySlot[] inventorySlots;

    public GameObject DraggableItemPrefab;

    int selectdeSlot=-1;

    public List<Item> itemList = new List<Item>();

    public CraftingUI craftUI;

    private void Awake()
    {
        instance=this;
        craftUI=GetComponent<CraftingUI>();
    }

    private void Start()
    {
        ChangeSelectedSLot(0);
        foreach(var item in startItems){
            AddItem(item);
        }

        // Resources 폴더에서 아이템 에셋 로드
        Item[] items = Resources.LoadAll<Item>("ItemAssets");

        // itemList에 로드된 아이템 추가
        foreach (Item item in items)
        {
            itemList.Add(item);
        }

        // itemList 원소 수 출력
        Debug.Log("itemList count: " + itemList.Count);

        // itemList 원소 정보 출력
        foreach (Item item in itemList)
        {
            Debug.Log("item ID: " + item.ID);
        }
    }

    private void Update()
    {
       if(Input.inputString!=null){
        bool isNumber=int.TryParse(Input.inputString,out int number);
        if(isNumber&&number>0&&number<10){
            ChangeSelectedSLot(number-1);}
        }


    CheckCraft(craftingRecipes[0]);
    CheckCraft1(craftingRecipes[1]);
    CheckCraft2(craftingRecipes[2]);
    CheckCraft3(craftingRecipes[3]);
        
        
    }

    void ChangeSelectedSLot(int newValue)
    {   
        if(selectdeSlot>=0){
        inventorySlots[selectdeSlot].Deselect();
        }

        inventorySlots[newValue].Select();
        selectdeSlot=newValue;
    }


   public void SpawnNewItem(Item item,InventorySlot slot)
    {
        GameObject newItemGo=Instantiate(DraggableItemPrefab,slot.transform);
        DraggableItem InventoryItem=newItemGo.GetComponent<DraggableItem>();
        InventoryItem.InitialiseItem(item);
    }
    public bool AddItem(Item item)
    {

        for(int i=0;i<inventorySlots.Length;i++)
        {
            InventorySlot slot=inventorySlots[i];
            DraggableItem itemInSlot=slot.GetComponentInChildren<DraggableItem>();
            if(itemInSlot!=null&&
                itemInSlot.item==item&&
                itemInSlot.count<maxItems&&
                itemInSlot.item.stackable==true)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
        }

        for(int i=0;i<inventorySlots.Length;i++)
        {
            InventorySlot slot=inventorySlots[i];
            DraggableItem itemInSlot=slot.GetComponentInChildren<DraggableItem>();
            if(itemInSlot==null)
            {
                SpawnNewItem(item,slot);
                return true;
            }
        }
        return false;
}


 public bool AddItem(int itemId)
    {
        // Item 리스트에서 ID와 일치하는 아이템 찾기
        Item item = itemList.Find(x => x.ID == itemId);

        if (item != null)
        {
            for(int i=0;i<inventorySlots.Length;i++)
            {
                InventorySlot slot=inventorySlots[i];
                DraggableItem itemInSlot=slot.GetComponentInChildren<DraggableItem>();
                if(itemInSlot!=null&&
                    itemInSlot.item==item&&
                    itemInSlot.count<maxItems&&
                    itemInSlot.item.stackable==true)
                {
                    itemInSlot.count++;
                    itemInSlot.RefreshCount();
                    return true;
                }
            }

            for(int i=0;i<inventorySlots.Length;i++)
            {
                InventorySlot slot=inventorySlots[i];
                DraggableItem itemInSlot=slot.GetComponentInChildren<DraggableItem>();
                if(itemInSlot==null)
                {
                    SpawnNewItem(item,slot);
                    return true;
                }
            }
        }

        return false;
    }



    public bool RemoveItem(int itemId)
{
    // Item 리스트에서 ID와 일치하는 아이템 찾기
    Item item = itemList.Find(x => x.ID == itemId);

    if (item != null)
    {
        for(int i=0;i<inventorySlots.Length;i++)
        {
            InventorySlot slot = inventorySlots[i];
            DraggableItem itemInSlot = slot.GetComponentInChildren<DraggableItem>();
            if(itemInSlot != null && itemInSlot.item == item)
            {
                if (itemInSlot.count > 1) // 아이템이 여러 개일 경우
                {
                    itemInSlot.count--;
                    itemInSlot.RefreshCount();
                    return true;
                }
                else // 아이템이 하나일 경우
                {
                    Destroy(itemInSlot.gameObject);
                    return true;
                }
            }
        }
    }

    return false;
}
    public Item GetSelectedItem(bool use)
    {
        InventorySlot slot=inventorySlots[selectdeSlot];
        DraggableItem itemInSlot=slot.GetComponentInChildren<DraggableItem>();
            if(itemInSlot!=null)
            {
                Item item=itemInSlot.item;
                if(use==true){
                    itemInSlot.count--;
                    if(itemInSlot.count<=0){
                        Destroy(itemInSlot.gameObject);
                    } else{
                        itemInSlot.RefreshCount();
                    }
                }
                return item;
            }
     return null;
    }

//////////////////Crafting 관련 함수들 ////////////////////////////////////////////////////

    // 아이템을 갖고 있는지 확인하는 함수
   public bool HasItem(Item item, int amount)
{
    int foundAmount = 0;

    for(int i = 0; i < inventorySlots.Length; i++)
    {
        InventorySlot slot = inventorySlots[i];
        DraggableItem itemInSlot = slot.GetComponentInChildren<DraggableItem>();

        if(itemInSlot != null && itemInSlot.item == item)
        {
            foundAmount += itemInSlot.count;
            if(foundAmount >= amount)
            {
                return true;
            }
        }
    }

    return false;
}

    // 아이템을 제거하는 함수
public void RemoveItem(Item item, int amount)
{
    for(int i = 0; i < inventorySlots.Length; i++)
    {
        InventorySlot slot = inventorySlots[i];
        DraggableItem itemInSlot = slot.GetComponentInChildren<DraggableItem>();

        if(itemInSlot != null && itemInSlot.item == item)
        {
            if(itemInSlot.count > amount)
            {
                itemInSlot.count -= amount;
                itemInSlot.RefreshCount();
                return;
            }
            else
            {
                Destroy(itemInSlot.gameObject);
                return;
            }
        }
    }
}
     // 아이템을 추가하는 함수
public bool AddItem(Item item, int amount)
{
    // stackable 변수가 false이면 무조건 새로운 아이템을 생성합니다.
    if (!item.stackable)
    {
        // 빈 슬롯을 검색하여 아이템을 추가합니다.
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            DraggableItem itemInSlot = slot.GetComponentInChildren<DraggableItem>();

            if (itemInSlot == null)
            {
                // 빈 슬롯을 찾았을 경우 새로운 아이템을 추가합니다.
                GameObject newItemObject = Instantiate(DraggableItemPrefab, slot.transform);
                DraggableItem newItem = newItemObject.GetComponent<DraggableItem>();
                newItem.InitialiseItem(item);
                newItem.count = amount;
                newItem.RefreshCount();
                

                return true;
            }
        }

        // 빈 슬롯도 없을 경우 아이템을 추가할 수 없습니다.
        return false;
    }

    // 아이템 슬롯을 검색하여 아이템이 이미 있는지 확인합니다.
    for (int i = 0; i < inventorySlots.Length; i++)
    {
        InventorySlot slot = inventorySlots[i];
        DraggableItem itemInSlot = slot.GetComponentInChildren<DraggableItem>();

        if (itemInSlot != null && itemInSlot.item == item)
        {
            // stackable 변수가 true이고 슬롯에 이미 해당 아이템이 있을 경우 수량을 늘립니다.
            itemInSlot.count += amount;
            itemInSlot.RefreshCount();

            return true;
        }
    }

    // stackable 변수가 true이고 슬롯에 해당 아이템이 없을 경우 빈 슬롯을 검색하여 아이템을 추가합니다.
    for (int i = 0; i < inventorySlots.Length; i++)
    {
        InventorySlot slot = inventorySlots[i];
        DraggableItem itemInSlot = slot.GetComponentInChildren<DraggableItem>();

        if (itemInSlot == null)
        {
            // 빈 슬롯을 찾았을 경우 새로운 아이템을 추가합니다.
            GameObject newItemObject = Instantiate(DraggableItemPrefab, slot.transform);
            DraggableItem newItem = newItemObject.GetComponent<DraggableItem>();
            newItem.InitialiseItem(item);
            newItem.count = amount;
            newItem.RefreshCount();
            return true;
        }
    }

    // 빈 슬롯도 없을 경우 아이템을 추가할 수 없습니다.
    return false;
}
 


private void Craft(CraftingRecipe reicpe)
    {
        if(reicpe.CanCraft(this))
        {
            Debug.Log("조합가능");
            reicpe.Craft(this);
        }
        else
        {
            Debug.Log("재료 없음");
        }
    }

private void CheckCraft(CraftingRecipe recipe)
{
    if(recipe.CanCraft(this))
    {
        craftUI.OnbtnOK();
    
    }
    else
    {
        craftUI.OnbtnNO();
     
    }
}

private void CheckCraft1(CraftingRecipe recipe)
{
    if(recipe.CanCraft(this))
    {
        craftUI.OnbtnOK1();
    
    }
    else
    {
        craftUI.OnbtnNO1();
     
    }
}

private void CheckCraft2(CraftingRecipe recipe)
{
    if(recipe.CanCraft(this))
    {
        craftUI.OnbtnOK2();
    
    }
    else
    {
        craftUI.OnbtnNO2();
    }
}

private void CheckCraft3(CraftingRecipe recipe)
{
    if(recipe.CanCraft(this))
    {
        craftUI.OnbtnOK3();
    
    }
    else
    {
        craftUI.OnbtnNO3();
     
    }
}

public void btnCup()
{
    Craft(craftingRecipes[0]);
}
public void btnFilter()
{
    Craft(craftingRecipes[1]);
}

public void btnGrill()
{
    Craft(craftingRecipes[2]);
}
public void btnCropPlot()
{
    Craft(craftingRecipes[3]);
}

}
