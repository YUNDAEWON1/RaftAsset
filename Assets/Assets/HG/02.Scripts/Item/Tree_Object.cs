using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree_Object : MonoBehaviour
{
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if(Input.GetMouseButtonDown(0) && other.CompareTag("Axe"))
        {
            PhotonNetwork.Instantiate("Plank", transform.position, Quaternion.identity, 0, null);
            PhotonNetwork.Destroy(this.pv);
            
        }
    }
}
