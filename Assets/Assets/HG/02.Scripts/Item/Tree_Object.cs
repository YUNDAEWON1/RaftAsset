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
            PhotonNetwork.Instantiate("Plank", transform.position, Quaternion.identity, 0, null);
            PhotonNetwork.Destroy(this.gameObject);
            interObj.interaction = !interObj.interaction;
            return;
        }
    }

}
