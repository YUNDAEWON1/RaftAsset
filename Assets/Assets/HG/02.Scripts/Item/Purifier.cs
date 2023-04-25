using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Purifier : MonoBehaviour
{
    public float coolTime = 5f;
    float thirsty = 0.3f; // 목마름 게이지

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                // Thirstybar -> gage 겟 컴포넌트 이미지 fillamount ++0.3f
            }

        }
    }
}
