using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foundation : MonoBehaviour
{
    //public Transform build;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Foundation"))
        {
            Debug.Log("함수탓어요");
            Vector3 newPosition = other.transform.position;
            transform.position = other.transform.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Foundation"))
        {
            Debug.Log("함수탓어요");
            Vector3 newPosition = collision.transform.position;
            transform.position = collision.transform.position;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Foundation"))
        {
            Debug.Log("함수탓어요");
            Vector3 newPosition = other.transform.position;
            transform.position = other.transform.position;
        }
    }

}
