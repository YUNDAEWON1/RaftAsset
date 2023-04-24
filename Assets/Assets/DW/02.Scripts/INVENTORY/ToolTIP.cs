using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTIP : MonoBehaviour
{
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private Text itemName;
    [SerializeField]
    private Text itemInfo;

    public void Awake()
    {
        ResetInfo();
    }

    public void ResetInfo()
    {
        this.itemImage.gameObject.SetActive(false);
        this.itemName.text="";
        this.itemInfo.text="";
    }

    public void SetInfo(Sprite sprite,string _itemName,string _itemInfo)
    {
        this.itemImage.gameObject.SetActive(true);
        this.itemImage.sprite=sprite;
        this.itemName.text=_itemName;
        this.itemInfo.text=_itemInfo;
    }
}
