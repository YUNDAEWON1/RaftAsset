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

    //스폰장소
    //private Transform[] EnemySpawnPoints;

    // (네트워크 UI 버전에서 ...)
    //Enemy 프리팹을 위한 레퍼런스
    //public GameObject Enemy;

    // 게임끝
    private bool gameEnd;

    // 스테이지 Enemy들을 위한 레퍼런스
    //private GameObject[] Enemys;

    //// 베이스 스타트를 위한 변수
    //public BaseCtrl baseStart;

    //채팅을 위한 인풋필드
    //public InputField inputField;

    void Awake()
    {
        //포톤추가
        //PhotonView 컴포넌트를 레퍼런스에 할당
        pv = GetComponent<PhotonView>();

        playerPos = GameObject.Find("PlayerSpawnPoint").GetComponentsInChildren<Transform>();

        // HG
        objectPos = GameObject.Find("Spawn_Object").transform;


        //룸에 입장한 후 기존 접속자 정보를 출력
        GetConnectPlayerCount();
        ////////////////////////////////////////////////////////////////////////////////////

        // 스폰 위치 얻기
        //EnemySpawnPoints = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        ////포톤추가
        //if(PhotonNetwork.connected && PhotonNetwork.isMasterClient)
        //{
        //    // 몬스터 스폰 코루틴 호출
        //    StartCoroutine(this.CreateEnemy());
        //}
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
            StartCoroutine(this.CreateObject());
        }
        

        //포톤 클라우드로부터 네트워크 메세지 수신을 다시 연결 이 씨이이이이이이이발
        PhotonNetwork.isMessageQueueRunning = true;
        /*
         * 유니티 마크업 태그
         * 글자크기 => <size="글자크기"> 표시할 내용 </size>
         * 글자색 => <color="컬러"> 표시할 내용 </color>
         * 진하게 => <b> 표시할 내용 </b>
         * 이탤릭 => <i> 표시할 내용 </i>
         * 
         *  EX)
         *  string sHp = "<color=yellow><b>HP: ##</b></color>";
         *  string sScore = "<color=#00ff00><b>SCORE: ##</b></color>";
         *  
         *  GUI.Lable ( new Rect(10, 10, 120, 50), sHp.Replace("##", 생명력.ToString() ) );
         *  GUI.Lable ( new Rect(50, 10, 120, 50), sScore.Replace("##", "" + score ) );
         *  
         *  Text에 색을 넣어서 사용해야 할때가 있는데 아래 처럼 사용하면 됨. (rgp색 16진수로 조합해서 사용)
         *  
         */

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
            PhotonNetwork.InstantiateSceneObject("MonsterShark", new Vector3(-155f, 7f, 100), Quaternion.identity, 0, null);

            PhotonNetwork.InstantiateSceneObject("Foundation", new Vector3(-150f, 10f, 100), Quaternion.identity, 0, null);
            PhotonNetwork.InstantiateSceneObject("Foundation", new Vector3(-151.5f, 10f, 100), Quaternion.identity, 0, null);
            PhotonNetwork.InstantiateSceneObject("Foundation", new Vector3(-150f, 10f, 101.5f), Quaternion.identity, 0, null);
            PhotonNetwork.InstantiateSceneObject("Foundation", new Vector3(-151.5f, 10f, 101.5f), Quaternion.identity, 0, null);
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

    //public void OnSubmit(InputField input)
    //{
    //    pv.RPC("LogMsg", PhotonTargets.AllBuffered, "\n\t" + PhotonNetwork.player.NickName + " : " + input.text);
    //    input.text = "";
    //}

    //포톤추가
    //모든 네트워크 플레이어의 스코어UI에 점수를 표시하는 함수를 호출
    IEnumerator SetConnectPlayerScore()
    {
        /*
         * PhotonNetwork.playerList 속성은 같은 룸에 입장한 모든 플레이어의 정보를 반환한다. 따라서
         * 차후, 이 속성은 현재 입장한 플레이어 목록을 UI로 표시할 때 유용하게 활용할 수 있다.
         */

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

            ////해당 베이스의 주인인 플레이어의 UI에 스코어 표시
            //_player.GetComponent<PlayerCtrl>().txtKillCount.text = currKillCount.ToString();

        }

        yield return null;
    }

    //IEnumerator CreateEnemy()
    //{
    //    // 게임중 일정시간마다 계속 호출됨
    //    while(!gameEnd)
    //    {
    //        // 리스폰타임 5초
    //        yield return new WaitForSeconds(5f);

    //        // 스테이지 총 몬스터 갯수 제한
    //        Enemys = GameObject.FindGameObjectsWithTag("Enemy");

    //        if(Enemys.Length < 20)
    //        {
    //            //루트 생성위치는 필요하지 않다.그래서 1 번째 인덱스부터 돌리자
    //            for (int i = 1; i<EnemySpawnPoints.Length; i++)
    //            {
    //                // (포톤 추가)
    //                // 네트워크 플레이어를 Scene 에 귀속하여 생성
    //                PhotonNetwork.InstantiateSceneObject("Enemy", EnemySpawnPoints[i].localPosition, EnemySpawnPoints[i].localRotation, 0, null);
    //            }
    //        }

    //    }
    //}

    //포톤추가
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

        //float pos = Random.Range(-100.0f, 100.0f);
        //포톤네트워크를 이용한 동적 네트워크 객체는 다음과 같이 Resources 폴더 안에 애셋의 이름을 인자로 전달 해야한다. 
        //PhotonNetwork.Instantiate( "MainPlayer", new Vector3(pos, 20.0f, pos), Quaternion.identity, 0 );
        GameObject player = PhotonNetwork.Instantiate("MainPlayer", playerPos[currRoom.PlayerCount].position, playerPos[currRoom.PlayerCount].rotation, 0, ex);


        // 기존 이름으로 변경해야 드럼통 폭파 가능(DestructionRay 스크립트 참조)
        //player.name = "Player";   //태그로 셋팅해서 안해도될듯    // 이거 하든 안하든 깡통이랑 박스 공격이 안되는데...

        //PhotonNetwork.InstantiateSceneObject(string prefabName, Vector3 position, Quaternion rotation, byte group, object[] data);
        //이 함수도 PhotonNetwork.Instantiate와 마찬가지로 네트워크 상에 프리팹을 동시에 생성시키지만, Master Client 만 생성 및 삭제 가능.
        //생성된 프리팹 오브젝트의 PhotonView 컴포넌트의 Owner는 Scene이 된다.

        yield return null;
    }

    // HG
    IEnumerator CreateObject()
    {
        while(true)
        {
            GameObject plank = PhotonNetwork.InstantiateSceneObject("Plank", objectPos.position, objectPos.rotation, 0, null);
            Vector3 newPosition_1 = objectPos.position + new Vector3(0f, 0f, 50f);
            GameObject plastic = PhotonNetwork.InstantiateSceneObject("Plastic", newPosition_1, objectPos.rotation, 0, null);
            Vector3 newPosition_2 = objectPos.position + new Vector3(0f, -2f, 100f);
            GameObject leaf = PhotonNetwork.InstantiateSceneObject("Leaf", newPosition_2, objectPos.rotation, 0, null);
            yield return new WaitForSeconds(10f);
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
        pv.RPC("LogChatMessage", PhotonTargets.All, PhotonNetwork.playerName, msg);

        // 채팅 UI를 일정 시간 동안 활성화
        chatTimer = chatTimeout;
    }
}

