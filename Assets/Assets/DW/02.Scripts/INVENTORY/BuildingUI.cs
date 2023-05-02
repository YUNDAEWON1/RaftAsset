using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUI : MonoBehaviour
{
     [HideInInspector]
     public CraftingRecipe recipe;
     [HideInInspector]
    public InventoryManager inventoryManager;

    public GameObject FoundUI;
    public GameObject FloorUI;
    public GameObject PillUI;
    public GameObject RoofUI;
    public GameObject StairsUI;

    public GameObject found_OK;
    public GameObject found_NO;
    public GameObject floor_OK;
    public GameObject floor_NO;
    public GameObject pill_OK;
    public GameObject pill_NO;
    public GameObject roof_OK;
    public GameObject roof_NO;
    public GameObject stairs_OK;
    public GameObject stairs_NO;



    public void Update()
    {
        //switch (플레이어가 해머를 들고있을때)
        //case e를 한번누르면 FoundUI 활성화 / 한번 더누르면 FloorUI 활성화 / q를 누르면 다시 FoundUI 활성화FloorUI 비활성화 이런느낌.....
    }




///////////////////떗목 토대//////////////
    public void OnFoundOK()
    {
        found_OK.SetActive(true);
        found_NO.SetActive(false);
    }
    public void OnFoundNO()
    {
        found_OK.SetActive(false);
        found_NO.SetActive(true);
    }
/////////////////나무 바닥///////////////////

    public void OnFloorOK()
    {
        floor_OK.SetActive(true);
        floor_NO.SetActive(false);
    }
    public void OnFloorNO()
    {
        floor_OK.SetActive(false);
        floor_NO.SetActive(true);
    }
///////////////////기둥//////////////////////

    public void OnPillOK()
    {
        pill_OK.SetActive(true);
        pill_NO.SetActive(false);
    }
    public void OnPillNO()
    {
        pill_OK.SetActive(false);
        pill_NO.SetActive(true);
    }
////////////////////지붕//////////////////////////

    public void OnRoofOK()
    {
        roof_OK.SetActive(true);
        roof_NO.SetActive(false);
    }
    public void OnRoofNO()
    {
        roof_OK.SetActive(false);
        roof_NO.SetActive(true);
    }
//////////////////계단/////////////////////////////

    public void OnStairsOK()
    {
        stairs_OK.SetActive(true);
        stairs_NO.SetActive(false);
    }
    public void OnStairsNO()
    {
        stairs_OK.SetActive(false);
        stairs_NO.SetActive(true);
    }
}
