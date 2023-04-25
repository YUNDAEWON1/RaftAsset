using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree_Object : MonoBehaviour
{
    public int hp = 5;

    private void Update()
    {
        if(hp == 0)
        {
            //나무 개수 늘리기
            Destroy(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Axe"))
        {
            hp--;
        }
    }
}
