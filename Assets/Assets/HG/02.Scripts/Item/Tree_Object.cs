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
            GameObject createObj =  PhotonNetwork.Instantiate("Plank", transform.position, Quaternion.identity, 0, null);

            if (createObj.transform.GetComponent<WaterMoveObject>() != null)
            {
                createObj.transform.GetComponent<WaterMoveObject>().enabled = false;
            }

            if (createObj.transform.GetComponent<WaterObject>() != null)
            {
                createObj.transform.GetComponent<WaterObject>().enabled = false;
            }

            PhotonNetwork.Destroy(this.gameObject);
            interObj.interaction = !interObj.interaction;
            return;
        }
    }

}
