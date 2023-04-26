using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ToolUI : MonoBehaviour
{
    public GameObject btn_foodwater;
    public GameObject btn_Ohter;
    public GameObject btn_Utilities;
    public GameObject btn_Weapons;
    public GameObject btn_Equopment;
    public GameObject btn_Materials;
    public GameObject btn_Navigation;
    public GameObject btn_Decoration;

    public GameObject btn_Cup;

    public GameObject foodwaterUI;
    public GameObject otherUI;
    public GameObject utilitiesUI;
    public GameObject weaponsUI;
    public GameObject equipmentUI;
    public GameObject materialsUI;
    public GameObject navigationUI;
    public GameObject decorationUI;

    public GameObject craft_cup;

    private GameObject currentUI;

    void Start()
    {
        btn_foodwater.GetComponent<Button>().onClick.AddListener(() => ToggleUI(foodwaterUI));
        btn_Ohter.GetComponent<Button>().onClick.AddListener(() => ToggleUI(otherUI));
        btn_Utilities.GetComponent<Button>().onClick.AddListener(() => ToggleUI(utilitiesUI));
        btn_Weapons.GetComponent<Button>().onClick.AddListener(() => ToggleUI(weaponsUI));
        btn_Equopment.GetComponent<Button>().onClick.AddListener(() => ToggleUI(equipmentUI));
        btn_Materials.GetComponent<Button>().onClick.AddListener(() => ToggleUI(materialsUI));
        btn_Navigation.GetComponent<Button>().onClick.AddListener(() => ToggleUI(navigationUI));
        btn_Decoration.GetComponent<Button>().onClick.AddListener(() => ToggleUI(decorationUI));
        btn_Cup.GetComponent<Button>().onClick.AddListener(ToggleCupUI);
    }

   public void ToggleUI(GameObject ui)
{
    if (currentUI != null && currentUI != ui) // 현재 열려있는 UI가 있으면 닫아줌
    {
        currentUI.SetActive(false);
    }

    if (currentUI != ui) // 현재 UI와 새로운 UI가 같은 경우는 무시함
    {
        ui.SetActive(!ui.activeSelf);
        currentUI = ui;
    }

    Debug.Log(ui.name + " is " + (ui.activeSelf ? "opened" : "closed"));
}

 public void ToggleCupUI()
    {
        // foodwaterUI가 켜져 있는지 확인
        bool isFoodwaterActive = foodwaterUI.activeSelf;

        // craft_cup을 foodwaterUI의 자식으로 만듭니다.
        craft_cup.transform.SetParent(foodwaterUI.transform);

        // craft_cup UI를 활성화하고 foodwaterUI를 다시 활성화합니다.
        craft_cup.SetActive(true);
        foodwaterUI.SetActive(isFoodwaterActive);
    }
}

