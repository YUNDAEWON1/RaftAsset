using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    //자기 자신의 Transform을 참조 할 수있는 레퍼런스
    Transform myTr;
    //메인 카메라의 Transform을 참조 할 수있는 레퍼런스
    Transform mainCameraTr;

    // Use this for initialization
    void Start()
    {

        // 레퍼런스 연결
        myTr = GetComponent<Transform>();

        //스테이지에 있는 메인 카메라의 Transform 컴포넌트를 추출 후 연결
        mainCameraTr = Camera.main.transform;

    }

    // Update is called once per frame
    void Update()
    {
        //빌보드 처리 (항상 카메라를 90도 방향으로 바라봄)
        myTr.LookAt(mainCameraTr);
    }
}
