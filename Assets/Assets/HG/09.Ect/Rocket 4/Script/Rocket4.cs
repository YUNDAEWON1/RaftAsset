using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket4 : MonoBehaviour
{

    public Rigidbody rig;
    public ConstantForce cf;
    public Transform IsKinematic;
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        //Destroy(this, 10f);
    }

    IEnumerator Start()

    {
        //Wait for 3 secs.
        yield return new WaitForSeconds(5);

        //Game object will turn off
        //GameObject.Find("Rocket").SetActive(false);

        //rig.isKinematic = true;
        //cf.enabled = false;

        pv.RPC("PhotonObjectCreateMaster", PhotonTargets.AllBuffered, "Heli", new Vector3(-89f, 36f, -89f), Quaternion.identity);
        pv.RPC("PhotonObjectDestroyMaster", PhotonTargets.AllBuffered, pv.viewID);

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

    [PunRPC]
    void PhotonObjectCreateMaster(string name, Vector3 pos, Quaternion rot)
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.InstantiateSceneObject(name, pos, rot, 0, null);
        }
    }

    public Transform rocket;
    [SerializeField] float y;

    private void Update()
    {
        rocket.transform.position = Vector3.Lerp(rocket.transform.position, new Vector3(rocket.transform.position.x, rocket.transform.position.y + y, rocket.transform.position.z), 3f);
    }

    IEnumerator Test()
    {
        Debug.Log("1");

        yield return new WaitForSeconds(3);

        Debug.Log('2');
    }
}