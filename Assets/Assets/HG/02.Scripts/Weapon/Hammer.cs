using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : Weapon
{
    public override void InitSetting()
    {
        data.delayTime = 0.1f;
        data.id = 2;
        data.info = "현재 무기 : 망치";
        data.aim = GameObject.Find("Aim");
        //data.bullet = Resources.Load<GameObject>(null);
    }

    public override void Using(Transform tip)
    {
        base.Using(tip);
    }
}
