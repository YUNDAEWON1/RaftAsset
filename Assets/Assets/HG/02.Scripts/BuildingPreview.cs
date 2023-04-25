using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    public GameObject[] buldingPrefab; // 생성할 빌딩 프리펩
    public Mesh[] meshes;

    public Material[] materials; // 빌딩 프리팹에 할당된 머티리얼 배열
    public RaycastHit raycast;

    private int buildingIndex = 0; // 현재 빌딩 인덱스
    private int meshIndex = 0; // 현재 메쉬 인덱스
    private int materialIndex = 0; // 현재 머티리얼 인덱스

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            buildingIndex++; // 다음 빌딩 인덱스로 변경

            if (buildingIndex >= buldingPrefab.Length) // 인덱스가 배열 크기를 초과하면
            {
                buildingIndex = 0; // 처음 빌딩으로 변경
            }

            meshIndex = buildingIndex; // 빌딩 인덱스와 동일한 메쉬 인덱스 할당
            materialIndex = buildingIndex; // 빌딩 인덱스와 동일한 머티리얼 인덱스 할당
            GetComponent<MeshFilter>().mesh = meshes[meshIndex]; // 현재 메쉬 할당
            GetComponent<MeshRenderer>().material = materials[materialIndex]; // 현재 머티리얼 할당
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            buildingIndex--; // 이전 빌딩 인덱스로 변경
            if (buildingIndex < 0) // 인덱스가 0보다 작아지면
            {
                buildingIndex = buldingPrefab.Length - 1; // 마지막 빌딩으로 변경
            }
            meshIndex = buildingIndex; // 빌딩 인덱스와 동일한 메쉬 인덱스 할당
            materialIndex = buildingIndex; // 빌딩 인덱스와 동일한 머티리얼 인덱스 할당
            GetComponent<MeshFilter>().mesh = meshes[meshIndex]; // 현재 메쉬 할당
            GetComponent<MeshRenderer>().material = materials[materialIndex]; // 현재 머티리얼 할당
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.Rotate(new Vector3(0f, 90f, 0f));
            // 현재 회전 각도가 -360 이하일 경우 360을 더해주고, 360 이상일 경우 360을 빼줌
            float currentAngle = transform.rotation.eulerAngles.y;
            if (currentAngle < -360f)
            {
                currentAngle += 360f;
            }
            else if (currentAngle > 360f)
            {
                currentAngle -= 360f;
            }
            // 회전 범위를 -360 ~ +360으로 설정
            if (currentAngle < -360f || currentAngle > 360f)
            {
                Debug.LogWarning("회전 범위를 벗어났습니다.");
            }
            else
            {
                currentAngle = Mathf.Clamp(currentAngle, -360f, 360f);
                transform.rotation = Quaternion.Euler(new Vector3(0f, currentAngle, 0f));
            }
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Instantiate(buldingPrefab[buildingIndex], transform.position, transform.rotation);
        //}

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            // 아래 방향 검사
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
            {
                Debug.Log("빌딩 생성 위치에 이미 오브젝트가 있습니다.");
                Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.red);
                return;
            }

            // 오른쪽 방향 검사
            Vector3 startPos = transform.position;
            Vector3 direction = transform.right;
            float distance = 3f;

            if (Physics.Raycast(startPos, direction, out hit, distance))
            {
                Debug.Log("오른쪽에 오브젝트가 있습니다.");
                Debug.DrawRay(startPos, direction * hit.distance, Color.red);
                return;
            }

            // 왼쪽 방향 검사
            direction = -transform.right;

            if (Physics.Raycast(startPos, direction, out hit, distance))
            {
                Debug.Log("왼쪽에 오브젝트가 있습니다.");
                Debug.DrawRay(startPos, direction * hit.distance, Color.red);
                return;
            }

            // 검사를 모두 통과한 경우 빌딩 생성
            Instantiate(buldingPrefab[buildingIndex], transform.position, transform.rotation);
        }

        else
        {
            // 마우스 왼쪽 버튼을 누르지 않은 경우 레이캐스트 그리기
            Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector3.down * 3f, Color.white);
            Debug.DrawRay(transform.position + Vector3.up * 0.1f, transform.right * 3f, Color.white);
            Debug.DrawRay(transform.position + Vector3.up * 0.1f, -transform.right * 3f, Color.white);
            // gittest
        }

    }
}