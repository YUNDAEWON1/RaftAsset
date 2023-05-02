using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject hungry_Gage;
    public GameObject thirsty_Gage;
    public GameObject hpGage;

    private Image hungryGageImage;
    private Image thirstyGageImage;
    private Image hpGageImage;


    public float hungry;
    public float thirsty;
    public float hp;

    private void Awake()
    {
        hungryGageImage = hungry_Gage.GetComponent<Image>();
        thirstyGageImage = thirsty_Gage.GetComponent<Image>();
        hpGageImage = hpGage.GetComponent<Image>();
    }

    private void Update()
    {
        Hungry();
        Thirsty();
    }

    void Hungry()
    {
        if (hungry > 0)
        {
            hungry -= Time.deltaTime * 0.05f;

            if (hungry < 0)
            {
                hungry = 0;
            }
        }

        if (hungry <= 0.0f)
        {
            Hp();
        }
        else
        {
            hungryGageImage.fillAmount = hungry;
        }
    }

    void Thirsty()
    {
        if (thirsty > 0)
        {
            thirsty -= Time.deltaTime * 0.05f;

            if (thirsty < 0)
            {
                thirsty = 0;
            }
        }

        if (thirsty<= 0.0f)
        {
            Hp();
        }
        else
        {
            thirstyGageImage.fillAmount = thirsty;
        }
    }

    public void Hp()
    {
        //if (hungry <= 0 || thirsty <= 0)
        //{
            hp -= Time.deltaTime * 0.05f;

            if (hp < 0)
            {
                hp = 0;
            }
        //}

        if (hp <= 0.0f)
        {
            Time.timeScale = 0;
        }

        else
        {
            hpGageImage.fillAmount = hp;
        }

    }
}
