using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCamp : MonoBehaviour
{
    public InventoryManager ivenManger;

    private void Start()
    {
        ivenManger = FindObjectOfType<InventoryManager>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(Input.GetKeyDown(KeyCode.F) && other.gameObject.CompareTag("Potato"))
        {
            ivenManger.Craft(ivenManger.craftingRecipes[7]);
            PhotonNetwork.Destroy(other.gameObject);
        }
    }
}
