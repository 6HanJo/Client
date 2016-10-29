using UnityEngine;
using System.Collections;

public class InGameManager : MonoBehaviour
{
    public event CallbackGameBegin EventGameBegin;
    public event CallbackGameEnd EventGameEnd;
    public event Callback EventTimeLimitBegin;
    public event Callback EventTimeLimitEnd;
    public event Callback EventReBoot;

    public static InGameManager instance;

    public float maxTimeLimit;
    float currentTimeLimit;
    bool isTimeLimitUpdating;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        //Event
        EventGameBegin += GameBegin;
        EventGameEnd += GameEnd;
        EventTimeLimitBegin += TimeLimitBegin;
        EventTimeLimitEnd += TimeLimitEnd;

        Init();
    }

    void Init()
    {
        currentTimeLimit = maxTimeLimit;
        isTimeLimitUpdating = false;
    }

    void Update()
    {

    }

    public void OnGameBegin()
    {
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

    public void OnTimeLimitBegin()
    {
        EventTimeLimitBegin();
    }

    void TimeLimitBegin()
    {
        Debug.Log("TimeLimitBegin");
        isTimeLimitUpdating = true;
        StartCoroutine(CoTimeLimitUpdate());
    }

    IEnumerator CoTimeLimitUpdate()
    {
        yield return null;
        while(isTimeLimitUpdating)
        {
            currentTimeLimit -= Time.deltaTime;
            yield return null;
        }
    }


    public void OnTimeLimitEnd()
    {
        EventTimeLimitEnd();
    }

    void TimeLimitEnd()
    {
        Debug.Log("TimeLimitEnd");
        isTimeLimitUpdating = false;
    }    

}
