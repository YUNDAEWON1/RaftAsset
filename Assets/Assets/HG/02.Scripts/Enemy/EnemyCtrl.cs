#define CBT_MODE
#define RELEASE_MODE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//네비게이션을 위한 네임스페이스 추가
using UnityEngine.AI;

// 간단하게 쓰기위해...
using Rand = UnityEngine.Random;
//공부용 네임스페이스 추가 본 소스랑 상관없다 나중에 지워주자
//using UnityEngine.iOS;

/*
 * 클래스는 [System.Serializable] Attribute를 명시 하여야
 * Inspector 뷰에 노출 
 */
[System.Serializable]
//애니메이션 클립을 저장할 클래스 
public class Anim
{
    public AnimationClip idle1;
    public AnimationClip idle2;
    public AnimationClip idle3;
    public AnimationClip idle4;
    public AnimationClip move;
    public AnimationClip surprise;
    public AnimationClip attack1;
    public AnimationClip attack2;
    public AnimationClip attack3;
    public AnimationClip attack4;
    public AnimationClip hit1;
    public AnimationClip hit2;
    public AnimationClip eat;
    public AnimationClip sleep;
    public AnimationClip die;
}


//필요한 컴포넌트를 위한 어트리뷰트 선언 
[RequireComponent(typeof(AudioSource))]

//다른 게임오브젝트에 현재 제작한 컴포넌트를 연결하고 싶을때 이 어트리뷰트를 사용하면 메뉴에 => Component에 추가된다.
[AddComponentMenu("EnemyCtrl/Follow EnemyCtrl")]

public class EnemyCtrl : MonoBehaviour
{

    //문자열로 컴포넌트 설명을 넣는다... 최종파일 뽑을때 비활성 시키자.
    //장문의 문자열을 위한 어트리뷰트 선언
    [Multiline(7)]
    public string Ex = "안녕하세요\n이 컴포넌트는...";

    //사용자의 메모를 제공하기 위한 어트리뷰트 선언 (인자로 라인수의 최소, 최대 전달)
    [TextArea(7, 11)]
    public string Memo = "";

    //변수들의 간격을 위한 어트리뷰트 선언(보기 좋다)
    [Space(30)]

    //인스펙터의 헤더의 표현을 위한 어트리뷰트 선언
    [Header("ANIMATION")]
    //인스펙터뷰에 노출시킬 Anim 클래스 변수 
    public Anim anims;
    //하위에 있는 모델의 Animation 컴포넌트에 접근하기 위한 레퍼런스
    private Animation _anim;
    //애니메이션 상태 저장 
    AnimationState animState;
    //애니메이션 셀렉트(랜덤한 연출 )
    private float randAnimTime;
    private int randAnim;

    //각종 연결 레퍼런스 선언 및 변수 선언

    //NavMeshAgent 연결 레퍼런스
    private NavMeshAgent myTraceAgent;

    //자신과 타겟 Transform 참조 변수  
    private Transform myTr;
    private Transform traceTarget;

    //추적을 위한 변수
    private bool traceObject;
    private bool traceAttack;

    // 로밍을위한 시간 변수
    private float hungryTime;
    private float nonHungryTime;

    //추적 대상 거리체크 변수 
    float dist1;
    float dist2;

    //플레이어를 찾기 위한 배열 
    private GameObject[] players;
    private Transform playerTarget;

    //추적 대상인 베이스 캠프
    private GameObject[] baseAll;
    private Transform baseTarget;

    //로밍 장소 
    private Transform[] roamingCheckPoints;
    //로밍 장소 중복해서 안가게...
    private int roamingRandcheckPos;
    //로밍 타겟
    private Transform roamingTarget;

    // public 멤버 인스펙터에 노출을 막는 어트리뷰트
    // 인스펙터에 노출은 막고 외부 노출은 원하는 경우 사용
    [HideInInspector]
    //죽었는지 상태변수 
    public bool isDie;

    /* [System.NonSerialized] 와 [HideInInspector]는 기능은 같지만 
     * [HideInInspector]는 인스펙터에서 수정한 값을 그대로 가져가지만
     * [System.NonSerialized]는 인스펙터에서 값을 저장하여도 실행시 디폴트 값이 나오는걸 볼 수있다.
     */

    // Enemy의 현재 상태정보를 위한 Enum 자료형 선언  
    public enum MODE_STATE { idle = 1, move, surprise, trace, attack, hit, eat, sleep, die };

    // Enemy의 종류 정보를 위한 Enum 자료형 선언  
    public enum MODE_KIND { enemy1 = 1, enemy2, enemyBoss };

    //변수들의 간격을 위한 어트리뷰트 선언(보기 좋다)
    [Space(20)]
    //인스펙터의 헤더의 표현을 위한 어트리뷰트 선언
    [Header("STATE")]
    //Enemy의 상태 셋팅
    public MODE_STATE enemyMode = MODE_STATE.idle;

    //인스펙터의 헤더의 표현을 위한 어트리뷰트 선언
    [Header("SETTING")]
    //Enemy의 종류 셋팅
    public MODE_KIND enemyKind = MODE_KIND.enemy1;

    //변수들의 간격을 위한 어트리뷰트 선언(보기 좋다)
    [Space(10)]
    //인스펙터의 헤더의 표현을 위한 어트리뷰트 선언
    [Header("몬스터 인공지능")]

    //변수들의 간격을 위한 어트리뷰트 선언(보기 좋다)
    [Space(5)]

