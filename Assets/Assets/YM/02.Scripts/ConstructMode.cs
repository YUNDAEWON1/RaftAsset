using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructMode : MonoBehaviour
{
    private Dictionary<int, string> photonMapping;

    private PhotonView pv;
    private PlayerCtrl playerCtrl;
    public Transform firePos;

    public Transform constructShadow;

    private RaycastHit hitInfo;

    public Material[] shadowMat;

    public bool constuctPossibility = true;

    //public int rotation = 0;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        playerCtrl = GetComponent<PlayerCtrl>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.photonMapping = playerCtrl.photonMapping;
        constructShadow.position = Vector3.zero;
        constructShadow.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (playerCtrl.constructMode)
        {
            RaycastHit[] hits = Physics.SphereCastAll(firePos.transform.position, transform.lossyScale.x / 5f, Camera.main.transform.forward, 3f); // 2미터 앞에 스페어캐스트에 걸리는게 있다면(범위 추후 조정필요할듯)
            for (int j = 0; j < hits.Length; j++)
            {
                if (hits[j].collider.tag == "Foundation" || hits[j].collider.tag == "WoodenFloor")   //  일단 뗏목 바닥에만 건설한다고 생각하면 이게 맞음
                {
                    Physics.SphereCast(firePos.transform.position, transform.lossyScale.x / 5f, Camera.main.transform.forward, out hitInfo,3f);
                    if (hitInfo.transform.gameObject.layer == 8)    // 기둥같은 애들이 있어도 건설 불가능하게 해야할거 같은데...
                    {
                        constructShadow.GetComponent<MeshRenderer>().material = shadowMat[1];
                        constuctPossibility = false;
                    }
                    else
                    {
                        constructShadow.GetComponent<MeshRenderer>().material = shadowMat[0];
                        constuctPossibility = true;
                    }

                    constructShadow.GetComponent<MeshFilter>().mesh = Resources.Load<GameObject>(photonMapping[playerCtrl.stuffs.transform.GetChild(playerCtrl.swapNum).GetChild(0).GetComponent<DraggableItem>().item.ID]).GetComponent<MeshFilter>().sharedMesh;
                    Vector3 shadowPos = firePos.transform.position + Camera.main.transform.forward * hits[j].distance;
                    constructShadow.position = new Vector3(shadowPos.x, shadowPos.y - 0.2f, shadowPos.z);
                    
                    if(Input.GetKeyDown("r"))
                    {
                        constructShadow.Rotate(0f, 90f, 0f);
                    }
                    break;
                }
                else
                {
                    constructShadow.GetComponent<MeshFilter>().mesh = null;
                }
            }
        }
        else
        {
            constructShadow.GetComponent<MeshFilter>().mesh = null;
        }
    }

    public void ConstructClick(int ID)
    {
        pv.RPC("ConstructClickMaster", PhotonTargets.AllBuffered, photonMapping[ID], constructShadow.position, constructShadow.rotation);
    }

    [PunRPC]
    void ConstructClickMaster(string ID, Vector3 pos, Quaternion rot)
    {
        if(PhotonNetwork.isMasterClient)
        {
            GameObject constructObject = PhotonNetwork.InstantiateSceneObject(ID, pos, rot, 0, null);
            constructObject.tag = "InteractionObject";   // 건설한 오브젝트의 태그를 없애서 건설한 애들은 주울 수 없게끔 하고 상호작용 가능하게 만든다
            constructObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    void OnDrawGizmos()     //디버그때만
    {
        if (playerCtrl.constructMode)
        {
            Gizmos.color = Color.green;
            float sphereScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

            RaycastHit[] hits = Physics.SphereCastAll(firePos.transform.position, transform.lossyScale.x / 5f, Camera.main.transform.forward, 3f);
            for (int i = 0; i < hits.Length; i++)
            {
                // 함수 파라미터 : 현재 위치, Sphere의 크기(x,y,z 중 가장 큰 값이 크기가 됨), Ray의 방향, RaycastHit 결과, Sphere의 회전값, SphereCast를 진행할 거리
                //if (true == Physics.SphereCast(firePos.transform.position, sphereScale / 1f, Camera.main.transform.forward, out RaycastHit hit, 3f) && hit.collider.tag != "Ground")
                if (hits[i].collider.tag == "Foundation")
                {
                    // Hit된 지점까지 ray를 그려준다.
                    Gizmos.DrawRay(firePos.transform.position, Camera.main.transform.forward * hits[i].distance);

                    // Hit된 지점에 Sphere를 그려준다.
                    Gizmos.DrawWireSphere(constructShadow.position, sphereScale / 5f);
                    break;
                }
                else
                {
                    // Hit가 되지 않았으면 최대 검출 거리로 ray를 그려준다.
                    Gizmos.DrawRay(firePos.transform.position, Camera.main.transform.forward * 3f);
                    //test
                }
            }
        }
    }
}
