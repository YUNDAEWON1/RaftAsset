using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicop : MonoBehaviour
{
    public GameObject heli;
    public GameObject player;

    private void Awake()
    {
        heli = GameObject.Find("Heli");
        player = GameObject.FindWithTag("Player");

    }

    void Start()
    {
        
    }

    void Update()
    {
        if (player != null)
        {
            Vector3 currentPosition = heli.transform.position;
            Vector3 targetPosition = player.transform.position;
            currentPosition.x = Mathf.Lerp(currentPosition.x, targetPosition.x, Time.deltaTime * 0.2f);
            currentPosition.z = Mathf.Lerp(currentPosition.z, targetPosition.z, Time.deltaTime * 0.2f);
            heli.transform.position = currentPosition;
        }
    }
}
