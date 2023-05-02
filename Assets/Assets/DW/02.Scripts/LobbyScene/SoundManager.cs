using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    //오디오 클립 저장 배열 선언
    public AudioClip[] soundFile;

    //사운드 Volume 설정 변수
    public float soundVolume=1.0f;
    //사운드 Mute 설정 변수
    public bool isSoundMute=false;
    public Slider sl;
    public Toggle tg;
    //Sound 오브젝트 연결 변수
    public GameObject Sound;
    //Sound Ui버튼 오브젝트 연결 변수
    public GameObject PlaySoundBtn;

    AudioSource audio;

     void Awake()   //모든 레퍼런스 연결만 하는 함수로 사용
    {
        audio=GetComponent<AudioSource>();
        DontDestroyOnLoad(this.gameObject);
        LoadData();  // Start함수에서 로드를 해도 된다.
    }

    // Start is called before the first frame update
    void Start()
    {  
        SetSound();
        PlaySoundBtn.SetActive(true);
    }

    void Update()
    {
        SaveData();
    }
    public void SetSound()
    {
        soundVolume=sl.value;
        isSoundMute=tg.isOn;
        AudioSet();
    }

    void AudioSet()
    {
        audio.volume=soundVolume;
        audio.mute=isSoundMute;
    }


    //스테이지 시작시 호출되는 함수
    public void PlayBackground(int stage)
    {
        //AudioSource의 사운드 연결
        audio.clip=soundFile[stage];
        //AudioSource 셋팅
        AudioSet();
        //사운드 플레이. Mute 설정시 사운드 안나옴
        audio.Play();
    }
    
    //사운드 공용함수 정의(어디서든 동적으로 사운드 게임오브젝트 생성)
    public void PlayEffct(Vector3 pos,AudioClip sfx)
    {
        //Mute 옵션 설정시 바로 빠져나가게
        if(isSoundMute){
            return;
        }

        //게임오브젝트의 동적 생성
        GameObject _soundObj=new GameObject("sfx");
        //사운드 발생 위치 지정
        _soundObj.transform.position=pos;

        //생성한 게임오브젝트에 AudioSource 컴포넌트 추가
        AudioSource _audioSource=_soundObj.AddComponent<AudioSource>();
        
        //AudioSource 속성 설정
         //사운드 파일 연결
         _audioSource.clip=sfx;
         //설정되어있는 볼륨 연결
         _audioSource.volume=soundVolume;
         //사운드 3d 셋팅 최소범위
         _audioSource.minDistance=15.0f;
         //사운드 3d 셋팅 최대범위
         _audioSource.maxDistance=30.0f;

         //사운드 실행
         _audioSource.Play();

         //모든 사운드플레이가 종료되면 동적생성된 사운드 오브젝트 삭제.
         Destroy(_soundObj,sfx.length+0.2f);  // 사운드 길이에 0.2초정도를 더해서 사운드의 끊어짐을 방지
        
    }

    //게임 사운드데이터 저장
    public void SaveData()
    {
        PlayerPrefs.SetFloat("SOUNDVOLUME",soundVolume);
        //PlayerPrefs 클래스 내부 함수에는 bool형을 저장해주는 함수가 없다.
        //bool형 데이터는 형 변환을 해야 PlayerPrefs.SetInt() 함수를 사용가능
        PlayerPrefs.SetInt("ISSOUNDMUTE",System.Convert.ToInt32(isSoundMute));

    }

    //게임 사운드 데이터 불러오기
    public void LoadData()
    {
        sl.value=PlayerPrefs.GetFloat("SOUNDVOLUME"); // 슬라이더 UI값에 불러오기
        
        //Int 형 데이터는 bool 형으로 형 변환.
        tg.isOn=System.Convert.ToBoolean(PlayerPrefs.GetInt("ISSOUNDMUTE"));  // 토글 UI에 값 불러오기

        //게임 첫 실행 세이브 설정시 - 저장 된 값이 없으면 0
        int isSave=PlayerPrefs.GetInt("ISSAVE");
        if (isSave==0)
        {
            sl.value=1.0f;
            tg.isOn=false;

            //첫 세이브는 볼륨 1.0f 뮤트버튼 false 값으로 저장.
            SaveData();
            PlayerPrefs.SetInt("ISSAVE",1);
        }
    }


}
