using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    private Animator ani;

    void Awake()
    {
        ani = gameObject.GetComponent<Animator>();
    }

    public void StopAni()
    {
        if (Input.GetMouseButton(0))
        {
            ani.speed = 0f;
        }    
    }
}