    //변수에 팁을 달아줄 수  있다.(인스펙터에서 확인)
    [Tooltip("몬스터의 HP")]
    //Enemy의 HP 셋팅 
    [Range(0, 1000)] public int hp = 100;

    //변수에 팁을 달아줄 수  있다.(인스펙터에서 확인)
    [Tooltip("몬스터의 속도")]
    //Enemy의 속도 셋팅, [SerializeField] 는 [HideInInspector] 와 반대 속성
    [Range(5f, 10f)] public float speed = 7.5f;

    //거리에 따른 상태 체크 변수 
    [Tooltip("몬스터 발견거리!!!")]
    [Range(35f, 39f)] [SerializeField] float findDist = 37.5f;
    [Tooltip("몬스터 추적거리!!!")]
    [Range(30f, 34f)] [SerializeField] float traceDist = 32.5f;
    [Tooltip("몬스터 공격거리!!!")]
    [Range(15f, 18f)] [SerializeField] float attackDist = 17.0f;
    [Tooltip("몬스터 로밍 시간!!!")]
    [Range(30f, 50f)] [SerializeField] float hungryTimeSet = 40f;
    [Tooltip("몬스터 로밍 대기시간!!!")]
    [Range(30f, 50f)] [SerializeField] float nonHungryTimeSet = 40f;

    //인공지능 부여 변수(테스트 변수로 나중에 private 선언)
    [Header("TEST")]
    // [SerializeField] 는 [HideInInspector] 와 반대 속성
    [SerializeField] private bool isHit;
    [SerializeField] private bool hungry;
    [SerializeField] private bool sleep;
    private bool nonHungry;
    private float isHitTime;

    // 포톤 추가///////////////////////////////////////////////////////////////////////

    //참조할 컴포넌트를 할당할 레퍼런스 (미리 할당하는게 좋음)
    Rigidbody myRbody;

    //PhotonView 컴포넌트를 할당할 레퍼런스 
    PhotonView pv = null;

    //위치 정보를 송수신할 때 사용할 변수 선언 및 초기값 설정 
    Vector3 currPos = Vector3.zero;
    Quaternion currRot = Quaternion.identity;

    // 애니메이션 동기화를 위한 변수
    // RPC로 처리해도 됨...선택은 상황에 따라서...
    int net_Aim = 0;

