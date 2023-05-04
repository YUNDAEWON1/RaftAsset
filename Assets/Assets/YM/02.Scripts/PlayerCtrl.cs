using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour {

    [HideInInspector]
    public Dictionary<int, string> photonMapping = new Dictionary<int, string>()
    {
        {0, "Hammer"},
        {1, "Hook"},
        {2, "Spear_wood"},
        {3, "Plastic"},
        {4, "Plank"},
        {5, "Potato"},
        {6, "Purifier"},
        {7, "Leaf" },
        {8, "Axe"},
        {9, "Bed" },
        //{10, "FireCamp" },
        {11, "Cup" },
        {12, "Scrap" },
        {13, "Rope"},
        {14, "Grill" },
        {15, "CropPlot" },
        {16, "WoodenStair"},
        {17, "WoodenWall"},
        {18, "WoodenPole"},
        {19, "WoodenFloor"},
        {20, "Foundation" },
        {21, "Cooked_Potato"},
        {22, "Rocket"},
        {23, "Rock"}
    };
    [HideInInspector]
    public bool isAnimating = false;               // 애니메이션 실행중 중복 실행을 막기위한 변수
    private float speed;                             // 캐릭터 움직임 스피드.
    private float jumpSpeed;                         // 캐릭터 점프 힘.
    public float gravity;                           // 캐릭터에게 작용하는 중력.
    public float turnSpeed = 2f;                  // 마우스 회전 속도(추후 UI세팅에서 감도설정 넣을수도 있을듯)          
    private float xRotate = 0.0f;                   // 내부 사용할 X축 회전량은 별도 정의 ( 카메라 위 아래 방향 )
    private float throwGage = 0f;                   // 훅 던질때 사용할 게이지변수
    public bool inventoryOn = false;                // 인벤토리가 켜져있는지 확인할 변수
    public bool EscapeOn = false;                   // esc메뉴가 켜져있는지 확인할 변수
    public bool chatingOn = false;                  // 채팅창이 켜져있는지 확인할 변수
    public bool constructMode = false;              // 건축모드인지 확인할 변수
    public bool hammerMode = false;              // 해머모드인지 확인할 변수

    public int swapNum = 0;                         // 소지품 몇번째칸을 가리키고 있는지

    public float hp = 100;                          // 체력
    public float satiety = 100;                     // 포만감
    public float thirsty = 100;                     // 목마름

    public bool swimMode = false;                   // 물에 들어갔는지 판단할 변수
    public bool hookThrow = false;                  // Hook을 던진상태인지 확인을 위한 변수(훅 던지고 회수 안된 상태-> true)
    private GameObject hook;                        // Hook 객체 저장할 변수(던져진 훅 컨트롤 시 사용)

    private CharacterController controller;         // 현재 캐릭터가 가지고있는 캐릭터 컨트롤러 콜라이더.
    private Vector3 MoveDir;                        // 캐릭터의 움직이는 방향.
    private Animator ani;                           // 애니메이터 연결

    [HideInInspector]
    public GameObject rightHandle;                 // 오른손 핸들러오브젝트 연결
    private GameObject rightHandleSave;             // 오른손에 쥔 오브젝트 저장용 변수

    public Transform firePos;

    //public Camera myCamera;

    public GameObject stuffs;
    private InventoryManager inventoryManager;

    //Ray ray;                                        // Ray 정보 저장 구조체 
    //RaycastHit hitInfo;                            // Ray에 맞은 오브젝트 정보를 저장 할 구조체

    // 포톤추가
    //PhotonView 컴포넌트를 할당할 레퍼런스
    PhotonView pv = null;

    //대원 추가************************
    //플레이어 하위의 Canvas 객체를 연결할 레퍼런스
    public Canvas hudCanvas;

    //위치정보를 송수신할때 사용할 변수 선언 및 초기값 설정
    Vector3 currPos = Vector3.zero;
    Quaternion currRot = Quaternion.identity;

    public Transform camTrans;

    private ConstructMode constructScript;
    private HammerMode hammerScript;
    private SharkCtrl sharkCtrl;
    private GameManager gm;

    //public GameObject hookPrefab;                   // test때문에 넣어놓은 훅프리팹


    void Awake()
    {
        constructScript = GetComponent<ConstructMode>();
        hammerScript = GetComponent<HammerMode>();
        ani = gameObject.GetComponentInChildren<Animator>();
        rightHandle = GameObject.FindGameObjectWithTag("RightHandle");
        controller = this.GetComponent<CharacterController>();

        stuffs = GameObject.FindGameObjectWithTag("QuickSlot");
        inventoryManager = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryManager>();
        //for (int i = 0; i < GameObject.FindGameObjectWithTag("Stuffs").transform.childCount; i++)       // 추후에는 처음시작시에는 Hook만 가지게끔하고 저장된데이터가 있으면 가져와서 넣게끔하기
        //{
        //    stuffs[i] = GameObject.FindGameObjectWithTag("Stuffs").transform.GetChild(i).gameObject;
        //    stuffsCount[i] = 1;
        //}

        // 포톤추가
        //PhotonView 컴포넌트 할당
        pv = this.GetComponent<PhotonView>();

        //PhotonView Observed Components 속성에 PlayerCtrl(현재) 스크립트 Component를 연결
        pv.ObservedComponents[0] = this;

        //데이타 전송 타입을 설정
        pv.synchronization = ViewSynchronization.UnreliableOnChange;

        //카메라 관련 넣을지 확인해보기
        //Debug.Log(pv.isMine);
        if (pv.isMine)
        {
            Camera.main.GetComponent<FirstPersonCam>().target = this.camTrans;
        }

        // 원격 플래이어의 위치 및 회전 값을 처리할 변수의 초기값 설정 
        // 잘 생각해보자 이런처리 안하면 순간이동 현상을 목격
        currPos = transform.position;
        currRot = transform.rotation;
    }

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        sharkCtrl = FindObjectOfType<SharkCtrl>();

        speed = 3.0f;
        jumpSpeed = 6.0f;
        gravity = 20.0f;

        MoveDir = Vector3.zero;
    }

    void Update()
    {
        if(pv.isMine)
        {
            if (!inventoryOn && !EscapeOn && !chatingOn)
            {
                #region 캐릭터이동
                // 캐릭터 이동 //

                // 화면 회전
                MouseRotation();

                // 현재 캐릭터가 땅에 있는가? + 수영모드가 아닐때(추가예정)
                if (controller.isGrounded)
                {
                    // 위, 아래 움직임 셋팅. 
                    MoveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

                    // 벡터를 로컬 좌표계 기준에서 월드 좌표계 기준으로 변환한다.
                    MoveDir = transform.TransformDirection(MoveDir);

                    // 스피드 증가.
                    MoveDir *= speed;

                    // 애니메이션
                    if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
                    {
                        ani.SetBool("Run", true);
                    }
                    else
                    {
                        ani.SetBool("Run", false);
                    }

                    // 캐릭터 점프
                    if (Input.GetButton("Jump"))
                    {
                        MoveDir.y = jumpSpeed;
                        ani.SetTrigger("Jump");
                    }
                }

                if (swimMode == false)  // 수영모드아닐때
                {
                    // 캐릭터에 중력 적용.
                    MoveDir.y -= gravity * Time.deltaTime;

                    // 캐릭터 움직임.
                    controller.Move(MoveDir * Time.deltaTime);
                }
                else    // 수영모드일때
                {
                    // 위, 아래 움직임 셋팅. 
                    MoveDir = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump") * 1.6f, Input.GetAxis("Vertical"));

                    // 애니메이션
                    if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
                    {
                        ani.SetBool("Swimming", true);
                    }
                    else
                    {
                        ani.SetBool("Swimming", false);
                    }

                    // 벡터를 로컬 좌표계 기준에서 월드 좌표계 기준으로 변환한다.
                    MoveDir = transform.TransformDirection(MoveDir);

                    // 스피드 증가.(물속에서 수영속도 조절 필요시 변수추가해서 따로 하기)
                    MoveDir *= speed;

                    controller.Move(MoveDir * Time.deltaTime);
                }

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                #endregion

                #region 손에 들고있는 물건 사용
                // 손에 들고있는 물건 사용하기 //
                // 기능있는 물건만 태그달아서 구현하면 될듯
                if (rightHandle.transform.childCount > 0)                       // 손에 무언가를 들고있다면
                {
                    if(Input.GetKeyDown("g") && rightHandle.transform.GetChild(0).gameObject.tag != "Hook")       // 버리기(훅은 못버리게 콜라이더 2개라서 2배이벤트됨;;)
                    {
                        PhotonNetwork.Destroy(rightHandle.transform.GetChild(0).gameObject);
                        inventoryManager.UseSelectedItem();
                        ThrowItem(stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID);
                        hammerMode = false;
                        constructMode = false;      // 혹시나를 위한 해머모드 건설모드 false
                        ani.SetBool("ConstructMode", false);
                    }

                    if (Input.GetMouseButtonDown(0))                            // 마우스 왼쪽버튼 다운시
                    {
                        // 손에 들고있는 물건이 Hook이고 수영모드가 아니라면
                        if (rightHandle.transform.GetChild(0).tag == "Hook" && swimMode == false && hookThrow == false)
                        {
                            ani.SetTrigger("Throw");
                            StartCoroutine(HookThrowGage());
                        }
                        else if(hammerMode && swimMode == false && hookThrow == false && inventoryManager.buildingRecipes[hammerScript.selectObject].CanBuild(inventoryManager))
                        {
                                hammerScript.HammerClick();
                                inventoryManager.Build(inventoryManager.buildingRecipes[hammerScript.selectObject]);                            
                        }
                    }
                }
                else  // 손에 아무것도 들고있지 않고
                {
                    if (stuffs.transform.GetChild(swapNum).childCount > 0)     // 건설 오브젝트들 못버리게
                    {
                        if (Input.GetKeyDown("g"))       // 버리기
                        {
                            inventoryManager.UseSelectedItem();
                            ThrowItem(stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID);
                            hammerMode = false;
                            constructMode = false;      // 혹시나를 위한 해머모드 건설모드 false
                            ani.SetBool("ConstructMode", false);
                        }
                    }

                    if (constructMode)   // 건축모드일때
                    { 
                        if(Input.GetMouseButtonDown(0) && constructScript.constuctPossibility)     // 마우스 왼쪽버튼이 눌리고 건설가능일때
                        {
                            StartCoroutine(ConstructClick(stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID));
                            inventoryManager.UseSelectedItem();
                            constructMode = false;
                            // 건축가능한 애들은 겹쳐서 소지품창에 넣을 수 없음. 건축하고나면 빈칸이 될거임(건축모드 끄는게 맞음)
                        }
                    }
                }
                //else  // SpearAttack 테스트를 위한 손에 아무것도 없을때 애니메이션
                //{
                //    if (Input.GetMouseButtonDown(0))                            // 마우스 왼쪽버튼 다운시
                //    {
                //        ani.SetTrigger("SpearAttack");
                //    }
                //}

                if (hookThrow == true)                                           // 훅이 던져진 상태
                {
                    StartCoroutine(PullHook());                                 // 훅 당기기 함수 호출
                }
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                #endregion

                #region 전방 일정거리 물체감지 및 상호작용
                //if (Physics.SphereCastAll(firePos.transform.position, transform.lossyScale.x / 1f, Camera.main.transform.forward, out hitInfo, 3f))  
                RaycastHit[] hits = Physics.SphereCastAll(firePos.transform.position, transform.lossyScale.x / 5f, Camera.main.transform.forward ,3f); // 2미터 앞에 스페어캐스트에 걸리는게 있다면(범위 추후 조정필요할듯)


                // 레이케스트에 감지된것이 없어도 작동
                if (rightHandle.transform.childCount > 0)        // 손에 뭔가를 들고 있을 때만 상호작용
                {
                    if (rightHandle.transform.GetChild(0).gameObject.tag == "Spear_wood")
                    {
                        if (!isAnimating)
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                isAnimating = true;
                                ani.SetTrigger("SpearAttack");

                                for (int i = 0; i < hits.Length; i++)   // 여기만 레이케스트에 감지된것이 있으면 작동
                                {
                                    if (hits[i].collider.tag == "Enemy")
                                    {
                                        // 상어한테 데미지주는 소스
                                        sharkCtrl.Damaged();
                                    }
                                }
                            }
                        }
                    }
                }
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////                    

                for (int j = 0; j < hits.Length; j++)   // 레이케스트에 감지된것이 있을때만 작동함
                {
                    if (hits[j].collider.tag != "Ground"&& hits[j].collider.tag != "Player")  // 2미터 앞에 스페어캐스트에 걸리는게 있다면(범위 추후 조정필요할듯)
                    {
                        // 상호작용 가능한 오브젝트들 UI띄우기
                        // InteractionObject를 제외하고는 바로 아래에 주울 수 있는 오브젝트 상호작용 if문 조건 따라가야함
                        if (hits[j].collider.tag == "InteractionObject" || hits[j].collider.tag == "Object" || hits[j].collider.tag == "Potato")
                        {
                            for (int i = 0; i < inventoryManager.itemList.Count; i++)
                            {
                                if (inventoryManager.itemList[i].ID == hits[j].transform.GetComponent<PhotonObject>().objectNum)
                                {
                                    Debug.Log(inventoryManager.itemList[i].Name);
                                }
                            }
                        }

                        #region 주울 수 있는 오브젝트 상호작용
                        if (hits[j].collider.tag == "Object" || hits[j].collider.tag == "Potato")      // 주울 수 있는 애들에? 상호작용이 가능한 애들에? Object 태그달기
                        {
                            if (Input.GetKeyDown("e"))
                            {
                                if (hits[j].transform.GetComponent<WaterMoveObject>() != null)
                                {
                                    hits[j].transform.GetComponent<WaterMoveObject>().enabled = false;
                                }

                                if (hits[j].transform.GetComponent<WaterObject>() != null)
                                {
                                    hits[j].transform.GetComponent<WaterObject>().enabled = false;
                                }

                                // 해당오브젝트의 ID따서 UI쪽 함수에 전달
                                inventoryManager.AddItem(hits[j].transform.GetComponent<PhotonObject>().objectNum);

                                //PhotonNetwork.Destroy(hits[j].collider.gameObject);   // 추후에는 포톤디스트로이 해야한다
                                pv.RPC("PhotonObjectDestroyMaster", PhotonTargets.AllBuffered, hits[j].transform.GetComponent<PhotonView>().viewID);
                            }
                        }
                        #endregion

                        #region 그릴 상호작용
                        if (rightHandle.transform.childCount > 0)        // 손에 뭔가를 들고 있을 때만 상호작용
                        {
                            // 레이케스트에 걸린게 상호작용가능오브젝트고 14(그릴)이고 손에든게 감자일때
                            if (hits[j].collider.tag == "InteractionObject" && hits[j].transform.GetComponent<PhotonObject>().objectNum == 14 && rightHandle.transform.GetChild(0).gameObject.tag == "Potato")
                            {
                                if (Input.GetKeyDown("f"))
                                {
                                    hits[j].transform.GetComponent<InteractionObject>().interaction = true;
                                    PhotonNetwork.Destroy(rightHandle.transform.GetChild(0).gameObject);
                                }
                            }
                        }
                        #endregion

                        #region 컵 & 정수기 상호작용
                        if (rightHandle.transform.childCount > 0)        // 손에 뭔가를 들고 있을 때만 상호작용
                        {
                            // 레이케스트에 걸린게 상호작용가능오브젝트고 6(정수기)이고 손에든게 컵일때
                            if (hits[j].collider.tag == "InteractionObject" && hits[j].transform.GetComponent<PhotonObject>().objectNum == 6 && rightHandle.transform.GetChild(0).gameObject.tag == "Cup")
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    hits[j].transform.GetComponent<InteractionObject>().interaction = true;
                                }
                            }

                            if(hits[j].collider.tag == "Sea" && rightHandle.transform.GetChild(0).gameObject.tag == "Cup")
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    gm.thirsty -= 0.1f;
                                }
                            }
                        }
                        #endregion

                        #region 도끼 상호작용
                        // 도끼 관련 상호작용
                        if (rightHandle.transform.childCount > 0)        // 손에 뭔가를 들고 있을 때만 상호작용
                        {
                            if (rightHandle.transform.GetChild(0).gameObject.tag == "Axe" && hits[j].collider.tag == "InteractionObject" && hits[j].transform.gameObject.layer == 19 && !swimMode)  // 도끼는 물에 있을때는 못쓰게
                            {
                                if (!isAnimating)
                                {
                                    if (Input.GetMouseButtonDown(0))
                                    {
                                        isAnimating = true;
                                        ani.SetTrigger("AxeAttack");
                                        hits[j].transform.GetComponent<InteractionObject>().interaction = true;
                                    }
                                }

                            }
                            else if (rightHandle.transform.GetChild(0).gameObject.tag == "Axe" && (hits[j].transform.gameObject.layer == 12 || hits[j].transform.gameObject.layer == 8) && !swimMode)  // 해머오브젝트, 건설오브젝트일때
                            {
                                if (!isAnimating)
                                {
                                    if (Input.GetMouseButtonDown(0))
                                    {
                                        isAnimating = true;
                                        ani.SetTrigger("AxeAttack");
                                        pv.RPC("PhotonObjectDestroyMaster", PhotonTargets.AllBuffered, hits[j].transform.GetComponent<PhotonView>().viewID);    // 일단은 바로 부숴지게
                                    }
                                }
                            }
                        }
                        #endregion

                        
                    }
                }
                
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                #endregion
            }
            else  // 인벤토리 조작 시
            {
                if(stuffs.transform.GetChild(swapNum).childCount == 0)  // 현재 손에 들고있는 소지품칸이 비워지면
                {
                    if(rightHandle.transform.childCount > 0)
                    {
                        PhotonNetwork.Destroy(rightHandle.transform.GetChild(0).gameObject);    // 손에 들고 있는 물건 destory
                    }
                }
            }

            // 얘는 인벤토리 온오프 상관없이 발동돼야함
            #region 손에 들고있는 물건변경    
            if (!hookThrow)
            {
                if (Input.GetKeyDown("1"))
                {
                    swapNum = 0;
                    ChangeRightHand(swapNum);
                }
                else if (Input.GetKeyDown("2"))
                {
                    swapNum = 1;
                    ChangeRightHand(swapNum);
                }
                else if (Input.GetKeyDown("3"))
                {
                    swapNum = 2;
                    ChangeRightHand(swapNum);
                }
                else if (Input.GetKeyDown("4"))
                {
                    swapNum = 3;
                    ChangeRightHand(swapNum);
                }
                else if (Input.GetKeyDown("5"))
                {
                    swapNum = 4;
                    ChangeRightHand(swapNum);
                }
                else if (Input.GetKeyDown("6"))
                {
                    swapNum = 5;
                    ChangeRightHand(swapNum);
                }
                else if (Input.GetKeyDown("7"))
                {
                    swapNum = 6;
                    ChangeRightHand(swapNum);
                }
                else if (Input.GetKeyDown("8"))
                {
                    swapNum = 7;
                    ChangeRightHand(swapNum);
                }
                else if (Input.GetKeyDown("9"))
                {
                    swapNum = 8;
                    ChangeRightHand(swapNum);
                }
                else if (Input.GetKeyDown("0"))
                {
                    swapNum = 9;
                    ChangeRightHand(swapNum);
                }
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            #endregion  

            // 인벤토리 온오프에 따른 플레이어 컨트롤 Lock
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                inventoryOn = !inventoryOn;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EscapeOn = !EscapeOn;
            }

            //if(Input.GetKeyDown(KeyCode.Return))
            //{
            //    chatingOn = !chatingOn;
            //}
        }
        else  // 아바타
        {
            //원격 플레이어의 아바타를 수신받은 위치까지 부드럽게 이동시키자
            transform.position = Vector3.Lerp(transform.position, currPos, Time.deltaTime * 3f);
            //원격 플레이어의 아바타를 수신받은 각도만큼 부드럽게 회전시키자
            transform.rotation = Quaternion.Slerp(transform.rotation, currRot, Time.deltaTime * 3f);
        }
        
    }

    // 마우스로 화면회전
    void MouseRotation()
    {
        // 좌우로 움직인 마우스의 이동량 * 속도에 따라 카메라가 좌우로 회전할 양 계산
        float yRotateSize = Input.GetAxis("Mouse X") * turnSpeed;
        // 현재 y축 회전값에 더한 새로운 회전각도 계산
        float yRotate = transform.eulerAngles.y + yRotateSize;

        // 위아래로 움직인 마우스의 이동량 * 속도에 따라 카메라가 회전할 양 계산(하늘, 바닥을 바라보는 동작)
        float xRotateSize = -Input.GetAxis("Mouse Y") * turnSpeed;
        // 위아래 회전량을 더해주지만 -45도 ~ 80도로 제한 (-60:하늘방향, 40:바닥방향)
        // Clamp 는 값의 범위를 제한하는 함수
        xRotate = Mathf.Clamp(xRotate + xRotateSize, -70, 70);

        // 카메라 회전량을 카메라에 반영(X, Y축만 회전)
        transform.eulerAngles = new Vector3(0, yRotate, 0);
        // 마우스 Y축이동시 캐릭터의 몸은 가만히 있는 상태로 메인카메라만 위아래로 회전시키기 위함
        Camera.main.transform.localEulerAngles = new Vector3(xRotate, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z);
    }

    // 물에서 들어오고 나갈때 애니메이션변경 및 모드변경
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Sea")
        {
            ani.SetBool("Run", false);
            swimMode = true;
            ani.SetBool("Swim", true);
            gravity = 0;
        }
    }
    // 물에서 들어오고 나갈때 애니메이션변경 및 모드변경
    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Sea")
        {
            
            ani.SetBool("Swimming", false);
            swimMode = false;
            ani.SetBool("Swim", false);
            gravity = 20;
        }
    }

    IEnumerator PullHook()
    {
        if (Input.GetMouseButton(0))                            // 마우스 왼쪽버튼 다운시
        {
            rightHandleSave.transform.position = Vector3.MoveTowards(rightHandleSave.transform.position, transform.position, 0.03f);    // 캐릭터 방향으로 훅 당겨오기

            if (Vector3.Distance(rightHandleSave.transform.position, transform.position) < 1)                                           // 훅과 캐릭터의 거리가 1 미만이면
            {
                rightHandleSave.transform.parent = rightHandle.transform;                                                               // 훅을 rightHandle 자식으로
                rightHandleSave.GetComponent<Rigidbody>().isKinematic = true;                                                           // 훅의 물리 잠그기

                if (rightHandle.transform.GetChild(0).tag == "Hook")                                                                    // rightHandle에 Hook이 다시 들어오면
                {
                    yield return new WaitForSeconds(1f);                                                                                // 약 1초뒤
                    rightHandleSave.transform.localPosition = Vector3.zero;                                                             // 훅 Transform 초기화
                    rightHandleSave.transform.localRotation = Quaternion.identity;
                    rightHandleSave.transform.localScale = new Vector3(1f, 1f, 1f);
                    hookThrow = false;                                                                                                  // hookThrow false
                }
            }
        }
        yield return null;
    }

    IEnumerator HookThrowGage()
    {
        while(Input.GetMouseButton(0))
        {
            throwGage += 0.5f;                          // throwForce 상승 폭과 최대값? 은 추후 조정(이 변수를 UI 게이지 조절로 사용하면 될듯)
            if(throwGage >= 15)
            {
                throwGage = 15;
            }
            Debug.Log(throwGage);
            //eventCS.hookThrowGage = throwGage;
            yield return new WaitForSeconds(0.1f);
        }

        // 마우스 다운이 끝난 후 Hook 던지는 부분
        ani.speed = 1f;                                             // 애니메이션에서 이벤트함수로 처리한 애니메이션 스탑 풀기

        Vector3 vec = Camera.main.transform.forward * throwGage;   // 던지는 방향은 메인카메라 기준으로하고 게이지값 곱하는식으로 + 훅 던질때는 못움직이게 해야하나?

        //Destroy(rightHandle.transform.GetChild(0).gameObject);
        //hook = Instantiate(hookPrefab, new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z + 1f), Quaternion.identity, transform);

        hook = rightHandle.transform.GetChild(0).gameObject;    // 둘중에 뭘로 할지 선택하기(끌고오는 부분에서 결정될듯)
        hook.transform.parent = null;

        hook.GetComponent<Rigidbody>().isKinematic = false;
        hook.GetComponent<Rigidbody>().AddForce(vec, ForceMode.Impulse);
        hookThrow = true;

        throwGage = 0f;                                         // 던지는 gage포스 초기화

        yield return null;
    }

    IEnumerator ConstructClick(int ID)
    {
        ani.SetTrigger("ConstructClick");
        // 여기에다가 건축관련 함수 호출하면 될듯(홀로그램은 저쪽에서 캐릭터 constructMode 변수 체크식으로 해서 활성화시키면 될듯)
        constructScript.ConstructClick(ID);

        yield return new WaitForSeconds(0.5f);

        ani.SetBool("ConstructMode", false);
        constructMode = false;
    }

    void ChangeRightHand(int swapNum)
    {
        //Debug.Log(stuffs.transform.GetChild(swapNum).childCount);

        if (stuffs.transform.GetChild(swapNum).childCount > 0)                            // 여기 UI화(비어있는지)
        {
            if (rightHandle.transform.childCount > 0)                    // 손에 이미 뭔가 들고있고
            {
                if (rightHandle.transform.GetChild(0).GetComponent<PhotonObject>().objectNum == stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID)    // 들려는물건과 들고있는게 같은거라면 return (태그로 비교하는게 맞나...?)
                {
                    return;
                }
                else
                {
                    ani.SetBool("ConstructMode", false);
                    constructMode = false;
                    hammerMode = false;

                    PhotonNetwork.Destroy(rightHandle.transform.GetChild(0).gameObject);
                    //Destroy(rightHandle.transform.GetChild(0).gameObject);

                    // 레이어 10번 훅
                    if (
                        Resources.Load<GameObject>(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).layer == 9 ||
                        Resources.Load<GameObject>(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).layer == 10 ||
                        Resources.Load<GameObject>(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).layer == 11 ||
                        Resources.Load<GameObject>(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).layer == 13 ||
                        Resources.Load<GameObject>(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).layer == 14
                        ) // 기능있는 물건들만 손에 들게하기(계속 추가), 이것도 레이어번호 매기는게 나을듯
                    {
                        //rightHandleSave = Instantiate(stuffs[0], rightHandle.transform);    // 손에 드는 물건 Photon동기화 시키려면 추후에 넘버링해서 Resource폴더에서 찾는 방법 있어야함...(ㅈ대따...)

                        rightHandleSave = PhotonNetwork.Instantiate(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID], transform.position, Quaternion.identity, 0);
                        rightHandleSave.transform.parent = rightHandle.transform;
                        rightHandleSave.transform.localPosition = Vector3.zero;
                        rightHandleSave.transform.localRotation = Quaternion.identity;

                        if(Resources.Load<GameObject>(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).layer == 11)     // 해머   // 특정애들 걸러내보기
                        {
                            hammerMode = true;
                        }
                        if (Resources.Load<GameObject>(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).layer == 13)    // 도끼
                        {
                            rightHandleSave.transform.localRotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
                        }
                        if(Resources.Load<GameObject>(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).tag == "Cup")    // 컵
                        {
                            rightHandleSave.transform.localPosition = new Vector3(-0.05f, 0, 0);
                            rightHandleSave.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                        }


                        if (!ani.GetBool("Run"))
                        {
                            ani.SetTrigger("Swap");
                        }
                    }
                    else if (Resources.Load<GameObject>(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).layer == 8)    // 건축오브젝트 레이어번호 확인해서 수정하기
                    {
                        //PhotonNetwork.Destroy(rightHandle.transform.GetChild(0).gameObject);
                        //Destroy(rightHandle.transform.GetChild(0).gameObject);
                        ani.SetBool("ConstructMode", true);
                        constructMode = true;
                    }
                }
            }
            else  //손에 아무것도 들려있지 않을때
            {
                ani.SetBool("ConstructMode", false);
                constructMode = false;
                hammerMode = false;

                if (
                        Resources.Load<GameObject>(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).layer == 9 ||
                        Resources.Load<GameObject>(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).layer == 10 ||
                        Resources.Load<GameObject>(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).layer == 11 ||
                        Resources.Load<GameObject>(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).layer == 13 ||
                        Resources.Load<GameObject>(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).layer == 14
                        ) // 기능있는 물건들만 손에 들게하기(계속 추가), 이것도 레이어번호 매기는게 나을듯
                {
                    //rightHandleSave = Instantiate(stuffs[0], rightHandle.transform);    // 손에 드는 물건 Photon동기화 시키려면 추후에 넘버링해서 Resource폴더에서 찾는 방법 있어야함...(ㅈ대따...)

                    rightHandleSave = PhotonNetwork.Instantiate(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID], transform.position, Quaternion.identity, 0);
                    rightHandleSave.transform.parent = rightHandle.transform;
                    rightHandleSave.transform.localPosition = Vector3.zero;
                    rightHandleSave.transform.localRotation = Quaternion.identity;

                    if (Resources.Load<GameObject>(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).layer == 11)   // 해머   // 특정애들 걸러내보기
                    {
                        hammerMode = true;
                    }
                    if (Resources.Load<GameObject>(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).layer == 13)   // 도끼
                    {
                        rightHandleSave.transform.localRotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
                    }
                    if (Resources.Load<GameObject>(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).tag == "Cup")    // 컵
                    {
                        rightHandleSave.transform.localPosition = new Vector3(-0.05f, 0, 0);
                        rightHandleSave.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                    }


                    if (!ani.GetBool("Run"))
                    {
                        ani.SetTrigger("Swap");
                    }
                }
                else if (Resources.Load<GameObject>(photonMapping[stuffs.transform.GetChild(swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).layer == 8)    // 건축오브젝트 레이어번호 확인해서 수정하기
                {
                    //PhotonNetwork.Destroy(rightHandle.transform.GetChild(0).gameObject);
                    //Destroy(rightHandle.transform.GetChild(0).gameObject);
                    ani.SetBool("ConstructMode", true);
                    constructMode = true;
                }
            }
        }
        else   // 선택한 소지품 칸이 비어있을때(이것도 스왑모션을 넣어야 할까...?)
        {
            ani.SetBool("ConstructMode", false);
            constructMode = false;
            hammerMode = false;

            if (rightHandle.transform.childCount > 0)                    // 손에 이미 뭔가 들고있다면
            {
                PhotonNetwork.Destroy(rightHandle.transform.GetChild(0).gameObject);
                //Destroy(rightHandle.transform.GetChild(0).gameObject);
                return;
            }
            else                                                         // 손에 아무것도 들고있지 않다면
            {
                //UI에서 해당 소지품칸 활성화만 시키고 (코드추가)
                return;
            }
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //로컬 플레이어의 위치 정보를 송신
        if (stream.isWriting)
        {
            //박싱
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else //원격 플레이어의 위치 정보를 수신
        {
            //언박싱
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void PhotonObjectDestroyMaster(int viewID)
    {
        // ID를 사용하여 게임 오브젝트 찾기
        GameObject obj = PhotonView.Find(viewID).gameObject;

        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.Destroy(obj);
        }
    }

    // 아이템 버리기
    public void ThrowItem(int id)
    {
        Vector3 createPos = transform.position + new Vector3(this.transform.forward.x, this.transform.forward.y + 1.5f, this.transform.forward.z);
        pv.RPC("PhotonObjectCreateMaster", PhotonTargets.AllBuffered, photonMapping[id], createPos, transform.rotation);
    }

    [PunRPC]
    void PhotonObjectCreateMaster(string name, Vector3 pos, Quaternion rot)
    {
        if (PhotonNetwork.isMasterClient)
        {
            GameObject createObject = PhotonNetwork.InstantiateSceneObject(name, pos, rot, 0, null);
            createObject.GetComponent<Rigidbody>().isKinematic = false;
            createObject.GetComponent<Rigidbody>().useGravity = true;
            if (createObject.GetComponent<WaterMoveObject>() != null)
            {
                createObject.GetComponent<WaterMoveObject>().enabled = false;
            }
            if (createObject.GetComponent<WaterObject>() != null)
            {
                createObject.GetComponent<WaterObject>().enabled = false;
            }
        }
    }

    void OnDrawGizmos()     //디버그때만
    {
        Gizmos.color = Color.red;
        float sphereScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

        RaycastHit[] hits = Physics.SphereCastAll(firePos.transform.position, transform.lossyScale.x / 5f, Camera.main.transform.forward, 3f);
        for(int i = 0; i<hits.Length; i++)
        {
            // 함수 파라미터 : 현재 위치, Sphere의 크기(x,y,z 중 가장 큰 값이 크기가 됨), Ray의 방향, RaycastHit 결과, Sphere의 회전값, SphereCast를 진행할 거리
            //if (true == Physics.SphereCast(firePos.transform.position, sphereScale / 1f, Camera.main.transform.forward, out RaycastHit hit, 3f) && hit.collider.tag != "Ground")
            if(hits[i].collider.tag != "Ground" && hits[i].collider.tag != "Player")
            {
                // Hit된 지점까지 ray를 그려준다.
                Gizmos.DrawRay(firePos.transform.position, Camera.main.transform.forward * hits[i].distance);

                // Hit된 지점에 Sphere를 그려준다.
                Gizmos.DrawWireSphere(firePos.transform.position + Camera.main.transform.forward * hits[i].distance, sphereScale / 5f);
            }
            else
            {
                // Hit가 되지 않았으면 최대 검출 거리로 ray를 그려준다.
                Gizmos.DrawRay(firePos.transform.position, Camera.main.transform.forward * 3f);
            }
        }
        
    }
}