// RPC 함수
[PunRPC]
public void LogChatMessage(string playerName, string message)
{
    // 채팅 로그 메시지 출력
    txtLogMsg.text += string.Format("\n\t<color=#ffffff>[<color=#ffffff>{0}</color>]:</color> <color=#ffffff>{1}</color>", PhotonNetwork.player.NickName, message);
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



/* RPC(Remote Procedure Call)함수의 네트웍 전달 대상 설정인 PhotonTargets 열거형 인자 옵션 설정
 *  옵션                      설명
 *  All                       모든 네트웍 유저에게 RPC 데이타를 전송하고 자신은 즉시 RPC 함수를 실행    
 *  Others                    자기 자신을 제외한 모든 네트웍 유저에게 RPC 데이타를 전송 
 *  MasterClient              Master Client에게만 RPC 데이타를 전송
 *  AllBuffered               All 옵션과 같으며, 또한 나중에 입장한 네트웍 유저에게 버퍼에 저장돼 있던 RPC 데이타가 전송
 *  OtherBuffered             Others 옵션과 같으며, 또한 나중에 입장한 네트웍 유저에게 버퍼에 저장해둔 RPC 데이타를 전송
 *  AllViaServer              모든 네트웍 유저에게 거의 동일한 시간에 RPC 데이타를 전송하기 위하여 서버에서 연결된 
 *                            모든 클라이언트들에게 RPC 데이타를 동시에 전송
 *  AllBufferedViaServer      AllViaServer 옵션과 같으며, 버퍼에 저장해둔 RPC 데이타를 나중에 입장한 네트웍 유저에게 전송 
 *  
 *  사용 방식: 1
 *  //자신의 아바타일 경우는 로컬함수를 호출하여 케논을 발포
 *  FireStart(100);
 *
 *  //원격 네트워크 플레이어의 자신의 아바타 케릭터에는 RPC로 원격으로 FireStart 함수를 호출 
 *  pv.RPC( "FireStart", PhotonTargets.Others, 100 );
 *
 * 사용 방식: 2
 *  //모든 네트웍 유저에게 RPC 데이타를 전송하여 RPC 함수를 호출, 로컬 플레이어는 로컬 Fire 함수를 바로 호출 
 *  //pv.RPC("FireStart", PhotonTargets.All, 100);
 *  
 *   [PunRPC]
 *   //플레이어 발사
 *  private void FireStart(int power)
 *  {
 *  }
 */