    void Awake()
    {
        //이 부분은 단순 공부용/////////////////////////////////////////////////////////////////

        /*
            -전 처리기

        유니티에선 #define 지시자가 없다 따라서 두 가지 방법으로 추가해야한다.
        1) PlayerSettings => OtherSettings => Scripting Define Symbols
        2) 매번 생성을 반복하면 힘드니깐 파일로 보관하는 경우 및에 참조...
        3) 각 스크립트 최 상위에 #define 지시어로 선언(해당 컴포넌트만 참조)


        #define AAA 를 전역으로 사용하고 싶다면 메모장에 -define:AAA 를 적고
        #define AAA 와 #define BBB 를 모두 사용하고 싶다면 -define:AAA;BBB 와 같은 형식으로 
        사용.(유니티 에디터에서도 사용법은 동일) 
        모두 작성했으면 유니티 프로젝트에 Assets 폴더에 

        (과거 버전)
        Api Compatibility Level이  .net 2.0 일 경우엔 gmcs.rps 
        .net 2.0 subset 일 경우엔 smcs.rps 로 파일저장
        (현재 버전)
        mcs.rps로 통일

        주의) mcs.rps 파일을 수정시엔 해당 define 을 사용하는 스크립트를 재 컴파일 해줘야 됨..


          
        
        
        // 사용 방법

        #if UNITY_EDITOR

               유니티 에디터 상태에서만 동작하는 스크립트 로직

        #endif



        #if UNITY_IOS

             빌드 타켓이 아이폰일 때 동작하는 스크립트 로직 

        #endif



        #if UNITY_ANDROID

             빌드 타켓이 안드로이드일 때 동작하는 스크립트 로직 

        #endif

        #지시어로 다음과 같이 선택적 스크립트 실행
        #if CBT_MODE
        int HP = 10;

        #elif RELEASE_MODE
        int HP = 100;
        #endif
        */

#if AAA
        Debug.Log("AAA");
#endif

#if BBB
        Debug.Log("BBB");  
#endif

#if UNITY_5_3_2
        Debug.Log("CCC");
#endif

#if UNITY_2017_3_1
        //Debug.Log("DDD");
#endif


#if UNITY_EDITOR
        //Debug.Log("EDITOR");
#endif

#if UNITY_IPHONE
        Debug.Log("UNITY_IPHONE");
#endif

#if UNITY_ANDROID
        //Debug.Log("UNITY_ANDROID");
#endif

#if UNITY_IPHONE
        // DeviceGeneration는 안드로이드는 지원안하고 안드로이드는 필요도 없다.
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {

            switch (Device.generation)
            {
                case DeviceGeneration.iPodTouch5Gen:
                    //처리
                    break;
                case DeviceGeneration.iPadMini1Gen:
                    //처리
                    break;
                case DeviceGeneration.iPodTouch3Gen:
                    //처리
                    break;
            }
        }
        //여기까지 공부용이니 차후 삭제////////////////////////////////////////////////////////////////////
#endif

        // 포톤 추가
        // 만약 Start 함수에서 초기화 했다면 Start 함수를 Awake 함수로 변경 ( 기존  Start 함수에서 처리 할 경우 
        // myTr = GetComponent<Transform>(); 전에 OnPhotonSerializeView 콜백 함수가 먼저 호출될 경우 Null Reference 오류 발생)

        //레퍼런스 할당 
        myTraceAgent = GetComponent<NavMeshAgent>();

        //자신의 자식에 있는 Animation 컴포넌트를 찾아와 레퍼런스에 할당 
        _anim = GetComponentInChildren<Animation>();

        // 포톤 추가
        // 네트워크 동기화를 위한 애니메이션
        net_Aim = 0;

        //자기 자신의 Transform 연결
        myTr = GetComponent<Transform>();
        //Hierarchy의 모든 Base를 찾음
        baseAll = GameObject.FindGameObjectsWithTag("Base");


        //로밍 위치 얻기
        roamingCheckPoints = GameObject.Find("RoamingPoint").GetComponentsInChildren<Transform>();

        //포톤 추가////////////////////////////////////////////////////////////

        //컴포넌트를 할당 
        myRbody = GetComponent<Rigidbody>();

        //PhotonView 컴포넌트 할당 
        pv = GetComponent<PhotonView>();

        /*
         * PhotonView 컴포넌트의 Observe 속성에 특정 스크립트를 연결하면
         * PhotonView 컴포넌트는 해당 스크립트에 있는 OnPhotonSerializeView 콜백 함수를 통해
         * 데이터를 전송주기에 맞춰 송/수신하는 역할을 함.
         * 
         * 다음의 두 가지 방법으로 PhotonView 컴포넌트의 Observe 속성에
         * 특정 스크립트를 연결할 수 있다.
         * 
         * 1)Resources 폴더 안에 있는 MainPlayer 프리팹을 선택하고 Inspector 뷰의 
         * PlayerCtrl 컴포넌트(스크립트) 헤더를 드래그드롭해서 PhotonView 컴포넌트의 Observe옵션에 연결
         * 
         * 2)다음과 같이 스크립트에서 직접 연결
         */

        //PhotonView Observed Components 속성에 PlayerCtrl(현재) 스크립트 Component를 연결
        pv.ObservedComponents[0] = this;

        /*
         * PhotonView 컴포넌트의 Observe option 속성
         * 옵션                           설명
         * off                            실시간 데이터 송수신을 하지 않음.
         * ReliableDeltaCompressed        데이터를 정확히 송수신한다(TCP/IP 프로토콜)
         * Unreliable                     데이터의 정합성을 보장할 수 없지만 속도가 빠르다(UDP 프로토콜)
         * UnreliableOnChange             Unreliable 옵션과 같지만 변경사항이 발생했을 경우에만 전송한다
         */

        //데이타 전송 타입을 설정
        pv.synchronization = ViewSynchronization.UnreliableOnChange;

        //자신의 네트워크 객체가 아닐때...(마스터 클라이언트가 아닐때)
        if (!PhotonNetwork.isMasterClient)
        {
            //원격 네트워크 유저의 아바타는 물리력을 안받게 처리하고
            //또한, 물리엔진으로 이동 처리하지 않고(Rigidbody로 이동 처리시...)
            //실시간 위치값을 전송받아 처리 한다 그러므로 Rigidbody 컴포넌트의
            //isKinematic 옵션을 체크해주자. 한마디로 물리엔진의 영향에서 벗어나게 하여
            //불필요한 물리연산을 하지 않게 해주자...

            //원격 네트워크 플레이어의 아바타는 물리력을 이용하지 않음 (마스터 클라이언트가 아닐때)
            //(원래 게임이 이렇다는거다...우리건 안해도 체크 돼있음...)
            myRbody.isKinematic = true;
            //네비게이션도 중지
            //myTraceAgent.isStopped = true; 이걸로 하면 off Mesh Link 에서 에러 발생 그냥 비활성 하자
            myTraceAgent.enabled = false;
        }

        // 원격 플래이어의 위치 및 회전 값을 처리할 변수의 초기값 설정 
        // 잘 생각해보자 이런처리 안하면 순간이동 현상을 목격
        currPos = myTr.position;
        currRot = myTr.rotation;

        /////////////////////////////////////////////////////////////////


    }

    // Use this for initialization
    IEnumerator Start()
    {

        // 테스트 모드 전처리기 셋팅
#if CBT_MODE
        hp = 1000;
#elif RELEASE_MODE
        hp = 100;
#endif
        //Animation 컴포넌트의 clip속성에 idle1 애니메이션 클립 지정 
        _anim.clip = anims.idle1;
        //지정한 애니메이션 클립(애니메이션) 실행 
        _anim.Play();

        //포톤 추가//////////////////////////////////
        // 마스터 클라이언트만 수행
        if (PhotonNetwork.isMasterClient)
        {
            //일단 첫 Base의 Transform만 연결
            traceTarget = baseAll[0].transform;

            //추적하는 대상의 위치(Vector3)를 셋팅하면 바로 추적 시작 (가독성이 좋다)
            myTraceAgent.SetDestination(traceTarget.position);
            // 위와 같은 동작을 수행하지만...가독성이 별로다
            // myTraceAgent.destination = traceObj.position;

            // 정해진 시간 간격으로 Enemy의 Ai 변화 상태를 셋팅하는 코루틴
            StartCoroutine(ModeSet());

            // Enemy의 상태 변화에 따라 일정 행동을 수행하는 코루틴
            StartCoroutine(ModeAction());

            // 일정 간격으로 주변의 가장 가까운 Base와 플레이어를 찾는 코루틴 
            StartCoroutine(this.TargtSetting());

            // 로밍 루트 설정
            RoamingCheckStart();
        }
        // 포톤 추가
        // 마스터 클라이언트가 아닐때 네트워크 객체를 일정 간격으로 애니메이션을 동기화 하는 코루틴
        else
        {
            StartCoroutine(this.NetAnim());
        }

        ////////////////////////////////////////////////

        yield return null;
    }

