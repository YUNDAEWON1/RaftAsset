using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinWorldWindowCtrl : MonoBehaviour
{
    public GameObject btnJoinWorldOff;
    public GameObject btnJoinWorldOn;
    public GameObject btnJoinWorldHover;
  

    public void ChangeBtn(GameObject buttonToActivate, GameObject buttonToDeactivate)
    {
        buttonToActivate.SetActive(true);
        buttonToDeactivate.SetActive(false);
    }

    

    public void OnJoinWorldButtonEnter()
    {
        if (btnJoinWorldOn.activeSelf)
        {
            ChangeBtn(btnJoinWorldHover, btnJoinWorldOn);
        }
    }

    public void OnJoinWorldButtonExit()
    {
        if (btnJoinWorldHover.activeSelf)
        {
            ChangeBtn(btnJoinWorldOn, btnJoinWorldHover);
        }
    }

   
}
