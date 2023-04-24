using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// uGUI의 UI 항목을 사용하기 위한 네임스페이스 설정
using UnityEngine.UI;

// 포톤 추가
public class DisplayUserId : MonoBehaviour {

    // uGUI의 Text 항목 연결을 위한 레퍼런스
    public Text userId;

    //PhotonView 컴포넌트를 할당할 레퍼런스 
    PhotonView pv = null;

    // Use this for initialization
    void Start () {

        //컴포넌트를 할당 
        pv = GetComponent<PhotonView>();

        //HUD(Head Up Display)의 유저 아이디 설정 
        userId.text = pv.owner.NickName;

    }

}