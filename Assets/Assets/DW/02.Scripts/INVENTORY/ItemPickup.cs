using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        if(other.tag.Equals("Player"))
        {
            Destroy(this.gameObject);
        }
        
    }
}
