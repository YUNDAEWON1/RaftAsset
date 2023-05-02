using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscControl : MonoBehaviour
{
    public GameObject btnSetting;
    public GameObject btnReturn;

    public GameObject EscUI;

    void Awkae()
    {
        GameObject SetWindow=GameObject.Find("pnlSettingWindow");
    }


    void Update()
{
    if(Input.GetKeyDown(KeyCode.Escape))
    {
        EscUI.SetActive(!EscUI.activeSelf);
    }
}

public void OnClickSetting()
    {
        // SomeObject 게임 오브젝트를 찾아서 비활성화합니다.
        GameObject SetWindow = GameObject.Find("pnlSettingWindow");
        if (SetWindow != null)
        {
            SetWindow.SetActive(false);
        }
    }

}
