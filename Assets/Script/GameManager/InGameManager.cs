﻿using UnityEngine;
using System.Collections;
using PathologicalGames;

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
    static public InGameManager Instance
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

    public UIManager uiManager;

    public int maxTimeLimit;
    public int leftTime;
    public bool isTimeLimitUpdating;
    public int[] skillBuild;

    public PlayState playState;

    public bool isPuaseGame = false;
	public int bossHp = 0;
	public int[] arrBossHp;

	public BossInfo bossInfo;

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

        OnGameBegin();
    }

    void Init()
    {
		GameManager.Instance.bossType = eBossType.BossC;
		switch (GameManager.Instance.bossType) {
		case eBossType.BossA:
			bossHp = arrBossHp [0];
			break;
		case eBossType.BossB:
			bossHp = arrBossHp[1];
			break;
		case eBossType.BossC:
			bossHp = arrBossHp[2];
			break;
		}
		uiManager.SetMaxSliderBossHP (bossHp);

		bossInfo = GameObject.FindGameObjectWithTag ("Boss").GetComponent<BossInfo>();
		bossInfo.hp = bossHp;

        leftTime = maxTimeLimit;
        uiManager.SetMaxSliderLimitTime(maxTimeLimit);

        isTimeLimitUpdating = false;
        playState = PlayState.Init;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(playState == PlayState.Play)
                OnReBoot();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPuaseGame == false)
                OnPauseGame();
            else
                OnResumeGame();
        }
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
        yield return new WaitForSeconds(1f);
        playState = PlayState.Play;
        OnTimeLimitBegin();
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
        Debug.Log("GameOver is Win : " + isWin);
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
        isPuaseGame = true;
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
        isPuaseGame = false;
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
        StartCoroutine(uiManager.CoUIUpdateLeftTime());

        while (isTimeLimitUpdating)
        {
            
            if(isPuaseGame == false)
            {
                leftTime -= 1;
                if (leftTime < 0)
                {
                    OnTimeLimitEnd();
                }
            }
            yield return new WaitForSeconds(1f);
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
        OnGameEnd(false);
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
        OnPauseGame();
        StartCoroutine(CoReBoot());
    }

    IEnumerator CoReBoot()
    {
        yield return null;
        //리붓 연출
        PoolManager.Pools["Test"].DespawnAll();

        
        yield return StartCoroutine(uiManager.CoUIReBoot());
        OnBalanceAccounts();
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
        StartCoroutine(CoBalanceAccounts());
    }

    IEnumerator CoBalanceAccounts()
    {
        yield return null;
        yield return StartCoroutine(uiManager.CoUIEnterBalanceAccounts());
        
        yield return StartCoroutine(uiManager.CoUIExitBalanceAccounts());

        OnEnterStore();
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
        StartCoroutine(CoEnterStore());
    }

    IEnumerator CoEnterStore()
    {
        yield return null;
        yield return StartCoroutine(uiManager.CoUIEnterScore());

        playState = PlayState.Store;
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
        StartCoroutine(CoExitStore());
    }

    IEnumerator CoExitStore()
    {
        yield return null;
        yield return StartCoroutine(uiManager.CoUIExitScore());
        OnResumeGame();
        playState = PlayState.Play;
    }
}
