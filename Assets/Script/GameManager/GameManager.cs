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

public enum eBossLevel
{
    None,
    Low,
    Midium,
    High
}

public class GameManager : MonoBehaviour {
    static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    public Scene currentScene;

    public int startSceneIdx = 0;
    public eBossType bossType;
    public eBossLevel bossLevel;
    public eCharType charType;

    CallbackGameEnd end;

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
        currentScene = scene;
        print(mode);
        switch (scene.name)
        {
            case "TitleScene":
                {
                    Debug.Log("TitleSceneLoaded");
                    GameObject.Find("BtnGameStart").GetComponent<Button>().onClick.AddListener(OnBtnGameStartClicked);
                }
                break;
            case "SelectBossScene":
                {
                    Debug.Log("SelectStageSceneLoaded");

                    //GameObject.Find("ImgBossA").GetComponent<Button>().onClick.AddListener()
                    GameObject.Find("BtnLowLevelBoss").GetComponent<Button>().onClick.AddListener(OnBtnLowLevelBoss);
                    GameObject.Find("BtnMidiumLevelBoss").GetComponent<Button>().onClick.AddListener(OnBtnMidiumLevelBoss);
                    GameObject.Find("BtnHighLevelBoss").GetComponent<Button>().onClick.AddListener(OnBtnHighLevelBoss);
                }
                break;
            case "SelectCharScene":
                {
                    GameObject.Find("BtnBeginInGame").GetComponent<Button>().onClick.AddListener(OnBtnBeginInGameClicked);
                    GameObject.Find("BtnBackToTitle").GetComponent<Button>().onClick.AddListener(OnBtnBackToTitleClicked);
                }
                break;
            default:
                break;
        }
    }

    //TitleScene
    void OnBtnGameStartClicked()
    {
        SceneManager.LoadScene("SelectBossScene");
    }

    //SelectBossScene
    void OnBtnLowLevelBoss()
    {
        LoadSceneSelectCharScene();
    }

    void OnBtnMidiumLevelBoss()
    {
        LoadSceneSelectCharScene();

    }

    void OnBtnHighLevelBoss()
    {
        LoadSceneSelectCharScene();

    }

    void LoadSceneSelectCharScene()
    {
        SceneManager.LoadScene("SelectCharScene");
    }

    //SelectCharScene
    void OnBtnBeginInGameClicked()
    {
        SceneManager.LoadScene("InGameScene");
    }

    void OnBtnBackToTitleClicked()
    {
        SceneManager.LoadScene("SelectBossScene");
    }



}
