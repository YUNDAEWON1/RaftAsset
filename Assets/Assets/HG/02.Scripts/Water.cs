using UnityEngine;

public class Water : MonoBehaviour
{
    // 물결 효과에 필요한 변수들
    public float waveHeight = 0.2f;     // 물결의 높이
    public float waveSpeed = 1f;        // 물결의 속도
    public float noiseStrength = 0.1f;  // 노이즈 강도
    public float noiseWalk = 1f;        // 노이즈 걸음 속도

    // 물체를 구성하는 메시 필터, 원래 꼭짓점, 변형된 꼭짓점, 물의 높이를 저장할 변수
    public float waterLevel;           // 물의 높이
    private MeshFilter meshFilter;      // 물체의 메시 필터
    private Vector3[] originalVertices; // 원래 꼭짓점 정보
    private Vector3[] displacedVertices;// 변형된 꼭짓점 정보

    // 초기화
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();         // 물체의 메시 필터 가져오기
        originalVertices = meshFilter.mesh.vertices;     // 원래 꼭짓점 정보 가져오기
        displacedVertices = new Vector3[originalVertices.Length]; // 꼭짓점 정보 복사
        waterLevel = transform.position.y;               // 물의 높이 저장

        // 꼭짓점 정보를 복사하여 배열에 저장
        for (int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }
    }

    // 매 프레임마다 실행되는 업데이트 함수
    void Update()
    {
        // 물결 효과 계산
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            // 물결 효과 계산
            Vector3 vertex = originalVertices[i]; // 원래 꼭짓점 정보 가져오기
            // 사인 함수를 이용하여 물결 효과 계산 (originalVertices[i]의 x, y, z값을 더해서 변형을 다르게 함)
            vertex.y += Mathf.Sin(Time.time * waveSpeed + originalVertices[i].x + originalVertices[i].y + originalVertices[i].z + noiseWalk) * waveHeight;
            // Perlin Noise를 이용하여 노이즈 효과 계산
            vertex.y += Mathf.PerlinNoise(originalVertices[i].x + noiseWalk, originalVertices[i].y + Mathf.Sin(Time.time * 0.1f)) * noiseStrength;
            // 변형된 꼭짓점 정보 저장
            displacedVertices[i] = vertex;
        }

        // 물 표면 갱신
        meshFilter.mesh.vertices = displacedVertices;  // 변형된 꼭짓점 정보를 메시 필터의 꼭짓점 정보에 적용
        meshFilter.mesh.RecalculateNormals();          // 메시의 노말 벡터 계산
    }
}
