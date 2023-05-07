﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkCtrl : MonoBehaviour
{
    private AudioSource playerAudioSource;

    public AudioClip[] audioClips;
    private AudioSource audioSource;

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

    //private GameManager gm;

    public Material[] sharkDamagedEffectMat;

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

    public bool run = false;
    private bool deadBool = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        ani = GetComponent<Animator>();
        // 로밍 위치 얻기
        roamingCheckPoints = GameObject.Find("RoamingPoint").GetComponentsInChildren<Transform>();
        rigid = GetComponent<Rigidbody>();
        
    }

    private void Start()
    {
        //gm = FindObjectOfType<GameManager>();
        currentState = State.Idle;
        RoamingCheckStart();
    }

    private void Update()
    {
        if(!run)
        {
            players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                if (player.GetComponent<PlayerCtrl>().swimMode)
                {
                    targetPlayer = player.transform;
                    currentState = State.Chase;
                    ani.SetBool("Chase", true);
                    ani.SetBool("Attack", true);
                    break;
                }
                else
                {
                    targetPlayer = null;
                }
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
            //case State.Attack:
            //    Attack();
            //    break;
            case State.Dead:
                if(!deadBool)
                {
                    StartCoroutine(Dead());
                }
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
            ani.SetBool("Attack", false);
            return;
        }

        // 추적하는 플레이어는 가장 먼저 물에 들어온 플레이어
        transform.LookAt(targetPlayer);                                         
        transform.position += transform.forward * (moveSpeed * 3) * Time.deltaTime;     // Chase(추격) 시 평소(Idle) 스피드 6배

        // 일정 거리 이내에 있으면 상태를 Attack로 변경
        if (Vector3.Distance(transform.position, targetPlayer.position) <= attackDistance)
        {
            StartCoroutine(Attack());
            ani.SetBool("Attack", true);
        }
    }

    IEnumerator Attack()
    {
        // 추적 중인 플레이어를 공격하는 로직 구현
        // 공격이 끝나면 targetPlayer를 null로 초기화
        if(targetPlayer.tag == "Player")
        {
            targetPlayer.GetComponent<PlayerCtrl>().hp -= 0.01f;
            yield return new WaitForSeconds(0.5f);

            if(run == false)
            {
                targetPlayer.GetComponent<PlayerCtrl>().audioSource.clip = targetPlayer.GetComponent<PlayerCtrl>().audioClips[6];
                targetPlayer.GetComponent<PlayerCtrl>().audioSource.Play();

                audioSource.clip = audioClips[0];
                audioSource.volume = 0.5f;
                audioSource.Play();
            }

            run = true;
            targetPlayer = roamingTarget;

            ani.SetBool("Attack", false);

            currentState = State.Chase;
            ani.SetBool("Chase", true);

            //targetPlayer = null;
        }

    }

    public void Damaged()
    {
        StartCoroutine(DamagedEffect());
        HP -= 25;

        if(HP <= 50)
        {
            run = true;
            targetPlayer = roamingTarget;

            ani.SetBool("Attack", false);

            currentState = State.Chase;
            ani.SetBool("Chase", true);
        }
    }

    IEnumerator DamagedEffect()
    {
        transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material = sharkDamagedEffectMat[1];
        yield return new WaitForSeconds(2f);
        transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material = sharkDamagedEffectMat[0];
    }

    IEnumerator Dead()
    {
        deadBool = true;
        ani.SetBool("Dead", true);

        yield return new WaitForSeconds(30f);
        HP = 100;
        currentState = State.Idle;
        ani.SetBool("Dead", false);
        deadBool = false;
        run = false;
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