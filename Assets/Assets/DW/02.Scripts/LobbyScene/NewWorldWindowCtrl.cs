using UnityEngine;
using UnityEngine.UI;


public class NewWorldWindowCtrl : MonoBehaviour
{
    public GameObject createMode;
    public GameObject createModeSelect;
    public GameObject normalMode;
    public GameObject normalModeSelect;
    public GameObject quit;
    public GameObject quitSelect;

    public GameObject CreateRoomOff;
    public GameObject CreateRoomOn;
    public GameObject CreateRoomHover;


    public Text Noname;

    public InputField WorldName;


    public void ChangeBtn(GameObject buttonToActivate, GameObject buttonToDeactivate)
    {
        buttonToActivate.SetActive(true);
        buttonToDeactivate.SetActive(false);
    }

    public void OnCreateModeButtonClicked()
    {
        ChangeBtn(createModeSelect, createMode);
        ChangeBtn(normalMode,normalModeSelect);
    }

    public void OnNormalModeButtonClicked()
    {
        ChangeBtn(normalModeSelect, normalMode);
        ChangeBtn(createMode,createModeSelect);
    }

    public void OnQuitButtonClicked()
    {
        ChangeBtn(quitSelect, quit);
    }

    public void OnInputChange()
    {
        //if (!string.IsNullOrEmpty(WorldName.text))
        if(WorldName.text.Length > 0)
        {
            Noname.gameObject.SetActive(false);
            CreateRoomOff.SetActive(false);
            CreateRoomOn.SetActive(true);
        }
        else
        {
            Noname.gameObject.SetActive(true);
            CreateRoomOff.SetActive(true);
            CreateRoomOn.SetActive(false);
        }
    }

    public void OnCreateRoomButtonEnter()
    {
        if (CreateRoomOn.activeSelf)
        {
            ChangeBtn(CreateRoomHover, CreateRoomOn);
        }
    }

    public void OnCreateRoomButtonExit()
    {
        if (CreateRoomHover.activeSelf)
        {
            ChangeBtn(CreateRoomOn, CreateRoomHover);
        }
    }

    

}