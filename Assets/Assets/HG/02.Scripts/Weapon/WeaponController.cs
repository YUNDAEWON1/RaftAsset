using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Weapon myWaepon;
    public Transform tip;

    private void Start()
    {
        myWaepon.InitSetting();
    }

    private void Update()
    {
        myWaepon.Using(tip);
    }
}
