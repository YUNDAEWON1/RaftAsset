using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscControl : MonoBehaviour
{
    public GameObject btnSetting;
    public GameObject btnReturn;

    public GameObject EscUI;

    public Transform setWindow;

    public GameObject Quickslot;

    InventoryUI invenUI;

    private bool activeEsc=false;

    void Awake()
    {
        EscUI.SetActive(activeEsc);
        invenUI=GetComponent<InventoryUI>();
    }


private void Update() {
    if(Input.GetKeyDown(KeyCode.Escape))
    {
    Quickslot.SetActive(false);
       activeEsc=!activeEsc;
       EscUI.SetActive(activeEsc);

       if(activeEsc)
       {
        invenUI.aim.enabled=false;
        Cursor.lockState=CursorLockMode.None;
        Cursor.visible=true;
       }
       else
       {
        invenUI.aim.enabled=true;
        Cursor.lockState=CursorLockMode.Locked;
        Cursor.visible=false;
        Quickslot.SetActive(true);
       }
    }
}



public void OnClickSetting()
{
    
    GameObject.Find("SoundCanvas").transform.Find("pnlSettingWindow").gameObject.SetActive(true);
}

public void OnClickReturn()
{
    EscUI.SetActive(false);
    Quickslot.SetActive(true);
    invenUI.aim.enabled=true;
        Cursor.lockState=CursorLockMode.Locked;
        Cursor.visible=false;
}
}
