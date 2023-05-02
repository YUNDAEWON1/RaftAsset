using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class InventorySlot : MonoBehaviour,IDropHandler,IPointerEnterHandler,IPointerExitHandler
{

    public Image image;
    public Sprite selectedSprite,notSelectedSprite;



    public void Awake()
    {
        Deselect();
    }

    public void Select(){
        image.sprite=selectedSprite;
    }
    public void Deselect()
    {
        image.sprite=notSelectedSprite;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount==0){
        GameObject dropped=eventData.pointerDrag;
        DraggableItem draggableItem=dropped.GetComponent<DraggableItem>();
        draggableItem.parentAfterDrag=transform;
        }
    }

   

   public void OnPointerEnter(PointerEventData eventData)
{
    DraggableItem draggableItem = GetComponentInChildren<DraggableItem>();
    if (draggableItem != null)
    {
        Item item = draggableItem.item;
      

        ToolTIP toolTip = FindObjectOfType<ToolTIP>();
        toolTip.SetInfo(item.image, item.Name, item.Tooltip);
       
    }
}

public void OnPointerExit(PointerEventData eventData)
{
   
    ToolTIP toolTip = FindObjectOfType<ToolTIP>();
    toolTip.ResetInfo();
    
}


}
