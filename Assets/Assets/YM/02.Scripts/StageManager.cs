using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// (UI 버전에서 사용)
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    // 포톤 추가////////////////////////////////////////////////
    //접속된 플레이어 수를 표시할 Text UI 항목 연결 레퍼런스 (Text 컴포넌트 연결 레퍼런스)
    //public Text txtConnect;

    //접속 로그를 표시할 Text UI 항목 연결 레퍼런스 선언
    public Text txtLogMsg;

    //채팅을 입력할 인풋필드
    public InputField chatField;

    public GameObject chatUI; // 채팅UI 게임 오브젝트
    public float chatTimeout = 5f; // 채팅 입력이 없을 때 UI가 자동으로 꺼지는 시간

    private bool isChatting = false; // 채팅 중인지 여부를 저장하는 변수

    private float chatTimer; // 채팅 입력 타이머

    //RPC 호출을 위한 PhotonView 연결 레퍼런스
    PhotonView pv;

    //플레어의 생성 위치 저장 레퍼런스
    private Transform[] playerPos;
    ////////////////////////////////////////////////////////////

    // HG
    private Transform objectPos;
    public ObjetPool op;

    // 게임끝
    private bool gameEnd;


    void Awake()
    {
        //포톤추가
        //PhotonView 컴포넌트를 레퍼런스에 할당
        pv = GetComponent<PhotonView>();

        playerPos = GameObject.Find("PlayerSpawnPoint").GetComponentsInChildren<Transform>();

        // HG
        objectPos = GameObject.Find("Spawn_Object").transform;
        op = FindObjectOfType<ObjetPool>();

        //룸에 입장한 후 기존 접속자 정보를 출력
        GetConnectPlayerCount();
        ////////////////////////////////////////////////////////////////////////////////////

    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        // 포톤 추가/////////////////////////////////////////////////////////////
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("scNetTest"));

        //플레이어를 생성하는 함수 호출
        StartCoroutine(this.CreatePlayer());

        if(PhotonNetwork.isMasterClient)
        {
            // HG            
            op.CreateQueue();
            StartCoroutine(this.CreateObject());
        }
        

        //포톤 클라우드로부터 네트워크 메세지 수신을 다시 연결 이 씨이이이이이이이발
        PhotonNetwork.isMessageQueueRunning = true;

        // 로그 메시지에 출력할 문자열 생성
        string msg = "\n\t<color=#ffffff>[" + PhotonNetwork.player.NickName + "] 님이 입장하셨습니다.</color>";

        //RPC 함수 호출
        //(CF) 플레이어 접속,종료 시 호출되는 콜백 함수에서 메시지를 표시하는 루틴을 추가하여도
        //상관없지만, 뒤늦게 룸에 조인한 플레이어의 로그 창에 로그 메시지를 띄우기 위함.(로그 메시지를 Buffered RPC 처리)
        pv.RPC("LogMsg", PhotonTargets.AllBuffered, msg);

        //룸에 있는 네트워크 객체간의 통신이 완료될때까지 잠시 대기
        //yield return new WaitForSeconds(1f);

        //모든 플레이어의 스코어 UI에 점수를 표시하는 함수를 호출
        StartCoroutine(SetConnectPlayerScore());
        /////////////////////////////////////////////////////////////////////////////////////////////////

        // 채팅 UI를 비활성화
        chatUI.SetActive(false);
        chatTimer = 0f;

        if(PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.InstantiateSceneObject("MonsterShark", new Vector3(-155f, 7f, 85), Quaternion.identity, 0, null);

            PhotonNetwork.InstantiateSceneObject("Foundation", new Vector3(-150f, 10f, 85), Quaternion.identity, 0, null);
            PhotonNetwork.InstantiateSceneObject("Foundation", new Vector3(-151.5f, 10f, 85), Quaternion.identity, 0, null);
            PhotonNetwork.InstantiateSceneObject("Foundation", new Vector3(-150f, 10f, 86.5f), Quaternion.identity, 0, null);
            PhotonNetwork.InstantiateSceneObject("Foundation", new Vector3(-151.5f, 10f, 86.5f), Quaternion.identity, 0, null);
        }

        yield return null;
    }

    public void OnMasterClientSwitched()
    {
        if (PhotonNetwork.isMasterClient)
        {
            // HG
            StartCoroutine(this.CreateObject());
        }
    }

    //모든 네트워크 플레이어의 스코어UI에 점수를 표시하는 함수를 호출
    IEnumerator SetConnectPlayerScore()
    {
 
        // 현재 입장한 룸에 접속한 모든 네트워크 플레이어의 정보를 저장 
        PhotonPlayer[] players = PhotonNetwork.playerList;

        // 전체 네트워크 플레이어의 정보를 출력
        foreach (PhotonPlayer _player in players)
        {
            Debug.Log("[" + _player.ID + "]" + _player.NickName + " " + _player.GetScore() + " Kill");
        }

        //모든 Player 프리팹을 배열에 저장
        GameObject[] net_Player = GameObject.FindGameObjectsWithTag("Player");

        //Debug.Log(players.Length);

        // 동일 룸에 입장해있는 모든 네트워크 플레이어의 케릭터에 HUD 스코어 표시
        foreach (GameObject _player in net_Player)
        {

            //각 베이스(플레이어)별 스코어를 조회
            int currKillCount = _player.GetComponent<PhotonView>().owner.GetScore();
        }

        yield return null;
    }

    //플레이어생성함수
    IEnumerator CreatePlayer()
    {
        // 지금은 테스트를 위하여 플레이어 스폰 포인트가 2개이다 따라서 차후 접속 인원수에 맞게 스폰 포인트와
        // 총 접속인원의 수를 제한


        //현재 입장한 룸 정보를 받아옴(레퍼런스 연결)
        Room currRoom = PhotonNetwork.room;

        // 테스트를 위한 object 배열
        object[] ex = new object[3];
        ex[0] = 3;
        ex[1] = 4;
        ex[2] = 5;

        GameObject player = PhotonNetwork.Instantiate("MainPlayer", playerPos[currRoom.PlayerCount].position, playerPos[currRoom.PlayerCount].rotation, 0, ex);



        yield return null;
    }

    // HG
    IEnumerator CreateObject()
    {
        while(true)
        {
            for (int i = 0; i < 7; i++)
            {
                GameObject plank = ObjetPool.instance.GetQueue();
                plank.transform.position = objectPos.position + new Vector3(Random.Range(-50f, 50f), transform.position.y, Random.Range(-50f, 50f));
                plank.transform.rotation = Quaternion.identity;
            }

            //GameObject plank = PhotonNetwork.InstantiateSceneObject("Plank", objectPos.position, objectPos.rotation, 0, null);
            //Vector3 newPosition_1 = objectPos.position + new Vector3(0f, 0f, 50f);
            //GameObject plastic = PhotonNetwork.InstantiateSceneObject("Plastic", newPosition_1, objectPos.rotation, 0, null);
            //Vector3 newPosition_2 = objectPos.position + new Vector3(0f, -2f, 100f);
            //GameObject leaf = PhotonNetwork.InstantiateSceneObject("Leaf", newPosition_2, objectPos.rotation, 0, null);
            yield return new WaitForSeconds(20f);
        }
    }

    //포톤추가
    //룸 접속자 정보를 조회하는함수
    void GetConnectPlayerCount()
    {
        //현재 입장한 룸 정보를 받아옴
        Room currRoom = PhotonNetwork.room;

        //현재 룸의 접속자수와 최대 접속가능한 수를 문자열로 구성한 다음 TextUI항목에 출력
        //txtConnect.text = currRoom.PlayerCount.ToString() + "/" + currRoom.MaxPlayers.ToString();
    }

    //포톤 추가
    //네트워크 플레이어가 룸으로 접속했을 때 호출되는 콜백 함수
    //PhotonPlayer 클래스 타입의 파라미터가 전달(서버에서...)
    //PhotonPlayer 파라미터는 해당 네트워크 플레이어의 정보를 담고 있다.
    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        //플레이어 ID(접속순번), 이름, 커스텀 속성
        Debug.Log(newPlayer.ToStringFull());
        //룸에 현재 접속자 정보를 display
        GetConnectPlayerCount();
    }

    // 포톤 추가
    //네트워크 플레이어가 룸을 나가거나 접속이 끊어졌을 경우 호출되는 콜백 함수
    void OnPhotonPlayerDisconnected(PhotonPlayer outPlayer)
    {
        // 룸에 현재 접속자 정보를 display
        GetConnectPlayerCount();

    }

    // 포톤 추가
    [PunRPC]
    public void LogMsg(string msg)
    {
       //로그 메시지 Text UI에 텍스트를 누적시켜 표시
       txtLogMsg.text = txtLogMsg.text + msg;
    }

    // 포톤 추가
    // 룸 나가기 버튼 클릭 이벤트에 연결될 함수
    public void OnClickExitRoom()
    {

        // 로그 메시지에 출력할 문자열 생성
        string msg = "\n\t<color=#ff0000>["
                    + PhotonNetwork.player.NickName
                    + "] 님이 퇴장하셨습니다.</color>";

        //RPC 함수 호출
        pv.RPC("LogMsg", PhotonTargets.AllBuffered, msg);

        //현재 룸을 빠져나가며 생성한 모든 네트워크 객체를 삭제
        PhotonNetwork.LeaveRoom();

        //(!) 서버에 통보한 후 룸에서 나가려는 클라이언트가 생성한 모든 네트워크 객체및 RPC를 제거하는 과정 진행(포톤 서버에서 진행)
    }

   
    // 포톤 추가
    //룸에서 접속 종료됐을 때 호출되는 콜백 함수 ( (!) 과정 후 포톤이 호출 )
    void OnLeftRoom()
    {
        // 로비로 이동
        SceneManager.LoadScene("scLobby");
    }

    /////////////////////////////////////////////////////////////////////////////

