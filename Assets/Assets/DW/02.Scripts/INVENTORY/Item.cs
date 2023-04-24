using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


[CreateAssetMenu(menuName ="Scriptable object/Item")] 
public class Item : ScriptableObject
{
    
    public int ID => _id;
    public string Name => _name;
    public string Tooltip => _tooltip;


    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [Multiline]
    [SerializeField] private string _tooltip;

    [SerializeField] private GameObject itemPrefab;
    public ItemType type;
    public ActionType actionType;
    public Vector2Int range=new Vector2Int(5,4);

    public bool stackable=true;

    public Sprite image;

}

public enum ItemType{
    BuildingItem,
    Tool,
    Food,
    Material
}

public enum ActionType {
    Create,
    Delete
}