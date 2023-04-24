using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyBtnCtrl : MonoBehaviour
{   

    //각 버튼 연결 레퍼런스
    public GameObject btnNewWorld;
    public GameObject btnLoadWorld;
    public GameObject btnJoinWorld;
    public GameObject btnCreatePlayer;
    public GameObject btnSetting;
    public GameObject btnDeveloper;
    public GameObject btnQuit;
    public GameObject Discord;


    //버튼 눌렀을 때 열리는 윈도우 연결 레퍼런스
    public GameObject NewWorldWindow;
    public GameObject LoadWorldWindow;
    public GameObject JoinWorldWindow;
    public GameObject CreatePlayerWindow;
    public GameObject SettingWindow;
    public GameObject DeveloperWindow;
    

    //현재 열려있는 윈도우 정보, 닫기버튼이 아닌 다른 버튼 눌렀을때 꺼지게 하기 위함
    private GameObject currentWindow;


    //재미로 넣어본 디스코드 URL 연결
    public string discordUrl = "https://discord.gg/eNuWvEw56V"; // Discord 서버 URL

    public void OnButtonClick(GameObject window)
    { 
        if (currentWindow != null)
        {
            CloseUI(currentWindow);
        }
        OpenUI(window);
        currentWindow = window;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void CloseUI(GameObject window)
    {
        window.SetActive(false);
    }

    public void OpenUI(GameObject window)
    {
        window.SetActive(true);
    }

    private void Start()
    {
        // 각 버튼에 클릭 이벤트 추가
        btnNewWorld.GetComponent<Button>().onClick.AddListener(delegate { OnButtonClick(NewWorldWindow); });
        btnLoadWorld.GetComponent<Button>().onClick.AddListener(delegate { OnButtonClick(LoadWorldWindow); });
        btnJoinWorld.GetComponent<Button>().onClick.AddListener(delegate { OnButtonClick(JoinWorldWindow); });
        btnCreatePlayer.GetComponent<Button>().onClick.AddListener(delegate { OnButtonClick(CreatePlayerWindow); });
        btnSetting.GetComponent<Button>().onClick.AddListener(delegate { OnButtonClick(SettingWindow); });
        btnDeveloper.GetComponent<Button>().onClick.AddListener(delegate { OnButtonClick(DeveloperWindow); });
        btnQuit.GetComponent<Button>().onClick.AddListener(ExitGame);
    }


     

    public void OpenDiscordUrl()
    {
        Application.OpenURL(discordUrl);
    }
}
