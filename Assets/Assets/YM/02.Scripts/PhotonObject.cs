using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonObject : MonoBehaviour
{
    // 포톤추가
    //PhotonView 컴포넌트를 할당할 레퍼런스
    private PhotonView pv = null;

    //위치정보를 송수신할때 사용할 변수 선언 및 초기값 설정
    private Vector3 currPos = Vector3.zero;
    private Quaternion currRot = Quaternion.identity;

    public int objectNum = 0;    // 바껴야되는데...

    // HG 작성
    //private Plank_Object plank_object;
    //private Plastic_Object plastic_object;
    //private Leaf_Object leaf_object;
    //private Tree_Object tree_object;

    void Awake()
    {
        pv = this.GetComponent<PhotonView>();

        //PhotonView Observed Components 속성에 PlayerCtrl(현재) 스크립트 Component를 연결
        pv.ObservedComponents[0] = this;

        //데이타 전송 타입을 설정
        pv.synchronization = ViewSynchronization.UnreliableOnChange;
    }
    // Start is called before the first frame update
    void Start()
    {
        currPos = transform.position;
        currRot = transform.rotation;

        // HG 작성
        //plank_object = GetComponentInChildren<Plank_Object>();
        //if (plank_object != null) plank_object.enabled = pv.isMine;
        //plastic_object = GetComponentInChildren<Plastic_Object>();
        //if (plastic_object != null) plastic_object.enabled = pv.isMine;
        //leaf_object = GetComponentInChildren<Leaf_Object>();
        //if (leaf_object != null) leaf_object.enabled = pv.isMine;
        //tree_object = GetComponentInChildren<Tree_Object>();
        //if (tree_object != null) tree_object.enabled = pv.isMine;

    }

    // Update is called once per frame
    void Update()
    {
        if(!pv.isMine)
        {
            //원격 플레이어의 아바타를 수신받은 위치까지 부드럽게 이동시키자
            transform.position = Vector3.Lerp(transform.position, currPos, Time.deltaTime * 4f);
            //원격 플레이어의 아바타를 수신받은 각도만큼 부드럽게 회전시키자
            transform.rotation = Quaternion.Slerp(transform.rotation, currRot, Time.deltaTime * 4f);
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

            ////HG 작성
            //stream.SendNext(plank_object.target.position);
            //stream.SendNext(plastic_object.target.position);
            //stream.SendNext(leaf_object.target.position);
            //stream.SendNext(tree_object.transform.position);

        }
        else //원격 플레이어의 위치 정보를 수신
        {
            //언박싱
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();

            // HG 작성
            //Vector3 plankTargetPos = (Vector3)stream.ReceiveNext();
            //if (!pv.isMine)
            //{
            //    plank_object.target.position = plankTargetPos;
            //    plastic_object.target.position = plankTargetPos;
            //    leaf_object.target.position = plankTargetPos;
            //}
        }
    }
}
