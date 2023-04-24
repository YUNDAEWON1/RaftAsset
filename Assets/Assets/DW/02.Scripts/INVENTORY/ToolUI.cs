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

    public GameObject foodwaterUI;
    public GameObject otherUI;
    public GameObject utilitiesUI;
    public GameObject weaponsUI;
    public GameObject equipmentUI;
    public GameObject materialsUI;
    public GameObject navigationUI;
    public GameObject decorationUI;

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
    }

    public void ToggleUI(GameObject ui)
    {
        if (currentUI != null && currentUI != ui) // 현재 열려있는 UI가 있으면 닫아줌
        {
            currentUI.SetActive(false);
        }

        ui.SetActive(!ui.activeSelf);
        currentUI = ui;
    }
}

