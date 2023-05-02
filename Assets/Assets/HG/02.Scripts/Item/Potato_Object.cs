using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Potato_Object : MonoBehaviour
{
    public GameManager gm;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            gm.hungry += 0.5f;
            Destroy(this);
            // 인벤토리에서 제거하는 기능도 추가해야함
        }
    }
}
