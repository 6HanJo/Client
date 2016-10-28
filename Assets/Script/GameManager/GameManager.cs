using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum eCharType
{
    None,
    CharA,
    CharB,
    CharC
}

public enum eBossType
{
    None,
    BossA,
    BossB,
    BossC
}

public class GameManager : MonoBehaviour {
    static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    public int startSceneIdx = 0;
    public eBossType bossType;
    public eCharType charType;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        //GameObject.Find("BtnGameStart").GetComponent<Button>().onClick.AddListener(OnBtnGameStartClicked);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(startSceneIdx);

        Init();
    }

    void Init()
    {
        bossType = eBossType.None;
        charType = eCharType.None;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        print(mode);
        switch (scene.name)
        {
            case "TitleScene":
                {
                    Debug.Log("TitleSceneLoaded");
                    GameObject.Find("BtnGameStart").GetComponent<Button>().onClick.AddListener(OnBtnGameStartClicked);
                }
                break;
            case "SelectStageScene":
                {
                    Debug.Log("SelectStageSceneLoaded");
                    GameObject.Find("BtnBeginInGame").GetComponent<Button>().onClick.AddListener(OnBtnBeginInGameClicked);
                    GameObject.Find("BtnBackToTitle").GetComponent<Button>().onClick.AddListener(OnBtnBackToTitleClicked);

                    //GameObject.Find("ImgBossA").GetComponent<Button>().onClick.AddListener()
                }
                break;
            default:
                break;
        }
    }

    void OnBtnGameStartClicked()
    {
        SceneManager.LoadScene("SelectStageScene");
    }

    void OnBtnBeginInGameClicked()
    {
        Debug.Log("StartGame");
    }

    void OnBtnBackToTitleClicked()
    {
        SceneManager.LoadScene("TitleScene");
    }



}
