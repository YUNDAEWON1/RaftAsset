using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour
{
    private float hungry = -0.3f;
    private float thirsty = -0.3f;
    private float hp = 0.3f;

    public GameManager gm;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Axe"))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                gm.hungry += hungry;
                gm.thirsty += -thirsty;
                gm.hp += hp;
            }
        }
    }
}
