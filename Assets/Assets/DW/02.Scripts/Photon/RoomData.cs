﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;


//포톤 추가
public class RoomData : MonoBehaviour
{
    //외부 접근을 위해 public으로 선언했지만 Insperctor에 노출하고 싶지 않을때
    [HideInInspector]
    //방이름
    public string roomName="";

    //현재 접속 유저수
    [HideInInspector]
    public int connectPlayer=0;

    //룸의 최대 접속자수
    [HideInInspector]
    public int maxPlayers=4;
    // 룸 비밀번호
    [HideInInspector]
    public string password = "";
    // 호스트 이름
    [HideInInspector]
    public string hostName = "";

    //룸 이름 표시할 Text UI 항목 연결 레퍼런스
    public Text textRoomName;
    //룸 난이도 표시 Text UI 항목 연결 레퍼런스
    public Text textModeInfo;

    //룸 정보를 전달한 후 Text UI 항목에 룸 정보를 표시하는 함수
    public void DisplayRoomData()
    {
        textRoomName.text=roomName;
        textModeInfo.text="보통";
    }
}
