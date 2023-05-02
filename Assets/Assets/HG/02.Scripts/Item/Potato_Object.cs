using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Potato_Object : MonoBehaviour
{
    public GameManager gm;
    public InventoryManager ivenmanager;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        ivenmanager = FindObjectOfType<InventoryManager>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            gm.hungry += 0.5f;            
            ivenmanager.UseSelectedItem();
        }
    }
}
