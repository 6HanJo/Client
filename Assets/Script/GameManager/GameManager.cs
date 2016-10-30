using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum eCharType
{
    None = -1,
    CharA,
    CharB,
    CharC
}

public enum eBossType
{
    None = -1,
    BossA,
    BossB,
    BossC
}

public enum eBossLevel
{
    None = -1,
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

    public float onceAddGoldBonus = 0.1f;
    public int addGoldBonusPlusTime = 10;
    public float goldBonus = 0;

    public int totalGold = 0;

    //SelectBossScene
    Button[] arrBtnBoss;
    Button[] arrBtnLevel;

    //SelectCharScene
    Button[] arrBtnChar;
    Button btnBeginInGame;

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
       // SceneManager.LoadScene(startSceneIdx);

        //SelectBossScene
        arrBtnBoss = new Button[3];
        arrBtnLevel = new Button[3];

        //SelectCharScene
        arrBtnChar = new Button[3];

        Init();
    }

    void Init()
    {
        for (int i = 0; i < 3; i++)
        {
            arrBtnLevel[i] = null;
        }
        bossType = eBossType.None;
        bossLevel = eBossLevel.None;
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
                    Debug.Log("SelectBossSceneLoaded");

                    //GameObject.Find("ImgBossA").GetComponent<Button>().onClick.AddListener()
                    bossType = eBossType.None;

                    arrBtnBoss[(int)eBossType.BossA] = GameObject.Find("BtnBossA").GetComponent<Button>();
                    arrBtnBoss[(int)eBossType.BossB] = GameObject.Find("BtnBossB").GetComponent<Button>();
                    arrBtnBoss[(int)eBossType.BossC] = GameObject.Find("BtnBossC").GetComponent<Button>();

                    arrBtnBoss[(int)eBossType.BossA].onClick.AddListener(OnBtnBossAClicked);
                    arrBtnBoss[(int)eBossType.BossB].onClick.AddListener(OnBtnBossBClicked);
                    arrBtnBoss[(int)eBossType.BossC].onClick.AddListener(OnBtnBossCClicked);

                    /*
                    arrBtnLevel[(int)eBossLevel.Low] = GameObject.Find("BtnLowLevelBoss").GetComponent<Button>();
                    arrBtnLevel[(int)eBossLevel.Midium] = GameObject.Find("BtnMidiumLevelBoss").GetComponent<Button>();
                    arrBtnLevel[(int)eBossLevel.High] = GameObject.Find("BtnHighLevelBoss").GetComponent<Button>();

                    arrBtnLevel[(int)eBossLevel.Low].onClick.AddListener(OnBtnLowLevelBossClicked);
                    arrBtnLevel[(int)eBossLevel.Midium].onClick.AddListener(OnBtnMidiumLevelBossClicked);
                    arrBtnLevel[(int)eBossLevel.High].onClick.AddListener(OnBtnHighLevelBossClicked);
                    */
                }
                break;
            case "SelectCharScene":
                {
                    Debug.Log("SelectCharSceneLoaded");

                    charType = eCharType.None;
                    btnBeginInGame = GameObject.Find("BtnBeginInGame").GetComponent<Button>();

                    btnBeginInGame.onClick.AddListener(OnBtnBeginInGameClicked);
                    GameObject.Find("BtnBackToTitle").GetComponent<Button>().onClick.AddListener(OnBtnBackToTitleClicked);

                    arrBtnChar[(int)eCharType.CharA] = GameObject.Find("BtnCharA").GetComponent<Button>();
                    arrBtnChar[(int)eCharType.CharB] = GameObject.Find("BtnCharB").GetComponent<Button>();
                    arrBtnChar[(int)eCharType.CharC] = GameObject.Find("BtnCharC").GetComponent<Button>();
                    
                    arrBtnChar[(int)eCharType.CharA].onClick.AddListener(OnBtnCharAClicked);
                    arrBtnChar[(int)eCharType.CharB].onClick.AddListener(OnBtnCharBClicked);
                    arrBtnChar[(int)eCharType.CharC].onClick.AddListener(OnBtnCharCClicked);
                }
                break;
            default:
                break;
        }
    }

    //TitleScene
    void OnBtnGameStartClicked()
    {
        Debug.Log("shit");
        SceneManager.LoadScene("SelectBossScene");
    }

    void OnBtnBossAClicked()
    {
        //velButtonInteractable(eBossType.BossA);
        bossType = eBossType.BossA;
        LoadSceneSelectCharScene();
    }

    void OnBtnBossBClicked()
    {
        //velButtonInteractable(eBossType.BossB);
        bossType = eBossType.BossB;
        LoadSceneSelectCharScene();
    }

    void OnBtnBossCClicked()
    {
        //LevelButtonInteractable(eBossType.BossC);
        bossType = eBossType.BossC;
        LoadSceneSelectCharScene();
    }

    /*
    void LevelButtonInteractable(eBossType type)
    {
        if (bossType == type)
            return;
        for (int i = 0; i < 3; i++)
        {
            arrBtnLevel[i].interactable = true;
        }
        GameObject.Find("ImgSelector").transform.position = arrBtnBoss[(int)type].transform.position;
    }

    */

    //SelectBossScene
    void OnBtnLowLevelBossClicked()
    {
        LoadSceneSelectCharScene();
        bossLevel = eBossLevel.Low;
    }

    void OnBtnMidiumLevelBossClicked()
    {
        LoadSceneSelectCharScene();
        bossLevel = eBossLevel.Midium;
    }

    void OnBtnHighLevelBossClicked()
    {
        LoadSceneSelectCharScene();
        bossLevel = eBossLevel.High;
    }

    void LoadSceneSelectCharScene()
    {
        SceneManager.LoadScene("SelectCharScene");
    }

    //SelectCharScene
    void OnBtnCharAClicked()
    {
        CharButtonInteractable(eCharType.CharA);
        charType = eCharType.CharA;
    }

    void OnBtnCharBClicked()
    {
        CharButtonInteractable(eCharType.CharB);
        charType = eCharType.CharB;
    }

    void OnBtnCharCClicked()
    {
        CharButtonInteractable(eCharType.CharC);
        charType = eCharType.CharC;
    }

    void CharButtonInteractable(eCharType type)
    {
        if (charType == type)
            return;

        btnBeginInGame.interactable = true;
        GameObject.Find("ImgSelector").transform.position = arrBtnChar[(int)type].transform.position;
    }

    void OnBtnBeginInGameClicked()
    {
        Debug.Log("BossType : " + bossType);
        Debug.Log("BossLevel : " + bossLevel);
        Debug.Log("Character Type : " + charType);
        SceneManager.LoadScene("InGameScene");

        switch (bossType)
        {
            case eBossType.BossA:
			
                SceneManager.LoadScene(5, LoadSceneMode.Additive);
                break;
            case eBossType.BossB:
                SceneManager.LoadScene(6, LoadSceneMode.Additive);
                break;
            case eBossType.BossC:
                SceneManager.LoadScene(7, LoadSceneMode.Additive);
                break;
            default:
                break;
        }
    }

    void OnBtnBackToTitleClicked()
    {
        SceneManager.LoadScene("SelectBossScene");
    }



}
