using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class InventoryManager : MonoBehaviour
{


    [SerializeField] public List<CraftingRecipe> craftingRecipes=new List<CraftingRecipe>();
    [SerializeField] public List<BuildingRecipe> buildingRecipes=new List<BuildingRecipe>();
    public static InventoryManager instance;
    public Item[] startItems;

    public int maxItems=64;
    public InventorySlot[] inventorySlots;

    public GameObject DraggableItemPrefab;

    int selectdeSlot=-1;

    public List<Item> itemList = new List<Item>();

    public CraftingUI craftUI;
    public BuildingUI buildingUI;

    /////////////////////DATA SAVE&LOAD////////////////////////////

   string path;
   string filename;

    private void Awake()
    {
        instance=this;
        craftUI=GetComponent<CraftingUI>();
        path= Application.dataPath+"/Resources/";
        filename = "save_" + PhotonNetwork.player.NickName + ".json";


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

    private void Start()
    {
        LoadInventory();
    }

    private void Update()
    {
       if(Input.inputString!=null){
        bool isNumber=int.TryParse(Input.inputString,out int number);
        if(isNumber&&number>0&&number<10){
            ChangeSelectedSLot(number-1);}
        }


    CheckCraft(craftingRecipes[0]);    //컵제작
    CheckCraft1(craftingRecipes[1]);   //정수기제작
    CheckCraft2(craftingRecipes[2]);   //그릴 제작
    CheckCraft3(craftingRecipes[3]);   //작은 작물 밭
    CheckCraft4(craftingRecipes[4]);   //해머
    CheckCraft5(craftingRecipes[5]);   //훅
    CheckCraft6(craftingRecipes[6]);   //나무 창
    CheckCraft7(craftingRecipes[8]); //도끼
    CheckCraft8(craftingRecipes[9]); //침대
    CheckCraft9(craftingRecipes[10]); //폭죽
    

    //////////////////////빌드 관련//////////////////////

    CheckBuildFound(buildingRecipes[0]);
    CheckBuildPill(buildingRecipes[1]);
    CheckBuildStairs(buildingRecipes[2]);
    CheckBuildFloor(buildingRecipes[3]);
    CheckBuildRoof(buildingRecipes[4]);

        
        
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

    public void UseSelectedItem()
    {
        Item receiveItem = GetSelectedItem(true);
        if (receiveItem != null)
        {
            Debug.Log("Use item: " + receiveItem);
        }
        else
        {
            Debug.Log("No item Used!");
        }
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
 


public void Craft(CraftingRecipe reicpe)
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

private void CheckCraft4(CraftingRecipe recipe)
{
    if(recipe.CanCraft(this))
    {
        craftUI.OnbtnOK4();
    
    }
    else
    {
        craftUI.OnbtnNO4();
     
    }
}

private void CheckCraft5(CraftingRecipe recipe)
{
    if(recipe.CanCraft(this))
    {
        craftUI.OnbtnOK5();
    
    }
    else
    {
        craftUI.OnbtnNO5();
     
    }
}

private void CheckCraft6(CraftingRecipe recipe)
{
    if(recipe.CanCraft(this))
    {
        craftUI.OnbtnOK6();
    
    }
    else
    {
        craftUI.OnbtnNO6();
     
    }
}

private void CheckCraft7(CraftingRecipe recipe)
{
    if(recipe.CanCraft(this))
    {
        craftUI.OnbtnOK7();
    
    }
    else
    {
        craftUI.OnbtnNO7();
     
    }
}

private void CheckCraft8(CraftingRecipe recipe)
{
    if(recipe.CanCraft(this))
    {
        craftUI.OnbtnOK8();
    
    }
    else
    {
        craftUI.OnbtnNO8();
     
    }
}


private void CheckCraft9(CraftingRecipe recipe)
{
    if(recipe.CanCraft(this))
    {
        craftUI.OnbtnOK9();
    
    }
    else
    {
        craftUI.OnbtnNO9();
     
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
public void btnHameer()
{
    Craft(craftingRecipes[4]);
}
public void btnHook()
{
    Craft(craftingRecipes[5]);
}

public void btnSpear()
{
    Craft(craftingRecipes[6]);
}

public void btnAxe()
{
    Craft(craftingRecipes[8]);
}
public void vtnBed()
{
    Craft(craftingRecipes[9]);
}

public void btnRocket()
{
    Craft(craftingRecipes[10]);
}
////////////////////Building 관련 함수들///////////////////

public void Build(BuildingRecipe _recipe)
{
    if(_recipe.CanBuild(this))
    {
        _recipe.Build(this);
    }
    else
    {
        Debug.Log("재료부족");
    }
}

public void CheckBuildFound(BuildingRecipe _recipe)
{
    if(_recipe.CanBuild(this))
    {
        buildingUI.OnFoundOK();
    }
    else
    {
        buildingUI.OnFoundNO();
    }
}

public void CheckBuildFloor(BuildingRecipe _recipe)
{
    if(_recipe.CanBuild(this))
    {
        buildingUI.OnFloorOK();
    }
    else
    {
       buildingUI.OnFloorNO();
    }
}

public void CheckBuildPill(BuildingRecipe _recipe)
{
    if(_recipe.CanBuild(this))
    {
        buildingUI.OnPillOK();
    }
    else
    {
        buildingUI.OnPillNO();
    }
}

public void CheckBuildRoof(BuildingRecipe _recipe)
{
    if(_recipe.CanBuild(this))
    {
        buildingUI.OnRoofOK();
    }
    else
    {
        buildingUI.OnRoofNO();
    }

}

public void CheckBuildStairs(BuildingRecipe _recipe)
{
    if(_recipe.CanBuild(this))
    {
        buildingUI.OnStairsOK();
    }
    else
    {
        buildingUI.OnStairsNO();
    }

}


//////////////////////////////JSON///////////////////////////////

public void SaveInventory()
{
    // 인벤토리 정보를 저장할 InventoryData 객체 생성
    InventoryData inventoryData = new InventoryData();
    inventoryData.items = new List<InventoryItem>();
    inventoryData.PlayerName=PhotonNetwork.player.NickName;
    Debug.Log(inventoryData.PlayerName);

    // 인벤토리 슬롯 배열을 순회하며 아이템 정보를 수집하여 InventoryData에 저장
    for(int i = 0; i < inventorySlots.Length; i++)
    {
        InventorySlot slot = inventorySlots[i];
        DraggableItem itemInSlot = slot.GetComponentInChildren<DraggableItem>();

        if(itemInSlot != null)
        {
            InventoryItem inventoryItem = new InventoryItem();
            inventoryItem.id = itemInSlot.item.ID;
            inventoryItem.count = itemInSlot.count;
            inventoryData.items.Add(inventoryItem);
            Debug.Log(inventoryItem);
        }
    }

    // InventoryData를 JSON 문자열로 변환하여 파일에 저장
    string json = JsonUtility.ToJson(inventoryData);
    File.WriteAllText(path+filename, json);

    Debug.Log(path);
}

public void LoadInventory()
{
   
    // 저장된 인벤토리 데이터 파일을 불러옵니다.
    if (File.Exists(path + filename))
    {
        string json = File.ReadAllText(path + filename);
        InventoryData inventoryData = JsonUtility.FromJson<InventoryData>(json);

        // 저장된 플레이어 이름이 로컬 플레이어 이름과 일치할 경우에만 인벤토리를 로드합니다.
        if (inventoryData.PlayerName == PhotonNetwork.playerName)
        {
            
            // 인벤토리 슬롯을 초기화합니다.
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                InventorySlot slot = inventorySlots[i];

                // 해당 슬롯에 아이템이 배치되어 있으면 삭제
                if (slot.transform.childCount > 0)
                {
                    Destroy(slot.transform.GetChild(0).gameObject);
                }
            }

            // InventoryData에 저장된 아이템 정보를 인벤토리 슬롯에 추가합니다.
            for (int i = 0; i < inventoryData.items.Count; i++)
            {
                InventoryItem item = inventoryData.items[i];

                // itemList에 저장된 아이템들 중 같은 ID를 가진 아이템을 찾아 추가합니다.
                for (int j = 0; j < itemList.Count; j++)
                {
                    Item currentItem = itemList[j];

                    if (currentItem.ID == item.id)
                    {
                        // 해당 슬롯에 아이템이 배치되어 있지 않은 경우, 아이템을 추가합니다.
                        // 배치되어 있는 경우, 개수만 업데이트합니다.
                        InventorySlot slot = inventorySlots[j];
                        if (slot.transform.childCount == 0)
                        {   
                            AddItem(currentItem, item.count);
                            Debug.Log(currentItem.ID);
                        }
                        else
                        {
                            DraggableItem draggableItem = slot.transform.GetChild(0).GetComponent<DraggableItem>();
                            draggableItem.count = item.count;
                            draggableItem.RefreshCount();
                        }
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.Log("Player name does not match inventory data.");
        }
    }
    else
    {
        Debug.Log("Inventory file not found!");
         ChangeSelectedSLot(0);
        foreach(var item in startItems){
            AddItem(item);
        }
    }
}




}





// 인벤토리 아이템 정보를 담는 클래스
[System.Serializable]
public class InventoryItem
{
    [SerializeField]
    public int id;
    [SerializeField]
    public int count;

    public int ID
    {
        get { return id; }
        set { id = value; }
    }

    public int Count
    {
        get { return count; }
        set { count = value; }
    }
}



// 인벤토리 정보를 담는 클래스
[Serializable]
public class InventoryData
{
    public List<InventoryItem> items;
    public string PlayerName;
}
