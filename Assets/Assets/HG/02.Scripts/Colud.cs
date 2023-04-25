using UnityEngine;

public class Colud : MonoBehaviour
{
    public float rotationSpeed = 0.01f; // 회전 속도

    private void Update()
    {
        // 현재 y축 회전 각도 얻기
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // y축 회전 각도 증가
        currentRotation.y += rotationSpeed;

        // 회전 각도 적용
        transform.rotation = Quaternion.Euler(currentRotation);
    }
}
