using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public Item[] startItems;

    public int maxItems=10;
    public InventorySlot[] inventorySlots;

    public GameObject DraggableItemPrefab;

    int selectdeSlot=-1;

    public List<Item> itemList = new List<Item>();

    private void Awkae()
    {
        instance=this;
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


        //if(Input.GetKeyDown(KeyCode.E))
        //{
        //    AddItem(Random.Range(0,5));
        //}
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

    
}
