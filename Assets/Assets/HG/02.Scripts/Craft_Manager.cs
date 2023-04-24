using UnityEngine;

public class Craft_Manager : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    //[SerializeField] Item_Manager items;
    [SerializeField] GameObject firepos;
    [SerializeField] int itemID;

    private PlayerCtrl playerCtrl;
    private PhotonView pv;

    private void Awake()
    {
        firepos = GameObject.Find("Firepos");
        //items = FindObjectOfType<Item_Manager>();
        playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
        pv = GetComponent<PhotonView>();
    }

    private void Update()
    {     
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    itemID++;

        //    //if (itemID > items.items.Count)
        //    //{
        //    //    itemID = 0;
        //    //}

        //    //if (items.items.ContainsKey(itemID))
        //    //{
        //    //    GetComponent<MeshFilter>().mesh = items.items[itemID].mesh;
        //    //    GetComponent<MeshRenderer>().material = items.items[itemID].material;
        //    //}
        //}

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    itemID--;

        //    //if (itemID < 0)
        //    //{
        //    //    itemID = items.items.Count - 1;
        //    //}

        //    //if (items.items.ContainsKey(itemID))
        //    //{
        //    //    GetComponent<MeshFilter>().mesh = items.items[itemID].mesh;
        //    //    GetComponent<MeshRenderer>().material = items.items[itemID].material;
        //    //}
        //}

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    firepos.transform.Rotate(new Vector3(0f, 90f, 0f));

        //    float currentAngle = transform.rotation.eulerAngles.y;

        //    if (currentAngle < -360f)
        //    {
        //        currentAngle += 360f;
        //    }

        //    else if (currentAngle > 360f)
        //    {
        //        currentAngle -= 360f;
        //    }

        //    if (currentAngle < -360f || currentAngle > 360f)
        //    {
        //        Debug.LogWarning("회전 범위를 벗어났습니다.");
        //    }

        //    else
        //    {
        //        currentAngle = Mathf.Clamp(currentAngle, -360f, 360f);
        //        transform.rotation = Quaternion.Euler(new Vector3(0f, currentAngle, 0f));
        //    }
        //}
    }

    private void OnTriggerStay(Collider other)
    {
        if(playerCtrl.hammerMode && playerCtrl.inventoryOn == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("aaaaaaaaaaaaaaaaaaaaa");
                //if (other.CompareTag("Ground")) // 이 조건문을 마우스 버튼 다운에 같이 넣었을때 밑에  if (other.CompareTag("R")... 조건문 실행 안됨 이유 모름
                //{
                //    if (itemID > 0 && itemID <= items.prefabs.Length)
                //    {
                //        Instantiate(items.prefabs[itemID - 1], transform.position, transform.rotation);
                //    }
                //}

                if (other.CompareTag("R") || other.CompareTag("L") || other.CompareTag("Up") || other.CompareTag("Down"))
                {
                    Vector3 position = other.transform.position;
                    Quaternion rotation = other.transform.rotation;
                    //Instantiate(prefab, position, rotation);
                    pv.RPC("HammerClickMaster", PhotonTargets.AllBuffered, "Foundation", position, rotation);
                }
            }
        }
    }

    [PunRPC]
    void HammerClickMaster(string ID, Vector3 pos, Quaternion rot)
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.InstantiateSceneObject(ID, pos, rot, 0, null);
        }
    }
}