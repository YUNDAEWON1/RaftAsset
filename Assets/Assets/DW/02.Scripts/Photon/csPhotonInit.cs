using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Data;

//(UI 버전에서 사용)
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class csPhotonInit : MonoBehaviour
{


    InventoryManager inventory;
    //App의 버전 정보 (번들 버전과 일치 시키자...)
    public string version = "Ver 0.1.0";

    //개발하는 동안 PUN 으로 개발하는 것이 처음이면 최대한 Unity 콘솔에 로그를 많이 찍어 어떤 
    //사항이 발생하는지 파악하는 것을 권장 합니다. 
    //예상하는 대로 동작하는 것에 대하여 확신이 서면 로그 레벨을 Informational 으로 변경 하자.
    public PhotonLogLevel LogLevel  = PhotonLogLevel.Full;

    //플레이어의 이름을 입력하는 UI 항목 연결을 위한 레퍼런스 (using UnityEngine.UI 추가해야함) (UI 버전에서 사용)
    public InputField userId;

    //룸 이름을 입력받을 UI 항목 연결 레퍼런스 (UI 버전에서 사용)
    public InputField roomName;

    public InputField passWord;

    //RoomItem이 차일드로 생성될 Parent 객체의 레퍼런스 (UI 버전에서 사용)
    public GameObject scrollContents;
  

    //룸 목록만큼 생성될 RoomItem 프리팹 연결 레퍼런스 (UI 버전에서 사용)
    public GameObject roomItem;

    //플레어의 생성 위치 저장 레퍼런스
    public Transform playerPos;

    public GameObject btnJoinWorldOff;
    public GameObject btnJoinWorldOn;
    public GameObject btnJoinWorldHover;


    private string currentRoomName;

    public InputField passwordInputField;

    public Text nopwd;
    public Text wrongpwd;

    public bool isCreateMode=false;


 
    //App 인증 및 로비연결
    void Awake()
    {
        
        if (!PhotonNetwork.connected)
        {
            
            PhotonNetwork.ConnectUsingSettings(version);

            PhotonNetwork.logLevel = LogLevel;

            //현재 클라이언트 유저의 이름을 포톤에 설정
            //PhotonView 컴포넌트의 요소 Owner의 값이 된다
           PhotonNetwork.playerName = "GUEST " + Random.Range(1, 9999);

        }

        // ScrollContents의 Pivot 좌표를 Top, Left로 설정 하자. (UI 버전에서 사용)
        scrollContents.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 1.0f);

        // 로비에 자동 입장
        PhotonNetwork.autoJoinLobby = true;

    }

    public void OnClickCreateMode()
    {
        isCreateMode=true;
    }
    public void OnClickNormalMode()
    {
        isCreateMode=false;
    }


    /*
     * 포톤은 로비 기반으로 서버가 이루어져 있음 따라서 연결은 로비로 연결되는 것임
     * 로비에서 방을 만들거나, 특정 방에 연결하거나, 랜덤으로 열결하게 됨
     */
    //포톤 클라우드에 정상적으로 접속한 후 로비에 입장하면 호출되는 콜백 함수
    //즉, 로비 입장 후 포톤 클라우드 서버가 클라이언트에게 정상적으로 로비에 입장했다는 콜백 함수를 호출~
    void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby !!!");

        /*
         * 클라이언트의 룸 찾기 : 클라이언트는 게임(방) 이름을 통해 참여하거나 Photon 에게 나에게 맞는 게임을 찾아 달라고 
         * 요청 하여 참여한다.
         *  •무작위: 무작위로 플레이어를 매칭. 마음대로 플레이어들을 룸에 입장 시키거나 동등하게 분배 후 입장하게 함.
         *  •필터: 좀 더 나에게 맞도록 매칭 시키기 위하여 무작위 매칭에 필터를 사용.
         *  •리스팅: 플레이어는 로비에 나열된 룸 중에서 하나를 선택 후 입장.
          * •파라미터화: 예측되는 속성 정의를 통한 커스터마이징된 무작위 매칭
         */

        // 로비 입장후 이미 생성된 룸(방) 중에서 무작위로 선택해 입장하는 (Random Match Making) 함수
        //PhotonNetwork.JoinRandomRoom(); // (UI 버전에서는 주석 처리)

        // 유저 아이디를 가져와 셋팅 (UI 버전에서 사용)
        // userId.text = GetUserId();

        //특정 조건을 만족하는 룸을 대상으로 무작위로 추출해 입장하는 오버로딩 된 함수 호출 방식 

        /* 사용 방법 : PhotonNetwork.JoinRandomRoom(Hsshtable 룸 속성, byte 최대접속자수);
         * 
         *  (EX 1) 
         *  
         * //using System.Collections; 주석 처리
         * using ExitGames.Client.Photon;
         * 
         * ...
         * ...
         * 
         * //무작위 추출할 룸의 조건을 Hashtable로 정의
         * //생성된 룸 중 맵은 1번을 사용하고 접속 가능한 레벨은 10 LV로 제한된 속성의 Hsshtable 생성
         * Hashtable  roomProperties = new Hashtable() { { "map", 1 }, { "minLevel", 10 } };
         * 
         * 위에 정의한 룸 속성과 최대 플레이어 수가 7명인 룸을 검색해 선택 랜덤 접속
         * PhotonNetwork.JoinRandomRoom(roomProperties, 7);
         * 
         *  (EX 2) 
         *  
         * using System.Collections;
         * //using ExitGames.Client.Photon; 주석 처리
         *  
         *  생성된 룸 중 맵은 3번을 사용하고 접속 가능한 최소 레벨은 10 LV로 제한된 속성의 Hsshtable 생성
         *  
         *  ExitGames.Client.Photon.Hashtable  roomProperties = new ExitGames.Client.Photon.Hashtable() { { "map", 3 }, { "minLevel", 10 } };
         *  
         *  위에 정의한 제한된 룸 속성과 최대 플레이어 수가 10명인 룸을 검색해 선택 랜덤 접속
         *  PhotonNetwork.JoinRandomRoom(roomProperties, 10);
         */

    }

    //로컬에 저장된 플레이어 이름을 반환하거나 랜덤 생성하는 함수 (UI 버전에서 사용)
    string GetUserId()
    {
        string _userId = PlayerPrefs.GetString("USER_ID");
        _userId = userId.text;
        return _userId;
    }

    // 룸에 입장하면 호출되는 콜백 함수 
    // PhotonNetwork.CreateRoom 함수로 룸을 생성한 후 입장하거나, PhotonNetwork.JoinRandomRoom, PhotonNetwork.JoinRoom 함수를 통해 입장해도 호출 된다.
    void OnJoinedRoom()
    {
        Debug.Log("Enter Room");
        Debug.Log(PhotonNetwork.player.NickName);
        //여기까지 게임을 실행하면 로비 입장, 랜덤 매치 메이킹, 룸 생성, 룸 입장의 과정을 거치며 Console 뷰에 
        //Joined Lobby !!!, No Rooms !!!, Enter Room 메시지가 출력~ 즉 순서대로 룸 입장까지 완료된 로그 메시지를 확인하자~!

        //플레이어를 생성하는 함수 호출 (UI 버전에서는 주석 처리) 
        //CreatePlayer();

        //룸 씬으로 전환하는 코루틴 실행 (UI 버전에서 사용)
        StartCoroutine(this.LoadStage());
    }

    /*
     * PhotonNetwork.Instantiate
     * 
     * 게임오브젝트 또는 프리팹을 로컬 및 네트워크상 동적으로 생성
     * 하려면 PhotonNetwork.Instantiate 이 함수를 사용해야 함
     * 
     * PhotonNetwork.Instantiate() 함수를 사용하여 생성한 게임오브젝트
     * 나 프리팹은 현재 동일 룸에 접속해 있는 모든 네트워크 플레이어에게
     * 객체를 동시에 생성 시킴 (cf)이 함수는 유니티 빌트인 네트워크의 
     * Network.Instantiate 함수와 동일 기능(역할)을 하는 함수
     * 
     * PhotonNetwork.Instantiate(string prefabName, Vector3 position, Quaternion rotation, byte group);
     * 
     * 이 함수의 첫 번째 인자는 Network.Instantiate 함수와 다르게 string형 타입으로서 생성하려하는 
     * 애셋(프리팹)의 이름을 전달 해야함 즉, 예약 폴더인 Resources 폴더에서 찾겠다!!!(이 함수는 해당 애셋을 Resources 폴더에서 로드)
     * 그러므로,  PhotonNetwork.Instantiate 함수를 통하여 생성하려는 애셋(프리팹)은 무조건 Resources 폴더에 위치해야함
     * PhotonNetwork.Instantiate("프리팹, 위치, 각도, 그룹"); 함수는 현재 게임에 접속한 모든 유저에게 프리팹을
     * 생성해주며 내부적으론 Buffered RPC를 호출하여 나중에 접속한 플레이어도 미리 생성된 프리팹을 볼 수 있음.
     * 또한, group을 지정하면 동일 group의 사용자들에게만 생성
     * 
     * PhotonNetwork.Instantiate(string prefabName, Vector3 position, Quaternion rotation, byte group, object[] data);
     * 
     * 다음과 같이 오버로딩 되어있음...마지막에 데이타를 전달 가능하다~^^
     * 위에 두 함수로 생성된 오브젝트는 클라이언트가 접속 종료하면 네트워크상의 모든 오브젝트는 사라짐...
     * 즉, 위의 두 Method는 로컬 플레이어에 종속된 Object를 생성하는 것 이다. PhotonView 컴포넌트의 요소 Owner의 값은 PhotonNetwork.playerName
     * 
     * PhotonNetwork.InstantiateSceneObject(string prefabName, Vector3 position, Quaternion rotation, byte group, object[] data);
     * 이 함수는 위 두함수와 마찬가지로 네트워크 상에 프리팹을 동시에 생성시키지만 Master Client(방장)만 생성 및 삭제할 수 있음
     * 확인해보면, 생성된 프리팹의 PhotonView 컴포넌트의 요소 Owner는 Scene이 된다.
     * 즉, 위의 Method는 Scene에 종속된 Object를 생성하는 것 이다.
     * 
     */

    //플레이어를 생성하는 함수 (UI 버전에서는 주석 처리)
    /*void CreatePlayer()
    {
        //float pos = Random.Range(-100.0f, 100.0f);
        //포톤네트워크를 이용한 동적 네트워크 객체는 다음과 같이 Resources 폴더 안에 애셋의 이름을 인자로 전달 해야한다. 
        //PhotonNetwork.Instantiate( "MainPlayer", new Vector3(pos, 20.0f, pos), Quaternion.identity, 0 );
        PhotonNetwork.Instantiate( "MainPlayer", playerPos.position, playerPos.rotation, 0 );
        
        
        //PhotonNetwork.InstantiateSceneObject(string prefabName, Vector3 position, Quaternion rotation, byte group, object[] data);
        //이 함수도 PhotonNetwork.Instantiate와 마찬가지로 네트워크 상에 프리팹을 동시에 생성시키지만, Master Client 만 생성 및 삭제 가능.
        //생성된 프리팹 오브젝트의 PhotonView 컴포넌트의 Owner는 Scene이 된다.
        


    }*/


    public void StartLoadingScene()
    {
        // 다른 클래스에서 LoadingSceneController.LoadScene() 함수를 호출하여 로딩 씬을 시작
        LoadingSceneController.LoadScene("Stage");               //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@YM수정@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        //LoadingSceneController.LoadScene("scNetTest");
    }

    IEnumerator LoadStage()
    {
    //씬을 전환하는 동안 포톤 클라우드 서버로부터 네트워크 메시지 수신 중단
    //(Instantiate, RPC 메시지 그리고 모든 네트워크 이벤트를 안받음 )
    //차후 전환된 scene의 초기화 설정 작업이 완료후 이 속성을 true로 변경
    PhotonNetwork.isMessageQueueRunning = false;
    
    // 로딩 씬을 시작
    StartLoadingScene();

    // 씬 로딩이 완료 될때까기 대기...
    yield return new WaitUntil(() => PhotonNetwork.isMessageQueueRunning);

    Debug.Log("로딩 완료");
    }
    //룸 씬으로 이동하는 코루틴 함수 (UI 버전에서 사용)
    // IEnumerator LoadStage()
    // {

        
    //     //씬을 전환하는 동안 포톤 클라우드 서버로부터 네트워크 메시지 수신 중단
    //     //(Instantiate, RPC 메시지 그리고 모든 네트워크 이벤트를 안받음 )
    //     //차후 전환된 scene의 초기화 설정 작업이 완료후 이 속성을 true로 변경
    //     PhotonNetwork.isMessageQueueRunning = false;
    //     //백그라운드로 씬 로딩
    //     AsyncOperation ao = SceneManager.LoadSceneAsync("scNetTest");

        

    //       // 씬 로딩이 완료 될때까기 대기...
    //     yield return ao;

    //     Debug.Log("로딩 완료");
    // }

    /**************************************************************************************************
    // 5.3 이후 
    SceneManager.LoadScene(0);                                          // 로드. 
    SceneManager.LoadScene("SceneName");
    AsyncOperation ao = SceneManager.LoadSceneAsync(0);                 // 로드. (비동기)
    AsyncOperation ao = SceneManager.LoadSceneAsync("SceneName");
    SceneManager.LoadScene(0, LoadSceneMode.Additive);                  // 씬 병합 추가. 
    SceneManager.LoadScene("SceneName", LoadSceneMode.Additive);
    SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);             // 씬 병합 추가. (비동기)
    SceneManager.LoadSceneAsync("SceneName", LoadSceneMode.Additive);
    SceneManager.UnloadScene(0);                                        // 언로드. 
    SceneManager.UnloadScene("SceneName");
    SceneManager.sceneCount;                                            // 현재 로드 된 씬 개수. 
    SceneManager.sceneCountInBuildSettings;                             // BuildSetting 에 등록 된 씬 개수. 
    SceneManager.GetActiveScene().buildIndex;                           // 현재 씬 인덱스. 
    SceneManager.GetActiveScene().name;                                 // 현재 씬 이름. 
 
    // 씬 정보 조회. 
    Scene activeScene = SceneManager.GetActiveScene();
    Scene scene1 = SceneManager.GetSceneAt(0);
    Scene scene2 = SceneManager.GetSceneByName("SceneName");
    Scene scene3 = SceneManager.GetSceneByPath("Assets/01. Scenes/SceneName.unity");
    Scene[] loadedScenes = SceneManager.GetAllScenes();

    // Scene 구조체. 
    int buildIndex;
    string name;
    string path;
    bool isLoaded;
    bool isDirty;       // 씬을 변경(수정)했는지 여부. 
    int rootCount;      // 씬의 Root에 있는 GameObject 개수. 
    bool IsValid();     // 유효한 씬인지 여부. 

    // 기타. 
    Scene scene = gameObject.scene;                             // 게임오브젝트가 속해있는 씬을 가져오기. 
    GameObject go = new GameObject("New Object");               // 게임오브젝트를 생성하면 현재 씬에 추가 됨. 
    SceneManager.MoveGameObjectToScene(go, scene);              // 게임오브젝트를 다른 씬으로 이동. 
    SceneManager.MergeScenes(sourceScene, destinationScene);    // 씬을 병합. 
 
    // SceneManager.Get~() 으로 가져올 수 있는 것은 로드가 끝난 씬만 가능. 
    Scene scene = SceneManager.GetSceneByName("SceneName");
    bool isValid = scene.IsValid();     // false 가 리턴 됨.

    //ex 1
    SceneManager.LoadScene("03. Test", LoadSceneMode.Additive);
    Scene scene = SceneManager.GetSceneByName("03. Test");
    GameObject go = new GameObject("New Object");
    SceneManager.MoveGameObjectToScene(go, scene);

    //ex 2
    Scene scene = SceneManager.GetSceneAt(1);
    GameObject go = new GameObject("New Object");

    yield return new WaitForSeconds(3.0f);

    SceneManager.MoveGameObjectToScene(go, scene);
    SceneManager.MoveGameObjectToScene(gameObject, scene);

    yield return new WaitForSeconds(1.0f);

    SceneManager.UnloadSceneAsync("02. Room");


    [참고] PhotonNetwork.LoadLevel() 함수는 PhotonNetwork.isMessageQueueRunning를 false로 변경하고
           인자로 전달된 씬을 로드.

    **********************************************************************************************************/



   
    //세계 만들기 버튼 클릭 시 호출될 함수 (UI 버전에서 사용)
    public void OnClickCreateRoom()
    {
        string _roomName = roomName.text;
        string _passWord = passWord.text;
        if (!string.IsNullOrEmpty(_passWord))
         {
            _roomName=roomName.text+"[pwd]";
         }

        //로컬 플레이어의 이름을 설정
        if(isCreateMode)
        {
          userId.text="CreateMode";
        }
       
        PhotonNetwork.player.NickName = userId.text;
   

        //플레이어의 이름을 로컬 저장
        //PlayerPrefs.SetString("USER_ID", userId.text);

        //생성할 룸의 조건 설정 1
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 10;
        

        if (!string.IsNullOrEmpty(_passWord))
    {
        // 비밀번호가 있는 방 생성
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "password", _passWord } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "password" };
    }

        //지정한 조건에 맞는 룸 생성 함수 
        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
    }

    //생성된 룸 목록이 변경됐을 때 호출되는 콜백 함수 (최초 룸 접속시 호출) (UI 버전에서 사용)
    void OnReceivedRoomListUpdate()
    {
        // 포톤 클라우드 서버에서는 룸 목록의 변경이 발생하면 클라이언트로 룸 목록을 재전송하기
        // 때문에 밑에 로직이 없으면 다른 클라이언트에서 룸을 나갈때마다 룸 목록이 쌓인다.
        // 룸 목록을 다시 받았을 때 새로 갱신하기 위해 기존에 생성된 RoomItem을 삭제  
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOM_ITEM"))
        {
            Destroy(obj);
        }

        //Grid Layout Group 컴포넌트의 Constraint Count 값을 증가시킬 변수
        int rowCount = 0;
        //스크롤 영역 초기화
        //scrollContents.GetComponent<RectTransform>().sizeDelta = new Vector2(0 ,0);
        scrollContents.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        //수신받은 룸 목록의 정보로 RoomItem 프리팹 객체를 생성
        //GetRoomList 함수는 RoomInfo 클래스 타입의 배열을 반환
        foreach (RoomInfo _room in PhotonNetwork.GetRoomList())
        {
            Debug.Log(_room.Name);
            //RoomItem 프리팹을 동적으로 생성 하자
            GameObject room = (GameObject)Instantiate(roomItem);
            //생성한 RoomItem 프리팹의 Parent를 지정
            room.transform.SetParent(scrollContents.transform, false);

            //생성한 RoomItem에 룸 정보를 표시하기 위한 텍스트 정보 전달
            RoomData roomData = room.GetComponent<RoomData>();
            roomData.roomName = _room.Name;
            roomData.connectPlayer = _room.PlayerCount;
            roomData.maxPlayers = _room.MaxPlayers;

            //텍스트 정보를 표시 
            roomData.DisplayRoomData();

            //RoomItem의  Button 컴포넌트에 클릭 이벤트를 동적으로 연결
            roomData.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { OnClickRoomItem(roomData.roomName); Debug.Log("Room Click " + roomData.roomName); });
            //Grid Layout Group 컴포넌트의 Constraint Count 값을 증가시키자
            scrollContents.GetComponent<GridLayoutGroup>().constraintCount = ++rowCount;
            //스크롤 영역의 높이를 증가시키자
            scrollContents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 20);
        }
    }

    
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

    //RoomItem이 클릭되면 호출될 이벤트 연결 함수 (UI 버전에서 사용)
    void OnClickRoomItem(string roomName)
    {
        //로컬 플레이어의 이름을 설정
        PhotonNetwork.player.NickName = userId.text;

        ChangeBtn(btnJoinWorldOn,btnJoinWorldOff);
        // JoinRoom 함수 호출을 위해 roomName 변수 저장

         //플레이어 이름을 저장
        //PlayerPrefs.SetString( "USER_ID", userId.text );
        //인자로 전달된 이름에 해당하는 룸으로 입장
        //PhotonNetwork.JoinRoom(roomName);

        currentRoomName=roomName;
    }

    public void OnClickButton()
    {   
        OnClickJoinRoom();
    }

    public void OnClickJoinRoom()
{
    // 입력된 비밀번호를 가져옵니다.
    string password = passwordInputField.text;

    // 비밀번호가 필요한 방인지 체크
    RoomInfo roomInfo = PhotonNetwork.GetRoomList().FirstOrDefault(r => r.Name == currentRoomName);
    if (roomInfo != null && roomInfo.CustomProperties.ContainsKey("password"))
    {
        string correctPassword = roomInfo.CustomProperties["password"] as string;
        if (string.IsNullOrEmpty(password))
        {
            // 입력된 비밀번호가 없는 경우
            nopwd.text = "비밀번호를 입력해주세요!";
            StartCoroutine(ClearMessageAfterDelay(3f)); // 3초 뒤에 ClearMessageAfterDelay 코루틴 실행
        }
        else if (password == correctPassword)
        {
            PhotonNetwork.JoinRoom(currentRoomName);
        }
        else
        {
            // 비밀번호가 틀렸을 경우 처리
            Debug.Log("비밀번호가 틀렸습니다. 다시 입력해주세요.");
            wrongpwd.text="비밀번호가 틀렸습니다!";
            StartCoroutine(ClearMessageAfterDelay(3f));
        }
    }
    else
    {
        // 비밀번호가 필요없는 방이면 바로 JoinRoom 호출
        PhotonNetwork.JoinRoom(currentRoomName);
    }
}

