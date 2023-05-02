using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkCtrl : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float attackDistance = 2f;
    public int HP = 100;

    private float idleTime = 0;
    private float idleTimeCount = 0;

    private Rigidbody rigid;

    //로밍장소
    private Transform[] roamingCheckPoints;
    //로밍 장소 중복해서 안가게
    private int roamingRandcheckPos;
    //로밍타겟
    public Transform roamingTarget;

    private Animator ani;

    private GameManager gameMgr;

    private enum State
    {
        Idle,
        Chase,
        Attack,
        Dead
    }

    private State currentState;
    private GameObject[] players = new GameObject[3];
    public Transform targetPlayer;

    private void Awake()
    {
        ani = GetComponent<Animator>();
        // 로밍 위치 얻기
        roamingCheckPoints = GameObject.Find("RoamingPoint").GetComponentsInChildren<Transform>();
        rigid = GetComponent<Rigidbody>();
        gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        currentState = State.Idle;
        RoamingCheckStart();
    }

    private void Update()
    {

        players = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in players)
        {
            if(player.GetComponent<PlayerCtrl>().swimMode)
            {
                targetPlayer = player.transform;
                currentState = State.Chase;
                ani.SetBool("Chase", true);
                break;
            }
            else
            {
                targetPlayer = null;
            }
        } 

        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Dead:
                Dead();
                break;
        }

        if (HP <= 0)
        {
            currentState = State.Dead;
        }
    }

    private void Idle()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(0, transform.rotation.y, 0, transform.rotation.w), Time.deltaTime);

        float idleRandom = Random.Range(0, 100);

        // 움직이지 않거나 무작위로 돌아다니는 로직 구현        
        if (idleRandom < 20) // 20% 확률로 움직이지 않음
        {
            // 움직이지 않음
        }
        else
        {
            transform.LookAt(roamingTarget);
            transform.position += transform.forward * (moveSpeed/2) * Time.deltaTime;   // 평소에는 1/2스피드로 움직이게
        }    
    }

    private void Chase()
    {
        if(!targetPlayer)
        {
            currentState = State.Idle;
            ani.SetBool("Chase", false);
            return;
        }

        // 추적하는 플레이어는 가장 먼저 물에 들어온 플레이어
        transform.LookAt(targetPlayer);                                         
        transform.position += transform.forward * (moveSpeed * 2) * Time.deltaTime;     // Chase(추격) 시 평소(Idle) 스피드 4배

        // 일정 거리 이내에 있으면 상태를 Attack로 변경
        if (Vector3.Distance(transform.position, targetPlayer.position) <= attackDistance)
        {
            currentState = State.Attack;
            ani.SetBool("Attack", true);
        }
    }

    private void Attack()
    {
        // 추적 중인 플레이어를 공격하는 로직 구현
        // 일정 시간이 지나면 상태를 IDLE로 변경
        // 공격이 끝나면 targetPlayer를 null로 초기화
        gameMgr.hp -= 0.1f;
        gameMgr.Hp();

        currentState = State.Idle;
        ani.SetBool("Chase", false);
        ani.SetBool("Attack", false);
        targetPlayer = null;
    }

    private void Dead()
    {

    }

    public void RoamingCheckStart()
    {
        StartCoroutine(this.RoamingCheck(roamingRandcheckPos));
    }

    //로밍 이동 로직 
    IEnumerator RoamingCheck(int pos)
    {
        roamingRandcheckPos = Random.Range(1, roamingCheckPoints.Length);

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

    // 물에서 들어오고 나갈때 애니메이션변경 및 모드변경
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Sea")
        {
            rigid.useGravity = false;
        }
    }
    // 물에서 들어오고 나갈때 애니메이션변경 및 모드변경
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Sea")
        {
            rigid.useGravity = true;
        }
    }
}