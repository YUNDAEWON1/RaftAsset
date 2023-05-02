using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMoveObject : MonoBehaviour
{
    public int plastic = 1;
    public Transform target;
    public float speed = 0.01f;
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        // GameObject.Find를 사용하여 "Target"이라는 이름의 오브젝트를 찾아서 target 변수에 할당
        target = GameObject.Find("Target").transform;
    }

    private void Update()
    {
        if (target != null)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, target.position, step);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Land"))
        {
            //Destroy(gameObject);
            pv.RPC("PhotonObjectDestroyMaster", PhotonTargets.AllBuffered, transform.GetComponent<PhotonView>().viewID);
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
