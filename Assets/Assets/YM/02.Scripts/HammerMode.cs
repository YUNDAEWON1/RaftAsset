using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerMode : MonoBehaviour
{
    private Dictionary<int, string> photonMapping;

    private PhotonView pv;
    private PlayerCtrl playerCtrl;
    public Transform firePos;

    public Transform hammerShadow;

    public Material[] ShadowMat;

    private RaycastHit hitInfo;

    public GameObject[] hammerObject;
    public int selectObject = 0;

    public int rotation = 0;

    private GameObject[] buildingUI;
    private InventoryManager inventoryManager;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        playerCtrl = GetComponent<PlayerCtrl>();
        
        inventoryManager = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryManager>();

    }

    // Start is called before the first frame update
    void Start()
    {
        buildingUI = GameObject.FindGameObjectsWithTag("BuildingUI");

        this.photonMapping = playerCtrl.photonMapping;
        hammerShadow.position = Vector3.zero;
        hammerShadow.rotation = Quaternion.identity;

        for (int i = 0; i < buildingUI.Length; i++)
        {
            buildingUI[i].gameObject.SetActive(false);  // 모든 빌딩UI끄기
        }
    }

    void Update()
    {
        if (playerCtrl.hammerMode)
        {
            if (Input.GetKeyDown("q"))
            {
                selectObject--;
                if (selectObject < 0)
                {
                    selectObject = 4;
                }
            }
            if (Input.GetKeyDown("e"))
            {
                selectObject++;
                if (selectObject > 4)   //최대 수 추가할때마다 올려주기
                {
                    selectObject = 0;
                }
            }

            //Debug.Log(buildingUI.Length);
            for(int i = 0; i < buildingUI.Length; i++)
            {
                buildingUI[i].gameObject.SetActive(false);  // 모든 빌딩UI끄기
            }
            buildingUI[selectObject].gameObject.SetActive(true);

            if (Input.GetKeyDown("r"))
            {
                rotation++;
                if(rotation > 3)
                {
                    rotation = 0;
                }
            }

            RaycastHit[] hits = Physics.SphereCastAll(firePos.transform.position, transform.lossyScale.x / 1f, Camera.main.transform.forward, 3f); // 2미터 앞에 스페어캐스트에 걸리는게 있다면(범위 추후 조정필요할듯)
            for (int j = 0; j < hits.Length; j++)
            {
                if (hits[j].collider.gameObject.layer == 12)
                {
                    if(inventoryManager.buildingRecipes[selectObject].CanBuild(inventoryManager))
                    {
                        hammerShadow.GetComponent<MeshRenderer>().material = ShadowMat[0];
                    }
                    else
                    {
                        hammerShadow.GetComponent<MeshRenderer>().material = ShadowMat[1];
                    }

                    hammerShadow.GetComponent<MeshFilter>().mesh = hammerObject[selectObject].GetComponent<MeshFilter>().sharedMesh;
                    Vector3 shadowPos = firePos.transform.position + Camera.main.transform.forward * hits[j].distance;          // 레이케스트 충돌지점(모든 hammerobject감지)

                    switch (selectObject)
                    {
                        case 0: // 뗏목바닥
                            {
                                Vector3 newPosition = hits[j].collider.bounds.center;

                                Physics.SphereCast(firePos.transform.position, transform.lossyScale.x / 5f, Camera.main.transform.forward, out hitInfo, 3f);

                                if (hitInfo.collider != null)
                                {
                                    if (hitInfo.collider.tag == "Foundation")
                                    {
                                        hammerShadow.GetComponent<MeshFilter>().mesh = null;
                                        break;
                                    }
                                }

                                // 충돌한 물체의 센터 - 충돌지점
                                if ((hits[j].collider.bounds.center - hits[j].point).x > 0.5)  // 왼쪽
                                {
                                    newPosition.x = newPosition.x - 1.5f;
                                }
                                else if ((hits[j].collider.bounds.center - hits[j].point).x < -0.5)   // 오른쪽
                                {
                                    newPosition.x = newPosition.x + 1.5f;
                                }
                                else if ((hits[j].collider.bounds.center - hits[j].point).z > 0.5)     // 아래
                                {
                                    newPosition.z = newPosition.z - 1.5f;
                                }
                                else if ((hits[j].collider.bounds.center - hits[j].point).z < -0.5)   // 위
                                {
                                    newPosition.z = newPosition.z + 1.5f;
                                }
                                newPosition.y = newPosition.y + 0.25f;

                                hammerShadow.position = newPosition;
                                hammerShadow.rotation = Quaternion.identity;

                                break;
                            }

                        case 1: // 나무기둥
                            {
                                Physics.SphereCast(firePos.transform.position, transform.lossyScale.x / 5f, Camera.main.transform.forward, out hitInfo, 3f);

                                if (hitInfo.collider != null)
                                {
                                    if (hitInfo.collider.tag == "Foundation" || hitInfo.collider.tag == "WoodenFloor")
                                    {
                                        switch (rotation)
                                        {
                                            case 0:
                                                {
                                                    Vector3 newPosition = new Vector3(hitInfo.collider.bounds.center.x, hitInfo.collider.bounds.center.y + 0.2f, hitInfo.collider.transform.localPosition.z + 0.75f);
                                                    hammerShadow.position = newPosition;
                                                    hammerShadow.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                                                    break;
                                                }
                                            case 1:
                                                {
                                                    Vector3 newPosition = new Vector3(hitInfo.collider.bounds.center.x - 0.75f, hitInfo.collider.bounds.center.y + 0.2f, hitInfo.collider.transform.localPosition.z);
                                                    hammerShadow.position = newPosition;
                                                    hammerShadow.rotation = Quaternion.Euler(new Vector3(0, 90, 90));
                                                    break;
                                                }
                                            case 2:
                                                {
                                                    Vector3 newPosition = new Vector3(hitInfo.collider.bounds.center.x, hitInfo.collider.bounds.center.y + 0.2f, hitInfo.collider.transform.localPosition.z - 0.75f);
                                                    hammerShadow.position = newPosition;
                                                    hammerShadow.rotation = Quaternion.Euler(new Vector3(0, 180, 90));
                                                    break;
                                                }

                                            case 3:
                                                {
                                                    Vector3 newPosition = new Vector3(hitInfo.collider.bounds.center.x + 0.75f, hitInfo.collider.bounds.center.y + 0.2f, hitInfo.collider.transform.localPosition.z);
                                                    hammerShadow.position = newPosition;
                                                    hammerShadow.rotation = Quaternion.Euler(new Vector3(0, 270, 90));
                                                    break;
                                                }
                                        }
                                    }
                                    else
                                    {
                                        hammerShadow.GetComponent<MeshFilter>().mesh = null;
                                        break;
                                    }
                                }
                                break;

                            }
                        case 2: // 나무계단
                            {
                                Physics.SphereCast(firePos.transform.position, transform.lossyScale.x / 5f, Camera.main.transform.forward, out hitInfo, 3f);

                                if (hitInfo.collider != null)
                                {
                                    if (hitInfo.collider.tag == "Foundation" || hitInfo.collider.tag == "WoodenFloor")
                                    {
                                        Vector3 newPosition = new Vector3(hitInfo.collider.bounds.center.x, hitInfo.collider.bounds.center.y + 0.2f, hitInfo.collider.bounds.center.z);
                                        hammerShadow.position = newPosition;

                                        switch(rotation)
                                        {
                                            case 0:
                                                hammerShadow.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                                                break;
                                            case 1:
                                                hammerShadow.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                                                break;
                                            case 2:
                                                hammerShadow.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                                                break;
                                            case 3:
                                                hammerShadow.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        hammerShadow.GetComponent<MeshFilter>().mesh = null;
                                        break;
                                    }
                                }
                                break;

                            }
                        case 3: // 나무바닥
                            {
                                Physics.SphereCast(firePos.transform.position, transform.lossyScale.x / 5f, Camera.main.transform.forward, out hitInfo, 3f);

                                if (hitInfo.collider != null)
                                {
                                    if (hitInfo.collider.tag == "WoodenPole")
                                    {
                                        Vector3 newPosition = new Vector3(hitInfo.collider.bounds.center.x, hitInfo.collider.bounds.center.y + 1.35f, hitInfo.collider.bounds.center.z);
                                        hammerShadow.position = newPosition;
                                        hammerShadow.rotation = Quaternion.identity;
                                    }
                                    else if(hitInfo.collider.tag == "WoodenFloor")
                                    {
                                        Vector3 newPosition = hitInfo.collider.bounds.center;

                                        if ((hitInfo.collider.bounds.center - hitInfo.point).x > 0.5)  // 왼쪽
                                        {
                                            newPosition.x = newPosition.x - 1.5f;
                                        }
                                        else if ((hitInfo.collider.bounds.center - hitInfo.point).x < -0.5)   // 오른쪽
                                        {
                                            newPosition.x = newPosition.x + 1.5f;
                                        }
                                        else if ((hitInfo.collider.bounds.center - hitInfo.point).z > 0.5)     // 아래
                                        {
                                            newPosition.z = newPosition.z - 1.5f;
                                        }
                                        else if ((hitInfo.collider.bounds.center - hitInfo.point).z < -0.5)   // 위
                                        {
                                            newPosition.z = newPosition.z + 1.5f;
                                        }
                                        newPosition.y = newPosition.y + 0.1f;

                                        hammerShadow.position = newPosition;
                                        hammerShadow.rotation = Quaternion.identity;
                                    }
                                    else
                                    {
                                        hammerShadow.GetComponent<MeshFilter>().mesh = null;
                                        break;
                                    }
                                }
                                break;
                            }
                        case 4: // 나무벽
                            {
                                Physics.SphereCast(firePos.transform.position, transform.lossyScale.x / 5f, Camera.main.transform.forward, out hitInfo, 3f);

                                if (hitInfo.collider != null)
                                {
                                    if (hitInfo.collider.tag == "Foundation" || hitInfo.collider.tag == "WoodenFloor")
                                    {
                                        switch (rotation)
                                        {
                                            case 0:
                                                {
                                                    Vector3 newPosition = new Vector3(hitInfo.collider.bounds.center.x, hitInfo.collider.bounds.center.y, hitInfo.collider.transform.localPosition.z + 0.75f);
                                                    hammerShadow.position = newPosition;
                                                    hammerShadow.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                                                    break;
                                                }
                                            case 1:
                                                {
                                                    Vector3 newPosition = new Vector3(hitInfo.collider.bounds.center.x - 0.75f, hitInfo.collider.bounds.center.y, hitInfo.collider.transform.localPosition.z);
                                                    hammerShadow.position = newPosition;
                                                    hammerShadow.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                                                    break;
                                                }
                                            case 2:
                                                {
                                                    Vector3 newPosition = new Vector3(hitInfo.collider.bounds.center.x, hitInfo.collider.bounds.center.y, hitInfo.collider.transform.localPosition.z - 0.75f);
                                                    hammerShadow.position = newPosition;
                                                    hammerShadow.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                                                    break;
                                                }
                                                
                                            case 3:
                                                {
                                                    Vector3 newPosition = new Vector3(hitInfo.collider.bounds.center.x + 0.75f, hitInfo.collider.bounds.center.y, hitInfo.collider.transform.localPosition.z);
                                                    hammerShadow.position = newPosition;
                                                    hammerShadow.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                                                    break;
                                                }
                                        }
                                    }
                                    else
                                    {
                                        hammerShadow.GetComponent<MeshFilter>().mesh = null;
                                        break;
                                    }
                                }
                                break;
                            }


                    }
                    break;
                }
                else
                {
                    hammerShadow.GetComponent<MeshFilter>().mesh = null;
                }
            }
        }
        else
        {
            hammerShadow.GetComponent<MeshFilter>().mesh = null;

            for (int i = 0; i < buildingUI.Length; i++)
            {
                buildingUI[i].gameObject.SetActive(false);  // 모든 빌딩UI끄기
            }
        }
    }

    public void HammerClick()
    {
        pv.RPC("HammerClickMaster", PhotonTargets.AllBuffered, photonMapping[hammerObject[selectObject].GetComponent<PhotonObject>().objectNum], hammerShadow.position, hammerShadow.rotation);
    }

    [PunRPC]
    void HammerClickMaster(string ID, Vector3 pos, Quaternion rot)
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.InstantiateSceneObject(ID, pos, rot, 0, null);
        }
    }

    void OnDrawGizmos()     //디버그때만
    {
        if (playerCtrl.hammerMode)
        {
            Gizmos.color = Color.green;
            float sphereScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

            RaycastHit[] hits = Physics.SphereCastAll(firePos.transform.position, transform.lossyScale.x / 1f, Camera.main.transform.forward, 3f);
            for (int i = 0; i < hits.Length; i++)
            {
                // 함수 파라미터 : 현재 위치, Sphere의 크기(x,y,z 중 가장 큰 값이 크기가 됨), Ray의 방향, RaycastHit 결과, Sphere의 회전값, SphereCast를 진행할 거리
                //if (true == Physics.SphereCast(firePos.transform.position, sphereScale / 1f, Camera.main.transform.forward, out RaycastHit hit, 3f) && hit.collider.tag != "Ground")
                if (hits[i].collider.gameObject.layer == 12)
                {
                    // Hit된 지점까지 ray를 그려준다.
                    Gizmos.DrawRay(firePos.transform.position, Camera.main.transform.forward * hits[i].distance);

                    // Hit된 지점에 Sphere를 그려준다.
                    Gizmos.DrawWireSphere(firePos.transform.position + Camera.main.transform.forward * hits[i].distance, sphereScale / 1f);
                    break;
                }
                else
                {
                    // Hit가 되지 않았으면 최대 검출 거리로 ray를 그려준다.
                    Gizmos.DrawRay(firePos.transform.position, Camera.main.transform.forward * 3f);
                }
            }
        }
    }
}
