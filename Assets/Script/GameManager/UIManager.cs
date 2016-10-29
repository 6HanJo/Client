using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour {
    static UIManager instance;
    static public UIManager Instance
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
    public EnergyBar sliderBossHP;

    public Text textLeftTime;
    public EnergyBar sliderLimitTime;

    public Image imgPlayerChar;

    public RectTransform panelStoreUI;

    public GameObject goImgReBoot;

    bool isPause = false;

    void Start()
    {
        inGameManager = GameObject.Find("InGameManager").GetComponent<InGameManager>(); 

        SetTextTotalGold(0);
        SetMaxSliderBossHP(100);
        SetTextLeftTime(InGameManager.Instance.maxTimeLimit);
    }

    public IEnumerator CoUIUpdateLeftTime()
    {
        yield return new WaitForSeconds(1f);
        if(inGameManager.isPuaseGame == false)
        {
            SetTextLeftTime(inGameManager.leftTime);
            sliderLimitTime.SetValueCurrent(inGameManager.leftTime);
        }
        StartCoroutine(CoUIUpdateLeftTime());
    }

    public IEnumerator CoUIReBoot()
    {
        yield return null;
        //연출
        goImgReBoot.SetActive(true);
        yield return new WaitForSeconds(1f);
        goImgReBoot.SetActive(false);
    }

    public IEnumerator CoUIEnterBalanceAccounts()
    {
        yield return null;
    }

    public IEnumerator CoUIExitBalanceAccounts()
    {
        yield return null;
    }

    public IEnumerator CoUIEnterScore()
    {
        yield return null;
        panelStoreUI.DOAnchorPos(Vector2.zero, 1f);
    }

    public IEnumerator CoUIExitScore()
    {
        yield return null;
        panelStoreUI.DOAnchorPos(new Vector2(0f, -500f), 1f);
    }

    public void SetTextTotalGold(int totalGold)
    {
        textTotalGold.text = totalGold.ToString();
    }
    
    //UI 보스 최대 체력값 설정
    public void SetMaxSliderBossHP(int maxBossHp)
    {
        sliderBossHP.SetValueMax(maxBossHp);
        sliderBossHP.SetValueCurrent(maxBossHp);
    }

    public void SetTextBossHP(int bossHP)
    {
        textBossHP.text = bossHP.ToString();
    }

    //UI 최대 남은 시간 설정
    public void SetMaxSliderLimitTime(int limitTime)
    {
        sliderLimitTime.SetValueMax(limitTime);
        sliderLimitTime.SetValueCurrent(limitTime);
    }

    public void SetTextLeftTime(int leftTime)
    {
        if (leftTime < 0)
            return;
        textLeftTime.text = leftTime.ToString("##0.");
    }
}
