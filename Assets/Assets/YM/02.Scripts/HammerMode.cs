using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerMode : MonoBehaviour
{
    private PhotonView pv;
    private PlayerCtrl playerCtrl;
    public Transform firePos;

    private List<GameObject> raftList;

    public GameObject raftPrefab;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        playerCtrl = GetComponent<PlayerCtrl>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (playerCtrl.hammerMode)
        {
            RaycastHit[] hits = Physics.SphereCastAll(firePos.transform.position, transform.lossyScale.x / 5f, Camera.main.transform.forward, 3f); // 2미터 앞에 스페어캐스트에 걸리는게 있다면(범위 추후 조정필요할듯)
            for (int j = 0; j < hits.Length; j++)
            {
                if (hits[j].collider.tag == "HammerObject")
                {
                    Quaternion rotation = Quaternion.identity;
                   
                    // 충돌한 물체의 센터 - 충돌지점
                    if ((hits[j].collider.bounds.center - (firePos.transform.position + Camera.main.transform.forward * hits[j].distance)).x > 0.5)  // 왼쪽
                    {
                        Debug.Log("왼쪽");
                    }
                    else if ((hits[j].collider.bounds.center - (firePos.transform.position + Camera.main.transform.forward * hits[j].distance)).x < -0.5)   // 오른쪽
                    {
                        Debug.Log("오른쪽");
                    }
                    else if ((hits[j].collider.bounds.center - (firePos.transform.position + Camera.main.transform.forward * hits[j].distance)).z > 0.5)     // 아래
                    {
                        Debug.Log("아래");
                    }
                    else if ((hits[j].collider.bounds.center - (firePos.transform.position + Camera.main.transform.forward * hits[j].distance)).z < -0.5)   // 위
                    {
                        Debug.Log("위");
                    }
                }
            }
        }
    }

    void OnDrawGizmos()     //디버그때만
    {
        if (playerCtrl.hammerMode)
        {
            Gizmos.color = Color.green;
            float sphereScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

            RaycastHit[] hits = Physics.SphereCastAll(firePos.transform.position, transform.lossyScale.x / 5f, Camera.main.transform.forward, 3f);
            for (int i = 0; i < hits.Length; i++)
            {
                // 함수 파라미터 : 현재 위치, Sphere의 크기(x,y,z 중 가장 큰 값이 크기가 됨), Ray의 방향, RaycastHit 결과, Sphere의 회전값, SphereCast를 진행할 거리
                //if (true == Physics.SphereCast(firePos.transform.position, sphereScale / 1f, Camera.main.transform.forward, out RaycastHit hit, 3f) && hit.collider.tag != "Ground")
                if (hits[i].collider.tag == "HammerObject")
                {
                    // Hit된 지점까지 ray를 그려준다.
                    Gizmos.DrawRay(firePos.transform.position, Camera.main.transform.forward * hits[i].distance);

                    // Hit된 지점에 Sphere를 그려준다.
                    Gizmos.DrawWireSphere(firePos.transform.position + Camera.main.transform.forward * hits[i].distance, sphereScale / 5f);
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
