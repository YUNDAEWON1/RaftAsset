using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookPickupObj : MonoBehaviour
{
    private PlayerCtrl playerCtrl;
    private Rigidbody rigid;

    private InventoryManager inventoryManager;
    private PhotonView pv;

    void Awake()
    {
        playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
        rigid = GetComponent<Rigidbody>();

        inventoryManager = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryManager>();
        pv = GetComponent<PhotonView>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (playerCtrl.hookThrow && other.transform.tag == "Object")    // 추후 갈고리로 습득가능한 애들한테 Object 태그 붙여주기
        {
            if (other.GetComponent<WaterMoveObject>() != null)
            {
                other.GetComponent<WaterMoveObject>().enabled = false;
            }
            if (other.GetComponent<WaterObject>() != null)
            {
                other.GetComponent<WaterObject>().enabled = false;
            }

            other.transform.position = Vector3.MoveTowards(other.transform.position, transform.position, 0.5f);          // 트리거 이벤트가 발생한 오브젝트의 Tag가 'Object'이면 끌고오기

            if (Vector3.Distance(playerCtrl.transform.position, other.transform.position) < 1)                           // 끌고오는 물체와 캐릭터의 거리가 1 미만이면
            {
                //UI쪽 함수로 대체
                inventoryManager.AddItem(other.transform.GetComponent<PhotonObject>().objectNum);

                pv.RPC("PhotonObjectDestroyMaster", PhotonTargets.AllBuffered, other.GetComponent<PhotonView>().viewID);

                //PhotonNetwork.Destroy(other.gameObject);   // 추후에는 포톤디스트로이 해야한다

                //for (int i = 0; i < playerCtrl.stuffs.Length; i++)  // 먼저 소지품배열을 전부 돌면서 주우려는 것과 같은 물건이 있는지 확인
                //{
                //    if (playerCtrl.stuffs[i] == null)               // 빈칸이면 패스
                //    { continue; }
                //    else                                            // 빈칸이 아니고
                //    {
                //        if (playerCtrl.stuffs[i].tag == other.tag)   // 주으려는 물건의 태그와 소지품에 있는 태그가 같다면
                //        {
                //            playerCtrl.stuffsCount[i]++;            // 해당하는 소지품 카운트 ++
                //            Destroy(other.transform.gameObject);
                //            Debug.Log(playerCtrl.stuffsCount[i]);
                //            return;
                //        }
                //    }
                //}
                //// 소지품 배열에 주으려는 물건과 같은물건이 없다면 여기까지 온다
                //for (int i = 0; i < playerCtrl.stuffs.Length; i++)
                //{
                //    if (playerCtrl.stuffs[i] == null)                                                               // 빈 슬롯에
                //    {
                //        playerCtrl.stuffs[i] = other.gameObject;                                                    // 소스상의 stuffs배열과 오브젝트 Stuffs 동기화
                //        //playerCtrl.stuffs[i].transform.parent = GameObject.FindGameObjectWithTag("Stuffs").transform;
                //        playerCtrl.stuffsCount[i] = 1;

                //        other.GetComponent<Rigidbody>().isKinematic = true;                                         // 물리 off
                //        other.transform.localPosition = Vector3.zero;                                               //Transform 초기화
                //        other.transform.localRotation = Quaternion.identity;
                //        //hitInfo.collider.transform.localScale = new Vector3(1f, 1f, 1f);
                //        return;
                //    }
                //}
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Sea")             // Hook이 물에 뜨게끔하기
        {
            rigid.isKinematic = true;       // 오브젝트랑 상호작용하는거 봐야할듯
            //rigid.useGravity = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Sea")
        {
            rigid.isKinematic = false;
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
}
