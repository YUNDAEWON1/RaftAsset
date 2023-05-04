using UnityEngine;

public class Helicop : MonoBehaviour
{
    public GameObject player;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (player != null)
        {
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = player.transform.position;

            if (Mathf.Abs(currentPosition.x - targetPosition.x) <= 1f && Mathf.Abs(currentPosition.z - targetPosition.z) <= 5f)
            {
                currentPosition.y = Mathf.Lerp(currentPosition.y, targetPosition.y + 1f, Time.deltaTime * 0.5f);
            }
            else
            {
                currentPosition.x = Mathf.Lerp(currentPosition.x, targetPosition.x, Time.deltaTime * 0.2f);
                currentPosition.z = Mathf.Lerp(currentPosition.z, targetPosition.z, Time.deltaTime * 0.2f);
            }

            transform.position = currentPosition;
        }
    }
}
