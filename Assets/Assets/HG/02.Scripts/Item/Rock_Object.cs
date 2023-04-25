using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock_Object : MonoBehaviour
{
    public int rock;

    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.F))
        {
            rock++;
            Destroy(this);
        }
    }
}
