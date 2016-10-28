using UnityEngine;
using System.Collections;

public class InGameManager : MonoBehaviour
{
    public event CallbackGameBegin EventGameBegin;
    public event CallbackGameEnd EventGameEnd;
    public event Callback EventTimeLimitBegin;
    public event Callback EventTimeLimitEnd;

    public static InGameManager instance;

    public float maxTimeLimit;
    float currentTimeLimit;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        EventGameBegin += GameBegin;
        EventGameEnd += GameEnd;
        EventTimeLimitBegin += TimeLimitBegin;
        EventTimeLimitEnd += TimeLimitEnd;
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

    }

    public void OnTimeLimitEnd()
    {
        EventTimeLimitEnd();
    }

    void TimeLimitEnd()
    {
        Debug.Log("TimeLimitEnd");
    }

    void TimeLimitUpdate()
    {
        currentTimeLimit -= Time.deltaTime;
    }
    

}
