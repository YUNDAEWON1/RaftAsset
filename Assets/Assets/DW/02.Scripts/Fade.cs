using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Fade : MonoBehaviour
{
   [SerializeField] private CanvasGroup IngaemeCanvas;
   [SerializeField] private CanvasGroup DieCanvas;
   [SerializeField] private CanvasGroup EndingCanvas;
   [SerializeField] private RawImage rawimg;

   [SerializeField] private VideoPlayer vp;

   [SerializeField] private AudioSource audiosource;

  public bool fadeIN=false;

  public bool isExit=false;

   [SerializeField] private Image img;
   [SerializeField] private Text txt;

   void Update()
   {
    if(fadeIN)
    {
      DieUI();
      if(IngaemeCanvas.alpha==0)
      {
        fadeIN=false;
      }
    }
    
    if(isExit)
    {
      EXIT();
      Invoke("IMG",10.0f);
      isExit=false;
    }
   }

 
  public void DieUI()
  {
    IngaemeCanvas.alpha-=Time.deltaTime;
    DieCanvas.alpha+=Time.deltaTime;
    Invoke("QuitUI",5.0f);
  }

  public void QuitUI()
  {
    PhotonNetwork.LeaveRoom();
    IngaemeCanvas.alpha=1;
    DieCanvas.alpha=0;
  }


  public void EXIT()
  {
    IngaemeCanvas.alpha=0;
    EndingCanvas.alpha=1;
    rawimg.gameObject.SetActive(true);
    vp.Play();
    audiosource.Play();
  }

  public void IMG()
  {
    img.color=new Color(255,255,255,255);
    txt.color=new Color(234,216,181,255);
  }



}

