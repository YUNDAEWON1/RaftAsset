using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree_Object : MonoBehaviour
{
    private PhotonView pv;
    private InteractionObject interObj;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        interObj = GetComponent<InteractionObject>();
    }
    private void Start()
    {
    }

    private void Update()
    {
        if(interObj.interaction)
        {

            pv.RPC("PhotonObjectCreateMaster", PhotonTargets.AllBuffered, "Plank", transform.position, Quaternion.identity);
            pv.RPC("PhotonObjectDestroyMaster", PhotonTargets.AllBuffered, pv.viewID);
            interObj.interaction = !interObj.interaction;
            return;
        }
    }

    [PunRPC]
    void PhotonObjectCreateMaster(string name, Vector3 pos, Quaternion rot)
    {
        if (PhotonNetwork.isMasterClient)
        {
            GameObject createObject = PhotonNetwork.InstantiateSceneObject(name, pos, rot, 0, null);
            //createObject.GetComponent<Rigidbody>().isKinematic = false;
            //createObject.GetComponent<Rigidbody>().useGravity = true;
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

    [PunRPC]
    void PhotonObjectDestroyMaster(int viewID)
    {
        if (PhotonNetwork.isMasterClient)
        {
            // ID를 사용하여 게임 오브젝트 찾기
            GameObject obj = PhotonView.Find(viewID).gameObject;
            PhotonNetwork.Destroy(obj);
        }
    }

}
