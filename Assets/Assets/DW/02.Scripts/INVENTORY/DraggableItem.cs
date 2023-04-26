using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler
{
    [Header("UI")]
    public Image image;
    public Text countText;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public int count=1;
    [HideInInspector] public Item item;
    


    private void Start()
    {
        InitialiseItem(item);
       
    }
    public void InitialiseItem(Item newItem)
    {
        item=newItem;
        image.sprite = newItem.image;
        RefreshCount();
    }

    public void RefreshCount()
    {
        countText.text=count.ToString();
        bool textActive=count>1;
        countText.gameObject.SetActive(textActive);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
       image.raycastTarget=false;
       parentAfterDrag=transform.parent;
       transform.SetParent(transform.root);
       //transform.SetAsLastSibling();

       
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        transform.position=Input.mousePosition;
    }

    private bool IsOverUI()
    => EventSystem.current.IsPointerOverGameObject();
    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);

      if (!IsOverUI())
    {
        count--;
        if(count<=0){
         Destroy(gameObject);
        } else{
        RefreshCount();
                    }
        
    }

  
    }
}
