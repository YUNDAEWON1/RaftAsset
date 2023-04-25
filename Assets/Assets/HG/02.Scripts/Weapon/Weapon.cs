using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Data
{
    public int id;
    public float delayTime;
    public int maxBullet;
    public string info;
    public string soundEffect;
    public GameObject aim;
    public GameObject bullet;
}

public abstract class Weapon : MonoBehaviour
{
    public Data data;

    bool isCreate_Mode = false;


    public abstract void InitSetting();

    public virtual void Using(Transform tip)
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            //빌딩모드
        }
    }
}
