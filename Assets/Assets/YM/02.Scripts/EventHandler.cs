using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    private PlayerCtrl playerCtrl;
    private Animator ani;

    void Awake()
    {
        ani = gameObject.GetComponent<Animator>();
        playerCtrl = transform.parent.GetComponent<PlayerCtrl>();
    }

    public void StopAni()
    {
        if (Input.GetMouseButton(0))
        {
            ani.speed = 0f;
        }    
    }
    public void AnimationEnd()
    {
        // 애니메이션이 종료되면 실행 중인 플래그를 재설정합니다.
        playerCtrl.isAnimating = false;
    }
}
