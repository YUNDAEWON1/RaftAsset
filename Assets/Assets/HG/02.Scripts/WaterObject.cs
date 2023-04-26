using UnityEngine;

public class WaterObject : MonoBehaviour
{
    public Vector3 MovingDistances = new Vector3(0.002f, 0.001f, 0f);     // 물결의 위아래 움직임 벡터
    public float speed = 1f;                                              // 위아래 움직임의 속도

    public Vector3 WaveRotations;                                         // 객체의 측면 회전 벡터
    public float WaveRotationsSpeed = 0.3f;                               // 회전 속도

    public Vector3 AxisOffsetSpeed;                                       // 축을 따라 객체의 이동 속도

    Transform actualPos;                                                  // 실제 변환(Transform)을 저장하는 변수

    void Start()
    {
        actualPos = transform;                                            // actualPos를 현재의 Transform으로 초기화
    }

    void Update()
    {
        // 축 변경
        Vector3 mov = new Vector3(
            actualPos.position.x + Mathf.Sin(speed * Time.time) * MovingDistances.x, // x축 이동
            actualPos.position.y + Mathf.Sin(speed * Time.time) * MovingDistances.y, // y축 이동
            actualPos.position.z + Mathf.Sin(speed * Time.time) * MovingDistances.z  // z축 이동
        );

        // 회전 변경
        transform.rotation = Quaternion.Euler(
            actualPos.rotation.x + WaveRotations.x * Mathf.Sin(Time.time * WaveRotationsSpeed), // x축 회전
            actualPos.rotation.y + WaveRotations.y * Mathf.Sin(Time.time * WaveRotationsSpeed), // y축 회전
            actualPos.rotation.z + WaveRotations.z * Mathf.Sin(Time.time * WaveRotationsSpeed)  // z축 회전
        );

        // 변경된 값을 적용
        transform.position = mov;

        // 축을 따라 이동
        var tran = transform.position;

        tran.x += AxisOffsetSpeed.x * Time.deltaTime; // x축 이동
        tran.y += AxisOffsetSpeed.y * Time.deltaTime; // y축 이동
        tran.z += AxisOffsetSpeed.z * Time.deltaTime; // z축 이동

        transform.position = tran;
    }
}
