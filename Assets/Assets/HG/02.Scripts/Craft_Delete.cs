using UnityEngine;

public class Craft_Delete : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 12)
        {
            Destroy(gameObject);
        }
    }
}
