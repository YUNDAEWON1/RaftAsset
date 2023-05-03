using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Purifier : MonoBehaviour
{
    public GameManager gm;
    private InteractionObject interObj;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        interObj = GetComponent<InteractionObject>();
    }

    private void Update()
    {
        if (interObj.interaction)
        {
            gm.thirsty += 0.5f;

            interObj.interaction = !interObj.interaction;
            return;
        }
    }
    //private void OnTriggerStay(Collider other)
    //{
    //    if (Input.GetKeyDown(KeyCode.F) && other.gameObject.CompareTag("Cup"))
    //    {
    //        gm.thirsty += 0.5f;
    //    }
    //}
}
