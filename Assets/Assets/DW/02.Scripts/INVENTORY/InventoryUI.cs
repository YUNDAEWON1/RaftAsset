using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject toolPanel;

    public Image aim;
    public Image aimSlider;
    public CursorLockMode cursorLockMode = CursorLockMode.Locked; // 마우스 커서 상태

    private bool activeInventory = false;

    private bool isAiming = false;
    private float aimTime = 0f;
    private float maxAimTime = 2f; // aim 게이지가 모두 채워지는 데 걸리는 시간

    private void Start()
    {
        inventoryPanel.SetActive(activeInventory);
        toolPanel.SetActive(false); // 시작할 때는 ToolPanel을 비활성화
        Cursor.lockState = cursorLockMode; // 시작할 때 마우스 커서 상태를 설정
        aimSlider.gameObject.SetActive(false); // 시작할 때는 aim 게이지를 비활성화
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            activeInventory = !activeInventory;
            inventoryPanel.SetActive(activeInventory);

            // ToolPanel은 activeInventory가 true일 때만 활성화
            toolPanel.SetActive(activeInventory);

            // aim 이미지와 마우스 커서 상태를 인벤토리 활성화 여부에 따라 변경
            if (activeInventory)
            {
                aim.enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                aim.enabled = true;
                Cursor.lockState = cursorLockMode;
                Cursor.visible = false;
            }
        }

        // if(Input.GetKeyDown(KeyCode.Escape))
        // {
        //     activeEscPanel = !activeEscPanel;
        //     EscPanel.SetActive(activeEscPanel);

        //     // aim 이미지와 마우스 커서 상태를 인벤토리 활성화 여부에 따라 변경
        //     if (activeEscPanel)
        //     {
        //         aim.enabled = false;
        //         Cursor.lockState = CursorLockMode.None;
        //         Cursor.visible = true;
        //     }
        //     else
        //     {
        //         aim.enabled = true;
        //         Cursor.lockState = cursorLockMode;
        //         Cursor.visible = false;
        //     }
        // }

        // 마우스 좌클릭이 눌린 경우
        if (Input.GetMouseButton(0))
        {
            isAiming = true;
            aimTime += Time.deltaTime;
            aimSlider.gameObject.SetActive(true);
            aimSlider.fillAmount = aimTime / maxAimTime;

            if (aimTime >= maxAimTime)
            {
                // aim 게이지가 모두 채워졌을 때 실행될 코드 작성
            }
        }
        // 마우스 좌클릭이 떨어진 경우
        else if (isAiming)
        {
            isAiming = false;
            aimTime = 0f;
            aimSlider.gameObject.SetActive(false);
        }
    }
}
