using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCam : MonoBehaviour
{
    public Transform target;
    //private GameObject parentObj;
    private Animator ani;

    void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = target;
        transform.localPosition = Vector3.zero;
        ani = transform.root.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(ani.GetBool("Swimming"))
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y,
            (-transform.parent.transform.localEulerAngles.z) + 18
            );
        }
        else
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y,
            (-transform.parent.transform.localEulerAngles.z) + 6
            );
        }
    }
}