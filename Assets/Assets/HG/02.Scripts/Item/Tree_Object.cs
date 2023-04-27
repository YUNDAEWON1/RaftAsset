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

    private void Update()
    {
       
    }

    private void OnCollisionStay(Collision col)
    {
        if(col.gameObject.CompareTag("Hook"))
        {
            Destroy(gameObject);
            Instantiate(woods, transform.position, Quaternion.identity);
        }
    }   
}