IEnumerator ClearMessageAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    nopwd.text = null;
    wrongpwd.text=null;
}


    
    //포톤 클라우드 서버로 접속하는 과정에 대한 로그 메시지 출력을 위한 콜백함수 
    //마지막 JoinedLobby 로그 메시지가 표시되면 정상적으로 포톤 클라우드에 접속하여 로비에 입장한 상태임.
    void OnGUI()
    {

        //화면 좌측 상단에 접속 과정에 대한 로그를 출력(포톤 클라우드 접속 상태 메시지 출력)
        // PhotonNetwork.ConnectUsingSettings 함수 호출시 속성 PhotonNetwork.connectionStateDetailed는
        //포톤 클라우드 서버에 접속하는 단계별 메시지를 반환함.
        //Joined Lobby 메시지시 포톤 클라우드 서버로 접속해 로비에 안전하게 입장했다는 뜻
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        ////만약 포톤네트워크에 연결 되었다면...
        //if (PhotonNetwork.connected)
        //{
        //    GUI.Label(new Rect(0, 50, 200, 100), "Connected");

        //    //룸 리스트를 배열로 받아온다.
        //    RoomInfo[] roomList = PhotonNetwork.GetRoomList();

        //    if (roomList.Length > 0)
        //    {
        //        foreach (RoomInfo info in roomList)
        //        {
        //            GUI.Label(new Rect(0, 80, 400, 100), "Room: " + info.Name
        //                + " PlayerCount/MaxPlayer :" + info.PlayerCount + "/" + info.MaxPlayers //현재 플레이어/최대 플레이어
        //                + " CustomProperties Count " + info.CustomProperties.Count // 설정한 CustomProperties 수 
        //                + " Map ???: " + info.CustomProperties.ContainsKey("Map") //키로 설정한 Map이 있나
        //                + " Map Count " + info.CustomProperties["Map"] // 설정한 키 값 
        //                + " GameType ??? " + info.CustomProperties.ContainsKey("GameType") //키로 설정한 GameType이 있나
        //                + " GameType " + info.CustomProperties["GameType"]);// 설정한 키 값 
        //        }
        //    }
        //    else
        //    {
        //        GUI.Label(new Rect(0, 80, 400, 100), "No Room List");
        //    }
        //}
        ////PhotonServerSettings 값 가져오기
        //{
        //    GUI.Label(new Rect(0, 170, 400, 100), "AppID  :  " +
        //        PhotonNetwork.PhotonServerSettings.AppID);
        //    GUI.Label(new Rect(0, 200, 200, 100), "HostType  :  " +
        //        PhotonNetwork.PhotonServerSettings.HostType);
        //    GUI.Label(new Rect(0, 230, 200, 100), "ServerAddress  :  " +
        //        PhotonNetwork.PhotonServerSettings.ServerAddress);
        //    GUI.Label(new Rect(0, 260, 200, 100), "ServerPort  :  " +
        //        PhotonNetwork.PhotonServerSettings.ServerPort);
        //    //PhotonNetwork.PhotonServerSettings.UseCloud(); 

        //    //핑 테스트
        //    int pingTime = PhotonNetwork.GetPing();
        //    GUI.Label(new Rect(0, 310, 200, 100), "Ping: " + pingTime.ToString());
        //}
    }
}
// 참고 https://doc-api.photonengine.com/ko-kr/pun/current/class_room_options.html


