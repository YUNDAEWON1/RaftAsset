using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCamp : MonoBehaviour
{
    public float hungry = 0.3f;

    private void OnTriggerStay(Collider other)
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            if (other.gameObject.CompareTag("player"))
            {
                // 헝그리 게이지 올라감
            }

        }
    }
}
