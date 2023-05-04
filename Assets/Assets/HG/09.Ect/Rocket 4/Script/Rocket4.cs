using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket4 : MonoBehaviour
{

    public Rigidbody rig;
    public ConstantForce cf;
    public Transform IsKinematic;

    IEnumerator Start()

    {
        //Wait for 3 secs.
        yield return new WaitForSeconds(3);

        //Game object will turn off
        GameObject.Find("Rocket").SetActive(false);

        rig.isKinematic = true;
        cf.enabled = false;


    }
    public Transform rocket;
    [SerializeField] float y;
    private void Awake()
    {
        Destroy(this, 10f);
    }

    private void Update()
    {
        rocket.transform.position = Vector3.Lerp(rocket.transform.position, new Vector3(rocket.transform.position.x, rocket.transform.position.y + y, rocket.transform.position.z), 3f);
    }
}