/*
 * = 네트워크 기초 =
 *                 
 * 1) 네트워크 게임??
 * 물리적/공간적 떨어져 있는 다른 유저와 통신망(LAN, 인터넷 망)을 통해서 서로 데이타를 
 * 주고받고 게임하는 것.
 * 
 * 2)네트워크 게임의 물리적 구조 (네트워크 구조를 그림으로 그릴줄 알아야함...기본!!!)
 * 
 * ■ P2P(Peer to Peer) 방식 => 서버/클라이언트 모델에 대응되는 모델
 * 
 *  => 유저끼리 별도의 서버 없이 네트워크 연결하여 데이타를 송수신하는 구조를 말함
 *     접속자가 적은 게임에 적용되며, 네트워크 상 유저(사용자)가 직접 접속해서 게임을 함.
 * 
 *    ○  응용 중심에 따른 분류(사용 용도에 따라 구분)
 *
 *       정보 공유형 응용
 *
 *          - 파일이나 데이터 등을 공유하거 메시지의 교환을 통하여 정보를 공유 (메신저)
 *
 *          - mp3 음악 파일을 공유하는 넵스터, 그누텔라, 인스턴트 메시지를 교환하는 ICQ 등 (파일 공유 프로그램)
 *
 *       자원 공유형 응용
 *
 *          - 하나의 커다란 처리를 세분하여 분산 클라이언트가 처리한 후 최종적으로 중심이 되는
 *
 *             서버에게 처리 결과를 전송하여 결합하는 시스템
 *
 *          - 각 컴퓨터의 처리능력(CPU)을 하나의 컴퓨팅 자원으로 취급하여 여러 컴퓨터가
 *
 *             필요에 따라 공유
 *             
 * ■ 서버/클라이언트 모델(방식) => 우리가 할거...(거의 대부분의 온라인 게임 방식)
 * 
 *  => 게임 서버를 구축하고 여러 유저(클라이언트)가 접속해서 상호간의 데이터를 
 *     게임 서버를 이용해 송/수신하는 방식으로 게임 서버의 기능적 역할은 접속한 클라이언트 사이의 데이타를 Relay하고
 *     게임 데이터를 DB 서버에 저장한다. 
 *     
 *     (참고) 유니티 빌트인 네트워크(Built in Network))로 이러한 서버/클라이언트 네트워크
 *     게임을 깊이 있고 복잡한 네트워크 지식없이 손쉽게 개발할 수 있다. 하지만 우린 포톤을 이용한 네트워크
 *     게임을 개발할것이다...그러나 포톤 하나만 알면 유니티 빌트인 네트워크는 껌이다...이유는
 *     포톤 개발사는 Built in Network를 맞춰서 (98% 이상(내 생각..) ) 용어, 사용법, 
 *     주요 기능(Network View, State Synchronization, RPC: Remote Procedure Call) 등을 제공한다
 *     (즉 포톤 클라우드는 유니티 빌트인 네트워크 API의 부족한 기능을 보완하고 네트워크 게임에 필요한 필수적인 기능을 확장했기 때문에 API 사용법이 거의 동일)
 *     따라서 포톤만 완벽히 알면 Built in Network 뿐만 아니라 유니티를 지원하는 검증된 여러 네트워크 게임 엔진(서드파티)을 
 *     살만 약간 붙혀서 손쉽게 사용 가능~!
 *     
 *     또한, 온라인 네트워크 게임 개발을 위해선 물리적 서버(돈) + 네트워크 게임 서버(기술력)가 구축되야 한다.
 *     네트워크 게임을 직접 구현하는것은 숙련된 네트워크 개발 경험 및 스킬을 갖추어야 한다.
 *     그리고 수많은 알파/베타 테스트등을 거쳐 네트워크 속도/안정성/최적화등의 작업이 이루어져야 한다.
 *     소규모/인디개발자에겐 현실적으로 어렵다...따라서 검증된 네트워크 게임 엔진을 사용하자.
 *     우린 여기서 서드파티(유니티를 베이스로...) 네트워크 게임 엔진인 포톤을 활용!~
 *     
 *     유니티를 지원하는 서드파티 네트워크 게임 엔진(서버) (가격/성능/서버의 운영체제 등을 고려하여 네트워크 게임 서버 선택!!!)
 *     Photon           http://www.exitgames.com (유니티에서 가장 오래 사랑받고 검증된 엔진)
 *     프라우드넷       http://www.nettention.com
 *     ElectroServer    http://www.electrotank.com
 *     MuchDefferent    http://www.muchdifferent.co.kr
 *     SmartFox         http://www.smartfoxserver.com
 *     
 *     (추가) 유니티에선 Unity 5.3이상 버전부터 대규모 네트워크 게임 개발이 가능한 UNET을 제공한다.
 *     Built in Network API를 대체하는 API로서 MMO등의 대규모 네트워크 게임 개발을 위한 필요한 기능을
 *     쉽고 편리하게 구현해 놓은 네트워... (2KB 남음)
 */