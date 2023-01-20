using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(KeyController))]
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                // monobehaviour를 안 쓴다면 
                // instance = new GameManager();

                GameObject go = new GameObject();
                go.name = "GameManager";
                instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    // [SerializeField]
    // private GameObject goBullet;
    public GameObject Canvas;
    public List<Vector2> Guide = new List<Vector2>();
    // private int index = 1;
    // public float flTimer = 0f;
    // public float flLvTime = 0.1f;
    // public Vector2 guideVec;

    public int playerHP = 4;
    public int playerMaxHP = 4;
    public int fullMaxHP = 8;
    public float score = 0;
    public int playerDmg = 1;
    public int fullPlayerDmg = 10;
    public int skillDmg = 1;
    public float playerSpeed = 2f;
    public float fullPlayerSpeed = 10f;
    public float playerShotSpeed = 0f;
    public float playerBulletScale = 0f;
    public float playerBulletRange = 0f;
    public float playerCoolTime = 0f;
    public float playerColdShotLv = 0f;
    public float playerFireShotLv = 0f;
    public float playerSparkShotLv = 0f;
    public float playerPushPower = 1f;
    public float playerSkillDamageLv = 1f;

    public float gameTimer = 0f;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        // 可変解像度
        // float ratio = 1f; // 16 : 9 の時好きな比率に変えるため（強制に１：１にしたり）
        // Screen.SetResolution(Screen.width, Screen.height * ratio, true);

        // Screen.width(height)は最初はデバイスの解像度が基準だが
        // setResolution が実行されてからは設定した解像度を参照する

        Canvas = GameObject.Find("Canvas");
        Vector2 center = Canvas.GetComponent<Canvas>().GetComponent<RectTransform>().anchoredPosition;

        // 1200 x 1920 --> center : x-600 y-960
        float minX = center.x - (Screen.width / 2f) - 0.5f;
        // float minX = center.x - 600;
        float maxX = center.x + (Screen.width / 2f) + 0.5f;
        // float maxX = center.x + 600;
        float minY = center.y - (Screen.height / 2f) - 0.5f;
        // float minY = center.y - 960;
        float maxY = center.y + (Screen.height / 2f) + 0.5f;
        // float maxY = center.y + 960;

        // 1200 x 1920に設定したい --> 固定解像度 
        Screen.SetResolution(1920, 1200, true);

        Guide.Add(new Vector2(minX, minY));
        Guide.Add(new Vector2(minX, maxY));
        Guide.Add(new Vector2(maxX, maxY));
        Guide.Add(new Vector2(maxX, minY));

        PoolManager.Instance.StartGame();
        UIManager.Instance.InitScore();
        UIManager.Instance.InitHP();
        playerDmg = 1;
        playerSkillDamageLv = 1;
        playerSpeed = 2;
        playerShotSpeed = 0;
        playerBulletScale = 0;
        playerBulletRange = 0;
        playerColdShotLv = 0;
        playerFireShotLv = 0;
        playerSparkShotLv = 0;

        // UpgradeList.cs

        gameTimer = 0f;

        SkillManager.Instance.Init();

        Cursor.lockState = CursorLockMode.None;
    }

    public Joystick JoystickM
    {
        get;
        private set;
    }

    public Skillstick AttackJoystickM
    {
        get;
        private set;
    }

    public Skillstick SkillJoystickM
    {
        get;
        private set;
    }

    public Character CharacterM
    {
        get;
        private set;
    }

    public void InitJoystick(Joystick j)
    {
        JoystickM = j;
    }

    public void InitAttackJoystick(Skillstick s)
    {
        AttackJoystickM = s;
    }

    public void InitSkillJoystick(Skillstick s)
    {
        SkillJoystickM = s;
    }

    public void InitCharacter(Character c)
    {
        CharacterM = c;
    }

    public void StopTime()
    {
        Time.timeScale = 0;
    }

    public void StartTime()
    {
        Time.timeScale = 1;
    }

    public void ReStartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadTheScene(Scene scene)
    {
        SceneManager.LoadScene(scene.name);
    }

    private void Update()
    {
        gameTimer += Time.deltaTime;
        // Debug.Log($"curTime : {gameTimer}");
    }
}
