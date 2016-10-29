using UnityEngine;
using System.Collections;

public enum PlayState
{
    None = -1,
    Init,
    Play,
    BalanceAccounts,
    Store
}

public class InGameManager : MonoBehaviour
{
    public event CallbackGameBegin EventGameBegin;
    public event CallbackGameEnd EventGameEnd;
    public event Callback EventPauseGame;
    public event Callback EventResumeGame;
    public event Callback EventTimeLimitBegin;
    public event Callback EventTimeLimitEnd;
    public event Callback EventReBoot;
    public event Callback EventBalanceAccounts;
    public event Callback EventEnterStore;
    public event Callback EventExitStore;

    static InGameManager instance;
    public InGameManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public float maxTimeLimit;
    float leftTime;
    bool isTimeLimitUpdating;

    public PlayState playState;


    void Start()
    {
        //Event
        EventGameBegin += GameBegin;
        EventGameEnd += GameEnd;
        EventPauseGame += PauseGame;
        EventResumeGame += ResumeGame;
        EventTimeLimitBegin += TimeLimitBegin;
        EventTimeLimitEnd += TimeLimitEnd;
        EventReBoot += ReBoot;
        EventBalanceAccounts += BalanceAccounts;
        EventEnterStore += EnterStore;
        EventExitStore += ExitStore;

        Init();
    }

    void Init()
    {
        leftTime = maxTimeLimit;
        isTimeLimitUpdating = false;
        playState = PlayState.Init;
    }

    void Update()
    {

    }

    public void OnGameBegin()
    {
        if (EventGameBegin == null)
            Debug.Log("EventGameBegin is Empty");
        else
            EventGameBegin();
    }

    void GameBegin()
    {
        StartCoroutine(CoGameBegin());
    }

    IEnumerator CoGameBegin()
    {
        yield return null;
    }
    
    public void OnGameEnd(bool isWin)
    {
        if (EventGameEnd == null)
            Debug.Log("EventGameEnd is Empty");
        else
            EventGameEnd(isWin);
    }

    void GameEnd(bool isWin)
    {
        StartCoroutine(CoGameEnd());
    }

    IEnumerator CoGameEnd()
    {
        yield return null;
    }

    public void OnPauseGame()
    {
        if (EventPauseGame == null)
            Debug.Log("EventPauseGame is Empty");
        else
            EventPauseGame();
    }

    void PauseGame()
    {
        Debug.Log("PauseGame");
    }

    public void OnResumeGame()
    {
        if (EventResumeGame == null)
            Debug.Log("EventResumeGame is Empty");
        else
            EventResumeGame();
    }

    void ResumeGame()
    {
        Debug.Log("ResumeGame");
    }

    public void OnTimeLimitBegin()
    {
        if (EventTimeLimitBegin == null)
            Debug.Log("EventTimeLimitBegin is Empty");
        else
        {
            isTimeLimitUpdating = true;
            EventTimeLimitBegin();
        }
    }

    void TimeLimitBegin()
    {
        Debug.Log("TimeLimitBegin");
        StartCoroutine(CoTimeLimitUpdate());
    }

    IEnumerator CoTimeLimitUpdate()
    {
        yield return null;
        while (isTimeLimitUpdating)
        {
            yield return null;
            leftTime -= Time.deltaTime;
            if (leftTime <= 0f)
            {
                OnTimeLimitEnd();
            }
        }
    }


    public void OnTimeLimitEnd()
    {
        if(EventTimeLimitEnd == null)
        {
            Debug.Log("EventTimeLimitEnd is Empty");
        }
        else
        {
            isTimeLimitUpdating = false;
            EventTimeLimitEnd();
        }
    }

    void TimeLimitEnd()
    {
        Debug.Log("TimeLimitEnd");
    }    

    public void OnReBoot()
    {
        if (EventReBoot == null)
            Debug.Log("EventReBoot is Empty");
        else
            EventReBoot();
    }

    void ReBoot()
    {
        Debug.Log("ReBoot");
    }

    public void OnBalanceAccounts()
    {
        if (EventBalanceAccounts == null)
            Debug.Log("EventBalanceAccounts is Empty");
        else
            EventBalanceAccounts();
    }

    void BalanceAccounts()
    {
        Debug.Log("BalanceAccounts");
    }

    public void OnEnterStore()
    {
        if (EventEnterStore == null)
            Debug.Log("EventEnterStore is Empty");
        else
            EventEnterStore();
    }

    void EnterStore()
    {
        Debug.Log("EnterStore");
    }

    public void OnExitStore()
    {
        if (EventExitStore == null)
            Debug.Log("EventExitStore is Empty");
        else
            EventExitStore();
    }

    void ExitStore()
    {
        Debug.Log("ExitStore");
    }
}
