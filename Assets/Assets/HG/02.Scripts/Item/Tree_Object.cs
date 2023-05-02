using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree_Object : MonoBehaviour
{
    public int hp = 5;
    public GameObject woods;

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if(Input.GetMouseButtonDown(0) && other.CompareTag("Axe"))
        {
            Destroy(this);
            Instantiate(woods, transform.position, Quaternion.identity);
        }
    }
}
