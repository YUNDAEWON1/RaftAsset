using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Potato_Object : MonoBehaviour
{
    public GameManager gm;
    public InventoryManager ivenmanager;
    public float hungry = 0.3f;

    private void Awake()
    {
        ivenmanager = FindObjectOfType<InventoryManager>();
    }

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();

    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            gm.hungry += hungry;
            ivenmanager.UseSelectedItem();
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