    // Update is called once per frame
    void Update()
    {

        //포톤 추가///////////////////////////////////////////////////////

        // Update 함수에서 로직구현으로 AI 및 애니메이션 처리했으나
        // PhotonNetwork.isMasterClient 속성이 false일 때는 즉 내게(마스터클라이언트) 아닐땐.. 로직을 수행하지 않고
        // 바로 함수를 빠져나와 네트워크 플레이어의 아바타의 오작동을 방지하자

        // 자신이 마스터 클라이언트가 아닌 경우는 루틴을 빠져 나감 
        //if (!PhotonNetwork.isMasterClient) return;

        //////////////////////////////////////////////////////////////

        //myTraceAgent.SetDestination(traceTarget.position);

        //랜덤 애니메이션 선택 
        if (Time.time > randAnimTime)
        {
            randAnim = Rand.Range(0, 4);
            randAnimTime = Time.time + 5.0f;
        }

        //Enemy 배고픈 시간 (추적 시간) => 이 로직은 코루틴으로 처리하면 더 효율적
        if (!hungry)
        {
            if (Time.time > hungryTime)
            {
                // 이거 안하면 몬스터 로밍박스에서 Idle할때 다음 목적지 못간다...
                RoamingCheckStart();
                // 변수 순서가 중요하다.
                hungry = true;
                nonHungryTime = Time.time + nonHungryTimeSet + Rand.Range(10f, 15f);
                nonHungry = true;

            }
        }

        // 로밍 중지 로직 
        if (nonHungry)
        {
            if (Time.time > nonHungryTime)
            {
                // 변수 순서가 중요하다.
                nonHungry = false;
                hungryTime = Time.time + hungryTimeSet + Rand.Range(10f, 15f);
                hungry = false;
            }

        }


        //공격 받았을 경우 
        if (isHit)
        {
            if (Time.time > isHitTime)
            {
                isHit = false;
            }
        }


        //적을 봐라봄  
        /*  if (enemyLook)
            {
                if (Time.time > enemyLookTime)
                {
                    //enemyLookRotation.eulerAngles = new Vector3(myTr.rotation.x, enemyLookRotation.eulerAngles.y, myTr.rotation.z); // 버그시...

                    //	enemyLookRotation = Quaternion.LookRotation(-(enemyTr.forward)); // - 해줘야 바라봄  
                    enemyLookRotation = Quaternion.LookRotation(enemyTr.position - myTr.position); // - 해줘야 바라봄  
                    myTr.rotation = Quaternion.Lerp(myTr.rotation, enemyLookRotation, Time.deltaTime * 10.0f);
                    enemyLookTime = Time.time + 0.01f;
                }
            }*/

        //포톤 추가
        // 마스터 클라이언트가 직접 Ai 및 애니메이션 로직 수행
        // pv.isMine 해도 됨
        if (PhotonNetwork.isMasterClient)
        {
            // 처리
        }
        //포톤 추가
        //원격 플레이어일 때 수행
        else
        {
            //원격 플레이어의 아바타를 수신받은 위치까지 부드럽게 이동시키자
            myTr.position = Vector3.Lerp(myTr.position, currPos, Time.deltaTime * 3.0f);
            //원격 플레이어의 아바타를 수신받은 각도만큼 부드럽게 회전시키자
            myTr.rotation = Quaternion.Slerp(myTr.rotation, currRot, Time.deltaTime * 3.0f);
        }


    }

