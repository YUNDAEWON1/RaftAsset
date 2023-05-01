using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamingBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("온트리거엔터");
        if(other.tag == "Enemy")
        {
            other.gameObject.GetComponent<SharkCtrl>().RoamingCheckStart();
        }
    }
    
}
