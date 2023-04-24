using UnityEngine;

public class Craft_Delete : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HammerObject"))
        {
            Destroy(gameObject);
        }
    }
}
