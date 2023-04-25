using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Craft
{
    public GameObject prefab;
    public GameObject preview_Prefab;
}

public class Craft_Mode : MonoBehaviour
{
    bool isPreviewActivated = false;
    bool isBuildable = false;

    [SerializeField]
    Craft[] craft;

    GameObject preview;
    [SerializeField]
    GameObject prefab;

    [SerializeField]
    Transform player;

    [SerializeField]
    RaycastHit[] hitinfo;

    [SerializeField]
    RaycastHit hit;

    [SerializeField]
    float range;

    private void Awake()
    {
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated)
        {
            preview = Instantiate(craft[0].preview_Prefab, player.position + player.forward, Quaternion.identity);
            isPreviewActivated = true;
        }

        if (isPreviewActivated)
        {
            PreviewPosition();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
        }
    }

    void Build()
    {
        if (isBuildable)
        {
            prefab = Instantiate(craft[0].prefab, preview.transform.position, preview.transform.rotation);
            Destroy(preview);
            isPreviewActivated = false;
        }
    }

    void Cancel()
    {
        if(isPreviewActivated)
        {
            Destroy(preview);
            isPreviewActivated = false;
        }
    }

    void PreviewPosition()
    {
        RaycastHit[] hitInfos = Physics.RaycastAll(player.position, player.forward, range);
        RaycastHit hitInfo;

        if (hitInfos.Length > 0) // 충돌 정보가 하나 이상 있는 경우
        {
            hitInfo = hitInfos[0]; // 첫 번째 충돌 정보만을 선택하여 처리
            if (hitInfo.transform != null)
            {
                Vector3 location = hitInfo.point;

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    preview.transform.rotation *= Quaternion.Euler(0, -90f, 0f);
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    preview.transform.rotation *= Quaternion.Euler(0, +90f, 0f);
                }

                location.Set(Mathf.Round(location.x), Mathf.Round(location.y / 0.1f) * 0.1f, Mathf.Round(location.z));
                preview.transform.position = location;

                if (hitInfo.collider.gameObject.layer == 12)
                {
                    preview.GetComponent<Renderer>().material.color = Color.green;
                    isBuildable = true;

                    if (Input.GetMouseButtonDown(0))
                    {
                        Instantiate(craft[0].prefab, hitInfo.point, Quaternion.identity);
                        //Build();
                    }
                }
                else
                {
                    preview.GetComponent<Renderer>().material.color = Color.red;
                    isBuildable = false;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(player.position, player.forward * range);
    }
}