// Enter 키를 눌렀을 때 호출되는 함수
public void OnChatInputEnter()
{   
    // 채팅 입력 필드의 텍스트를 가져옴
    string msg = chatField.text;

    if (!string.IsNullOrWhiteSpace(msg)) // 채팅 내용이 비어있지 않은 경우에만 로그 메시지 전송
    {
        // 채팅 입력 필드를 초기화
        chatField.text = null;

        // RPC 함수 호출
        pv.RPC("LogChatMessage", PhotonTargets.All, PhotonNetwork.player.NickName, msg);

        // 채팅 UI를 일정 시간 동안 활성화
        chatTimer = chatTimeout;
    }
}

// RPC 함수
[PunRPC]
public void LogChatMessage(string playerName, string message)
{
    // 채팅 로그 메시지 출력
    txtLogMsg.text += string.Format("\n\t<color=#ffffff>[<color=#ffffff>{0}</color>]:</color> <color=#ffffff>{1}</color>", playerName, message);
}

void Update()
{
    if (Input.GetKeyDown(KeyCode.Return))
    {
        // 엔터 키를 눌렀을 때 채팅 UI를 활성화하고, 채팅 입력 필드에 포커스를 줌
        chatUI.SetActive(true);
        chatField.Select();
        chatField.ActivateInputField();
        isChatting=true;
        Debug.Log(isChatting);
    }

    // 일정 시간이 지났을 때 채팅 UI를 자동으로 비활성화
    if (chatTimer > 0f)
    {
        chatTimer -= Time.deltaTime;
        if (chatTimer <= 0f)
        {
            chatUI.SetActive(false);
            isChatting=false;
            Debug.Log(isChatting);
        }
    }
}
}
