using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public float time;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    /*
    void Update()
    {
        TimeUpdate();
    }

    void TimeUpdate()
    {
        time += Time.deltaTime;
    }
    */

}
