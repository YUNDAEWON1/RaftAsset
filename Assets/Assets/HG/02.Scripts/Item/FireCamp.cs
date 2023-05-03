using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCamp : MonoBehaviour
{
    public InventoryManager ivenManger;
    private InteractionObject interObj;

    private void Awake()
    {
        ivenManger = FindObjectOfType<InventoryManager>();
        interObj = GetComponent<InteractionObject>();
    }

    private void Update()
    {
        if (interObj.interaction)
        {
            ivenManger.Craft(ivenManger.craftingRecipes[7]);
            //PhotonNetwork.Destroy(other.gameObject);

            interObj.interaction = !interObj.interaction;
            return;
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if(Input.GetKeyDown(KeyCode.F) && other.gameObject.CompareTag("Potato"))
    //    {
    //        ivenManger.Craft(ivenManger.craftingRecipes[7]);
    //        PhotonNetwork.Destroy(other.gameObject);
    //    }
    //}
}
