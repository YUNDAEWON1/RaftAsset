using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{

    [HideInInspector]
     public CraftingRecipe recipe;
     [HideInInspector]
    public InventoryManager inventoryManager;

    public GameObject cup_OK;
    public GameObject cup_NO;
    public GameObject filter_OK;
    public GameObject filter_NO;
    public GameObject Grill_OK;
    public GameObject Grill_NO;
    public GameObject CropPlot_OK;
    public GameObject CropPlot_NO;
    public GameObject hammer_OK;
    public GameObject hammer_NO;
    public GameObject hook_OK;
    public GameObject hook_NO;

    public GameObject spear_OK;
    public GameObject spear_NO;

    public void OnbtnOK()
    {
        cup_OK.SetActive(true);
        cup_NO.SetActive(false);
    }
    public void OnbtnNO()
    {
        cup_OK.SetActive(false);
        cup_NO.SetActive(true);
    }

    public void OnbtnOK1()
    {
        filter_OK.SetActive(true);
        filter_NO.SetActive(false);
    }
    public void OnbtnNO1()
    {
        filter_OK.SetActive(false);
        filter_NO.SetActive(true);
    }
    public void OnbtnOK2()
    {
        Grill_OK.SetActive(true);
        Grill_NO.SetActive(false);
    }
    public void OnbtnNO2()
    {
        Grill_OK.SetActive(false);
        Grill_NO.SetActive(true);
    }
    public void OnbtnOK3()
    {
        CropPlot_OK.SetActive(true);
        CropPlot_NO.SetActive(false);
    }
    public void OnbtnNO3()
    {
        CropPlot_OK.SetActive(false);
        CropPlot_NO.SetActive(true);
    }

     public void OnbtnOK4()
    {
        hammer_OK.SetActive(true);
        hammer_NO.SetActive(false);
    }
    public void OnbtnNO4()
    {
        hammer_OK.SetActive(false);
        hammer_NO.SetActive(true);
    }

      public void OnbtnOK5()
    {
        hook_OK.SetActive(true);
        hook_NO.SetActive(false);
    }
    public void OnbtnNO5()
    {
        hook_OK.SetActive(false);
        hook_NO.SetActive(true);
    }

       public void OnbtnOK6()
    {
        spear_OK.SetActive(true);
        spear_NO.SetActive(false);
    }
    public void OnbtnNO6()
    {
        spear_OK.SetActive(false);
        spear_NO.SetActive(true);
    }


}
