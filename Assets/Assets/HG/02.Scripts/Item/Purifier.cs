using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Purifier : MonoBehaviour
{
    public GameManager gm;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.F) && other.gameObject.CompareTag("Cup"))
        {
            gm.thirsty += 0.5f;
        }
    }
}
