using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour
{
    public float hungry = -0.3f;
    public float thirsty = 0.3f;
    public float hp = 0.3f;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                // Thirstybar -> gage 겟 컴포넌트 이미지 fillamount -0.3f
                // hungrybar -> gage --
                // player hpbar -> gage --
            }
        }
    }
}