    /*
     *  Enemy의 변화 상태에 따라 일정 행동을 수행하는 코루틴
     */
    IEnumerator ModeSet()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.2f);

            //자신과 Player의 거리 셋팅 
            float dist = Vector3.Distance(myTr.position, traceTarget.position);

            // 순서 중요
            if (isHit)  //공격 받았을시
            {
                enemyMode = MODE_STATE.hit;
            }
            else if (dist <= attackDist) // Attack 사거리에 들어왔는지 ??
            {
                enemyMode = MODE_STATE.attack; //몬스터의 상태를 공격으로 설정 
            }
            else if (traceAttack)  // 몬스터를 추적중이라면...
            {
                enemyMode = MODE_STATE.trace; //몬스터의 상태를 추적으로 설정
            }
            else if (dist <= traceDist) // Trace 사거리에 들어왔는지 ??
            {
                enemyMode = MODE_STATE.trace; //몬스터의 상태를 추적으로 설정 
            }
            else if (dist <= findDist) // Find 사거리에 들어왔는지 ??
            {
                enemyMode = MODE_STATE.surprise; //몬스터의 상태를 놀람으로 설정 
            }
            else if (hungry) //  배고플 때 (주인공 찾는다)
            {
                enemyMode = MODE_STATE.move; //몬스터의 상태를 이동으로 설정 
            }
            else if (sleep) // 잠잘 때 
            {
                enemyMode = MODE_STATE.sleep; //몬스터의 상태를 취침으로 설정 
            }
            else
            {
                enemyMode = MODE_STATE.idle; //몬스터의 상태를 idle 모드로 설정 
            }
        }
    }
    /*
	 * Enemy의 상태 변화에 따라 일정 행동을 수행하는 코루틴
	 */
    IEnumerator ModeAction()
    {
        while (!isDie)
        {
            switch (enemyMode)
            {
                //Enemy가 Idle 상태 일때... 
                case MODE_STATE.idle:

                    //네비게이션 멈추고 (추적 중지)
                    myTraceAgent.isStopped = true;

                    if (randAnim == 0)
                    {
                        //idle1 애니메이션 
                        _anim.CrossFade(anims.idle1.name, 0.3f);

                        // 포톤 추가
                        // 애니메이션 동기화
                        net_Aim = 0;
                    }
                    else if (randAnim == 1)
                    {
                        //idle2 애니메이션 
                        _anim.CrossFade(anims.idle2.name, 0.3f);

                        // 포톤 추가
                        // 애니메이션 동기화
                        net_Aim = 1;
                    }
                    else if (randAnim == 2)
                    {
                        //idle3 애니메이션 
                        _anim.CrossFade(anims.idle3.name, 0.3f);

                        // 포톤 추가
                        // 애니메이션 동기화
                        net_Aim = 2;
                    }
                    else if (randAnim == 3)
                    {
                        //idle3 애니메이션 
                        _anim.CrossFade(anims.idle4.name, 0.3f);

                        // 포톤 추가
                        // 애니메이션 동기화
                        net_Aim = 3;
                    }
                    break;

                //Enemy가 Trace 상태 일때... 
                case MODE_STATE.trace:

                    // 네비게이션 재시작(추적)
                    myTraceAgent.isStopped = false;
                    // 추적대상 설정(플레이어)
                    myTraceAgent.destination = traceTarget.position;

                    //네비속도 및 애니메이션 속도 제어
                    if (enemyKind == MODE_KIND.enemyBoss)
                    {
                        // 네비게이션의 추적 속도를 현재보다 1.8배
                        myTraceAgent.speed = speed * 1.8f;

                        //애니메이션 속도 변경
                        _anim[anims.move.name].speed = 1.8f;

                        //run 애니메이션 
                        _anim.CrossFade(anims.move.name, 0.3f);

                        // 포톤 추가
                        // 애니메이션 동기화
                        net_Aim = 4;

                    }
                    else
                    {
                        // 네비게이션의 추적 속도를 현재보다 1.5배
                        myTraceAgent.speed = speed * 1.5f;

                        //애니메이션 속도 변경
                        _anim[anims.move.name].speed = 1.5f;

                        //run 애니메이션 
                        _anim.CrossFade(anims.move.name, 0.3f);

                        // 포톤 추가
                        // 애니메이션 동기화
                        net_Aim = 5;

                    }
                    break;

                //공격 상태
                case MODE_STATE.attack:

                    //사운드 (공격)

                    //추적 중지(선택사항)
                    //네비게이션 멈추고 (추적 중지) 
                    myTraceAgent.isStopped = true;

                    //공격할때 적을 봐라 봐야함 
                    // myTr.LookAt(traceTarget.position); // 바로 봐라봄
                    // enemyLookRotation.eulerAngles = new Vector3(myTr.rotation.x, enemyLookRotation.eulerAngles.y, myTr.rotation.z);
                    Quaternion enemyLookRotation = Quaternion.LookRotation(traceTarget.position - myTr.position); // - 해줘야 바라봄  
                    myTr.rotation = Quaternion.Lerp(myTr.rotation, enemyLookRotation, Time.deltaTime * 10.0f);

                    if (randAnim == 0)
                    {
                        //attack1 애니메이션 
                        _anim.CrossFade(anims.attack1.name, 0.3f);

                        // 포톤 추가
                        // 애니메이션 동기화
                        net_Aim = 6;
                    }
                    else if (randAnim == 1)
                    {
                        //attack2 애니메이션 
                        _anim.CrossFade(anims.attack2.name, 0.3f);

                        // 포톤 추가
                        // 애니메이션 동기화
                        net_Aim = 7;
                    }
                    else if (randAnim == 2)
                    {
                        //attack3 애니메이션 
                        _anim.CrossFade(anims.attack3.name, 0.3f);

                        // 포톤 추가
                        // 애니메이션 동기화
                        net_Aim = 8;
                    }
                    else if (randAnim == 3)
                    {
                        //attack3 애니메이션 
                        _anim.CrossFade(anims.attack4.name, 0.3f);

                        // 포톤 추가
                        // 애니메이션 동기화
                        net_Aim = 9;
                    }
                    break;

                //이동 상태 
                case MODE_STATE.move:

                    // 네비게이션 재시작(추적)
                    myTraceAgent.isStopped = false;
                    // 추적대상 설정(로밍장소)
                    myTraceAgent.destination = roamingTarget.position;

                    //네비속도 및 애니메이션 속도 제어
                    if (enemyKind == MODE_KIND.enemyBoss)
                    {
                        // 네비게이션의 추적 속도를 현재보다 1.2배
                        myTraceAgent.speed = speed * 1.2f;

                        //애니메이션 속도 변경
                        _anim[anims.move.name].speed = 1.2f;

                        //run 애니메이션 
                        _anim.CrossFade(anims.move.name, 0.3f);

                        // 포톤 추가
                        // 애니메이션 동기화
                        net_Aim = 10;

                    }
                    else
                    {
                        // 네비게이션의 추적 속도를 현재 속도로...
                        myTraceAgent.speed = speed;

                        //애니메이션 속도 변경
                        _anim[anims.move.name].speed = 1.0f;

                        //walk 애니메이션 
                        _anim.CrossFade(anims.move.name, 0.3f);

                        // 포톤 추가
                        // 애니메이션 동기화
                        net_Aim = 11;

                    }
                    break;

                //놀람,적발견 상태 (여러가지 생각 해야함)
                //case MODE_STATE.surprise:

                //    if (!traceObject)
                //    {
                //        traceObject = true;

                //        //추적 중지 (선택사항)
                //        //네비게이션 멈추고 (추적 중지) 
                //        myTraceAgent.isStopped = true;

                //        //roar 애니메이션 
                //        _anim.CrossFade(anims.surprise.name, 0.3f);

                //        StartCoroutine(this.TraceObject());

                //        // 포톤 추가
                //        // 애니메이션 동기화
                //        net_Aim = 12;
                //    }

                //    break;

                //sleep 상태 
                case MODE_STATE.sleep:

                    //사운드 


                    //네비게이션 멈추고 (추적 중지) 
                    myTraceAgent.isStopped = true;

                    //sleep 애니메이션 
                    _anim.CrossFade(anims.sleep.name, 0.3f);

                    // 포톤 추가
                    // 애니메이션 동기화
                    net_Aim = 13;
                    break;

                //hit 상태  (여러가지 생각해야함 )
                //Enemy가 hit 상태 일때... 
                case MODE_STATE.hit:

                    //네비게이션 멈추고 (추적 중지)
                    myTraceAgent.isStopped = true;

                    if (randAnim == 0 || randAnim == 1)
                    {
                        // hit1 애니메이션 
                        _anim.CrossFade(anims.hit1.name, 0.3f);

                        // 포톤 추가
                        // 애니메이션 동기화
                        net_Aim = 14;
                    }
                    else if (randAnim == 1 || randAnim == 2)
                    {
                        // hit2 애니메이션 
                        _anim.CrossFade(anims.hit2.name, 0.3f);

                        // 포톤 추가
                        // 애니메이션 동기화
                        net_Aim = 15;
                    }


                    break;

            }


            yield return null;
        }
    }

    //// MODE_STATE.surprise 에서 호출 되는 함수 (이거없으면 계속 으르렁됨)
    //IEnumerator TraceObject()
    //{
    //    yield return new WaitForSeconds(2.5f);
    //    traceAttack = true;

    //    yield return new WaitForSeconds(5.5f);
    //    traceAttack = false;
    //    traceObject = false;
    //}

    // 자신과 가장 가까운 적을 찾음...플레이어가 베이스보다 우선순위가 높게 셋팅 추가
    IEnumerator TargtSetting()
    {
        while (!isDie)
        {

            yield return new WaitForSeconds(0.2f);

            // 자신과 가장 가까운 플레이어 찾음
            players = GameObject.FindGameObjectsWithTag("Player");

            //플레이어가 있을경우 
            if (players.Length != 0)
            {
                playerTarget = players[0].transform;
                dist1 = (playerTarget.position - myTr.position).sqrMagnitude;
                foreach (GameObject _players in players)
                {
                    if ((_players.transform.position - myTr.position).sqrMagnitude < dist1)
                    {
                        playerTarget = _players.transform;
                        dist1 = (playerTarget.position - myTr.position).sqrMagnitude;
                    }
                }
            }

            //// 자신과 가장 가까운 베이스 찾음
            //baseAll = GameObject.FindGameObjectsWithTag("Base");
            //baseTarget = baseAll[0].transform;
            //dist2 = (baseTarget.position - myTr.position).sqrMagnitude;
            //foreach (GameObject _baseAll in baseAll)
            //{
            //    if ((_baseAll.transform.position - myTr.position).sqrMagnitude < dist2)
            //    {
            //        baseTarget = _baseAll.transform;
            //        dist2 = (baseTarget.position - myTr.position).sqrMagnitude;
            //    }
            //}

            //만약 플레이어가 없으면 베이스 목표 설정  
            if (players.Length == 0)
            {
                traceTarget = baseTarget;
            }
            //그렇지 않으면...
            else
            {
                // 플레이어가 베이스보다 우선순위가 높게 셋팅 (게임마다 틀리다 즉 자기 맘)
                if (dist1 <= dist2)
                {
                    traceTarget = playerTarget;
                }
                else
                {
                    traceTarget = baseTarget;
                }
            }

        }
    }

    /* Vetor3.Distance || Vetor3.sqrMagnitude 
     * 
     * Vetor3.Distance : 두 점간의 거리를 구해주는 메소드
     * Vetor3.sqrMagnitude : 두 점간의 거리의 제곱에 값 (두 점간의 거리의 차이를 2차원 함수의 값으로 계산)
     *                       루트 연산을 하지 않으므로, 연산속도가 빠르다. 하지만 정확한 거리가 측정되지 않기 때문에
     *                       정확한 거리는 몰라도 되고 거리를 비교할 때 사용된다.
     * 
     * 연산속도 : Vetor3.Distance < Vetor3.sqrMagnitude (퍼포먼스 향상)
     * 정확성   : Vetor3.Distance > Vetor3.sqrMagnitude
     */

    //로밍 함수
    public void RoamingCheckStart()
    {
        StartCoroutine(this.RoamingCheck(roamingRandcheckPos));
    }

    //로밍 이동 로직 
    IEnumerator RoamingCheck(int pos)
    {

        roamingRandcheckPos = Rand.Range(1, roamingCheckPoints.Length);

        //같은 자리 안가게....
        if (roamingRandcheckPos == pos)
        {
            //중복값을 막기위하여 재귀함수 호출
            RoamingCheckStart();

            yield break;

        }

        //로밍 타겟 셋팅
        roamingTarget = roamingCheckPoints[roamingRandcheckPos];

        // Debug.Log("Checking1");
    }

    // (추가)

    // 몬스터 사망 처리
    public void EnemyDie()
    {
        // 포톤 추가
        if (pv.isMine)
        {
            StartCoroutine(this.Die());
        }
    }

    // Enemy의 사망 처리
    IEnumerator Die()
    {
        // Enemy의를 죽이자
        isDie = true;
        //죽는 애니메이션 시작
        _anim.CrossFade(anims.die.name, 0.3f);

        // 포톤 추가
        // 애니메이션 동기화
        net_Aim = 16;

        //Enemy의 모드를 die로 설정
        enemyMode = MODE_STATE.die;
        //Enemy의 태그를 Untagged로 변경하여 더이상 플레이어랑 포탑이 찾지 못함
        this.gameObject.tag = "Untagged";
        this.gameObject.transform.Find("EnemyBody").tag = "Untagged";
        //네비게이션 멈추고 (추적 중지) 
        myTraceAgent.isStopped = true;


        //Enemy에 추가된 모든 Collider를 비활성화(모든 충돌체는 Collider를 상속했음 따라서 다음과 같이 추출 가능)
        foreach (Collider coll in gameObject.GetComponentsInChildren<Collider>())
        {
            coll.enabled = false;
        }


        //4.5 초후 오브젝트 삭제
        yield return new WaitForSeconds(4.5f);

        // 포톤 추가
        // 자신과 네트워크상의 모든 아바타를 삭제
        PhotonNetwork.Destroy(gameObject);
    }

    // 드럼통 폭발 몬스터 사망 처리
    //public void EnemyBarrelDie(Vector3 firePos)
    //{
    //    StartCoroutine(this.BarrelDie(firePos));
    //}

    ////Enemy가 드럼통 폭발에 맞았을 때 호출되는 콜백 함수
    //IEnumerator BarrelDie(Vector3 firePos)
    //{

    //    // Enemy의를 죽이자
    //    isDie = true;
    //    //죽는 애니메이션 시작
    //    _anim.CrossFade(anims.die.name, 0.3f);
    //    //Enemy의 모드를 die로 설정
    //    enemyMode = MODE_STATE.die;
    //    //Enemy의 태그를 Untagged로 변경하여 더이상 플레이어랑 포탑이 찾지 못함
    //    this.gameObject.tag = "Untagged";
    //    this.gameObject.transform.Find("EnemyBody").tag = "Untagged";
    //    //네비게이션 멈추고 (추적 중지) 
    //    myTraceAgent.enabled = false;

    //    Rigidbody rbody = GetComponent<Rigidbody>();
    //    rbody.isKinematic = false;

    //    //무게 변경 (더 멀리 터지게)
    //    rbody.mass = 1.0f;
    //    //Rigidbody.AddExplosionForce(폭발력, 원점, 반경, 위로 솟구치는 힘);
    //    rbody.AddExplosionForce(1000.0f, firePos, 15.0f, 300.0f);

    //    //4.5 초후 isKinematic = true 해서 Enemy 콜라이더 해제해도 바닥으로 안떨어지게~
    //    yield return new WaitForSeconds(3.5f);
    //    rbody.isKinematic = true;

    //    //Enemy에 추가된 모든 Collider를 비활성화(모든 충돌체는 Collider를 상속했음 따라서 다음과 같이 추출 가능)
    //    foreach (Collider coll in gameObject.GetComponentsInChildren<Collider>())
    //    {
    //        coll.enabled = false;
    //    }


    //    //4.5 초후 오브젝트 삭제
    //    yield return new WaitForSeconds(4.5f);
    //    Destroy(gameObject);

    //}

    ////객체 소멸시 정리가 필요한 부분은 여기서...
    //void OnDestroy()
    //{
    //    //Debug.Log("Destroy");
    //    //모든 코루틴을 정지시키자
    //    StopAllCoroutines();
    //}

    //일정 확률로 Enemy 타격
    public void HitEnemy()
    {
        int rand = Rand.Range(0, 100);
        if (rand < 30)
        {
            if (randAnim == 0 || randAnim == 1)
            {
                isHitTime = Time.time + anims.hit1.length + 0.2f;
                isHit = true;
            }
            else if (randAnim == 1 || randAnim == 2)
            {
                isHitTime = Time.time + anims.hit1.length + 0.2f;
                isHit = true;
            }
        }

        //animator.SetTrigger("IsHit"); 
        //SetTrigger로 IsHit Trigger를 발생시키면, Any State에서 Hit으로 전이....
        //이럴땐 메카님이 확실히 편함
    }

    // 포톤 추가///////////////////////////////////////////////////////////////////////

    /*
	 * 마스터 클라이언트가 아닐때 애니메이션 상태를 동기화 하는 로직
     * RPC 로도 애니메이션 동기화 가능~!
	 */
    IEnumerator NetAnim()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.2f);

            if (!PhotonNetwork.isMasterClient)
            {
                if (net_Aim == 0)
                {
                    _anim.CrossFade(anims.idle1.name, 0.3f);
                }
                else if (net_Aim == 1)
                {
                    _anim.CrossFade(anims.idle2.name, 0.3f);
                }
                else if (net_Aim == 2)
                {
                    _anim.CrossFade(anims.idle3.name, 0.3f);
                }
                else if (net_Aim == 3)
                {
                    _anim.CrossFade(anims.idle4.name, 0.3f);
                }
                else if (net_Aim == 4)
                {
                    //애니메이션 속도 변경
                    _anim[anims.move.name].speed = 1.8f;

                    //run 애니메이션 
                    _anim.CrossFade(anims.move.name, 0.3f);
                }
                else if (net_Aim == 5)
                {
                    //애니메이션 속도 변경
                    _anim[anims.move.name].speed = 1.5f;

                    //run 애니메이션 
                    _anim.CrossFade(anims.move.name, 0.3f);
                }
                else if (net_Aim == 6)
                {
                    //attack1 애니메이션 
                    _anim.CrossFade(anims.attack1.name, 0.3f);
                }
                else if (net_Aim == 7)
                {
                    //attack2 애니메이션 
                    _anim.CrossFade(anims.attack2.name, 0.3f);
                }
                else if (net_Aim == 8)
                {
                    //attack3 애니메이션 
                    _anim.CrossFade(anims.attack3.name, 0.3f);
                }
                else if (net_Aim == 9)
                {
                    //attack4 애니메이션 
                    _anim.CrossFade(anims.attack4.name, 0.3f);
                }
                else if (net_Aim == 10)
                {
                    //애니메이션 속도 변경
                    _anim[anims.move.name].speed = 1.2f;

                    //run 애니메이션 
                    _anim.CrossFade(anims.move.name, 0.3f);
                }
                else if (net_Aim == 11)
                {
                    //애니메이션 속도 변경
                    _anim[anims.move.name].speed = 1.0f;

                    //walk 애니메이션 
                    _anim.CrossFade(anims.move.name, 0.3f);
                }
                else if (net_Aim == 12)
                {
                    //roar 애니메이션 
                    _anim.CrossFade(anims.surprise.name, 0.3f);
                }
                else if (net_Aim == 13)
                {
                    //sleep 애니메이션 
                    _anim.CrossFade(anims.sleep.name, 0.3f);
                }
                else if (net_Aim == 14)
                {
                    // hit1 애니메이션 
                    _anim.CrossFade(anims.hit1.name, 0.3f);
                }
                else if (net_Aim == 15)
                {
                    // hit2 애니메이션 
                    _anim.CrossFade(anims.hit2.name, 0.3f);
                }
                else if (net_Aim == 16)
                {
                    //죽는 애니메이션 시작
                    _anim.CrossFade(anims.die.name, 0.3f);

                    // 코루틴 함수를 빠져나감(종료)
                    yield break;
                }

            }
        }

    }

    /*
     * PhotonView 컴포넌트의 Observe 속성이 스크립트 컴포넌트로 지정되면 PhotonView
     * 컴포넌트는 데이터를 송수신할 때, 해당 스크립트의 OnPhotonSerializeView 콜백 함수를 호출한다. 
     */
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //로컬 플레이어의 위치 정보를 송신
        if (stream.isWriting)
        {
            //박싱
            stream.SendNext(myTr.position);
            stream.SendNext(myTr.rotation);
            stream.SendNext(net_Aim);
        }
        //원격 플레이어의 위치 정보를 수신
        else
        {
            //언박싱
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
            net_Aim = (int)stream.ReceiveNext();
        }

    }

    // 마스터 클라이언트가 변경되면 호출
    void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        if (PhotonNetwork.isMasterClient)
        {
            //일단 첫 Base의 Transform만 연결
            traceTarget = baseAll[0].transform;

            //추적하는 대상의 위치(Vector3)를 셋팅하면 바로 추적 시작 (가독성이 좋다)
            myTraceAgent.SetDestination(traceTarget.position);
            // 위와 같은 동작을 수행하지만...가독성이 별로다
            // myTraceAgent.destination = traceObj.position;

            // 정해진 시간 간격으로 Enemy의 Ai 변화 상태를 셋팅하는 코루틴
            StartCoroutine(ModeSet());

            // Enemy의 상태 변화에 따라 일정 행동을 수행하는 코루틴
            StartCoroutine(ModeAction());

            // 일정 간격으로 주변의 가장 가까운 Base와 플레이어를 찾는 코루틴 
            StartCoroutine(this.TargtSetting());

            // 로밍 루트 설정
            RoamingCheckStart();

            //myRbody.isKinematic = false;
            //네비게이션도 실행
            myTraceAgent.enabled = true;
        }
    }

    /*

    // 다음은 마스터 클라이언트의 명시적 변경을 예를 든거다

    // 접속 플레이어 정보 추가
    List<PhotonPlayer> setPlayer;

    //네트워크 플레이어가 룸으로 접속했을 때 호출되는 콜백 함수
    //PhotonPlayer 클래스 타입의 파라미터가 전달(서버에서...)
    //PhotonPlayer 파라미터는 해당 네트워크 플레이어의 정보를 담고 있다.
    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        // 플레이어 ID (접속 순번), 이름, 커스텀 속성
        Debug.Log(newPlayer.ToStringFull());

        if(PhotonNetwork.isMasterClient)
        {
            setPlayer.Add(newPlayer);
        }

    }

    // 마스터 클라이언트만 아래 함수 호출
    // 마스터 클라이언트 명시적 변경
    void SetMasterClient()
    {
        PhotonNetwork.SetMasterClient(setPlayer[0]);
    }

    */

    ////////////////////////////////////////////////////////////////

    // PropertyDrawer

    //인스펙터에 스크립트 우 클릭시 컨텍스트 메뉴에서 함수호출 가능
    [ContextMenu("FuncStart")]
    void FuncStart()
    {
        Debug.Log("Func start");
        Debug.Log(speed);
    }
}
