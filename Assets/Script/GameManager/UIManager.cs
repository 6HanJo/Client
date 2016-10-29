using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    static UIManager instance;
    public UIManager Instance
    {
        get { return instance; }
    }

    InGameManager inGameManager;

    void Awake()
    {
        instance = this;
    }

    public Text textTotalGold;

    public Text textBossHP;
    public Slider sliderBossHP;

    public Text textLeftTime;
    public Slider sliderLimitTime;

    public Image imgPlayerChar;

    void Start()
    {
        inGameManager = GameObject.Find("InGameManager").GetComponent<InGameManager>();

        SetTextTotalGold(2);
        SetMaxSliderBossHP(100);
        SetTextLeftTime(2f);
    }

    public IEnumerator CoUpdateSliderLimitTime()
    {
        yield return null;
        while(true)
        {
            yield return null;
            if(inGameManager.isPuaseGame == false)
            {
                sliderLimitTime.value = inGameManager.leftTime;
            }
        }
    }

    public IEnumerator CoUpdateTextLeftTime()
    {
        yield return new WaitForSeconds(1f);
        if(inGameManager.isPuaseGame == false)
        {
            SetTextLeftTime(inGameManager.leftTime);
        }
        StartCoroutine(CoUpdateTextLeftTime());
    }

    public IEnumerator CoUpdateReBoot()
    {
        yield return null;
        //연출
    }



    public void SetTextTotalGold(int totalGold)
    {
        textTotalGold.text = totalGold.ToString();
    }
    
    //UI 보스 최대 체력값 설저아
    public void SetMaxSliderBossHP(int maxBossHp)
    {
        sliderBossHP.maxValue = maxBossHp;
        sliderBossHP.value = maxBossHp;
    }

    public void SetTextBossHP(int bossHP)
    {
        textBossHP.text = bossHP.ToString();
    }

    //UI 최대 남은 시간 설정
    public void SetMaxSliderLimitTime(float limitTime)
    {
        sliderLimitTime.maxValue = limitTime;
        sliderLimitTime.value = limitTime;
    }

    public void SetTextLeftTime(float leftTime)
    {
        textLeftTime.text = leftTime.ToString("##0.");
    }
}
