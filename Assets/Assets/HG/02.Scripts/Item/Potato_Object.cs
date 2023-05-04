using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Potato_Object : MonoBehaviour
{
    //public GameManager gm;
    public InventoryManager ivenmanager;
    //private InteractionObject interSc;
    private PlayerCtrl pCtrl;
    public float hungry = 0.3f;

    private void Awake()
    {
        //interSc = GetComponent<InteractionObject>();
        ivenmanager = FindObjectOfType<InventoryManager>();
    }

    public void EatPotato(int viewID)
    {
        PhotonView.Find(viewID).gameObject.GetComponent<PlayerCtrl>().hungry += hungry;
        ivenmanager.UseSelectedItem();
        PhotonNetwork.Destroy(this.gameObject);
        return;
    }
}
