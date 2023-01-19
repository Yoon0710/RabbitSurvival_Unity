using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                // monobehaviour를 안 쓴다면 
                // instance = new GameManager();
                GameObject go = GameObject.Find("Canvas");

                if (go == null)
                {
                    go = new GameObject();
                    go.AddComponent<Canvas>();
                    go.name = "Canvas";
                }
                instance = go.AddComponent<UIManager>();
            }
            return instance;
        }
    }

    private TextMeshProUGUI lvUI;
    private TextMeshProUGUI expUI;
    private TextMeshProUGUI scoreUI;
    private GameObject statsUI;
    public GameObject gameOverUI;
    public GameObject warningUI;
    private List<Image> hearts = new List<Image>();
    private LevelSetting lvSetting;
    public List<Button> btns = new List<Button>();
    public Button attackBtn;
    public Button skillBtn;

    bool isWarning = false;

    GameObject LvUpUI;
    RectTransform cancelBtnRT;
    Color fingerC;

    ScreenOrientation screenOrientation;

    void Awake()
    {
        screenOrientation = ScreenOrientation.Portrait;
        lvUI = GameObject.Find("LV").GetComponent<TextMeshProUGUI>();
        expUI = GameObject.Find("Exp").GetComponent<TextMeshProUGUI>();
        scoreUI = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
        GameObject heartsGroup = transform.Find("Panel").Find("HP").gameObject;
        lvSetting = GameObject.Find("Character").GetComponent<LevelSetting>();
        attackBtn = GameObject.Find("ButtonAtk").GetComponent<Button>();
        skillBtn = GameObject.Find("ButtonSkill").GetComponent<Button>();
        gameOverUI = GameObject.Find("Result");
        gameOverUI.GetComponentInChildren<Button>().onClick.AddListener(GameManager.Instance.ReStartGame);
        gameOverUI.SetActive(false);
        statsUI = GameObject.Find("Stats");
        warningUI = GameObject.Find("Warning");
        GameObject.Find("Warning").SetActive(false);

        // LvUpUI = GameObject.Find("LvUP");
        // fingerC = GameObject.Find("Finger").GetComponent<Image>().color;
        // cancelBtnRT = GameObject.Find("NoChoose").GetComponent<RectTransform>();

        foreach (Button b in GetComponentsInChildren<Button>())
        {
            btns.Add(b);
        }

        foreach (Image heart in heartsGroup.GetComponentsInChildren<Image>())
        {
            hearts.Add(heart);
        }
    }

    public void InitScore()
    {
        GameManager.Instance.score = 0;
    }

    public void AddScore()
    {
        GameManager.Instance.score += Time.deltaTime;
    }

    public void InitHP()
    {
        GameManager.Instance.playerHP = 4;
        GameManager.Instance.playerMaxHP = 4;
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i <= GameManager.Instance.playerMaxHP - 1)
            {
                hearts[i].gameObject.SetActive(true);
                hearts[i].color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
            }
            else
            {
                hearts[i].color = new Color(75 / 255f, 75 / 255f, 75 / 255f, 75 / 255f);
                hearts[i].gameObject.SetActive(false);
            }
        }
    }

    public void HPDown()
    {
        if (GameManager.Instance.playerHP <= 0)
            return;

        GameManager.Instance.playerHP--;
        hearts[GameManager.Instance.playerHP].color = new Color(75 / 255f, 75 / 255f, 75 / 255f, 75 / 255f);
    }

    public void HPUP(int n = 1)
    {
        if (GameManager.Instance.playerHP >= GameManager.Instance.playerMaxHP)
            return;

        for (int i = 0; i < n; i++)
        {
            hearts[GameManager.Instance.playerHP].color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
            GameManager.Instance.playerHP++;
        }
    }

    public void MaxHPUP(int n = 1)
    {
        for (int i = 0; i < n; i++)
        {
            GameManager.Instance.playerMaxHP++;
            hearts[GameManager.Instance.playerMaxHP - 1].gameObject.SetActive(true);
            hearts[GameManager.Instance.playerMaxHP - 1].color = new Color(75 / 255f, 75 / 255f, 75 / 255f, 75 / 255f);
        }
    }

    public void GameOver()
    {
        UIManager.Instance.gameOverUI.SetActive(true);
        TextMeshProUGUI resultT = gameOverUI.transform.Find("Panel").GetComponentInChildren<TextMeshProUGUI>();
        resultT.text = $"your score is <b><color=yellow>{GameManager.Instance.score:#00}</color></b>!";
        GameManager.Instance.StopTime();
    }

    public void ChangeAtkButtonImage(GameObject skill)
    {
        Image atkBtnImg = attackBtn.transform.Find("Image").GetComponent<Image>();
        atkBtnImg.sprite = skill.GetComponent<SpriteRenderer>().sprite;
        atkBtnImg.color = skill.GetComponent<SpriteRenderer>().color;
        if (skill.transform.localScale.x < 0)
            atkBtnImg.transform.localScale = new Vector3(Vector3.left.x * atkBtnImg.transform.localScale.x, atkBtnImg.transform.localScale.y, atkBtnImg.transform.localScale.z);
    }

    public void ChangeSklButtonImage(GameObject skill)
    {
        Image sklBtnImg = skillBtn.transform.Find("Image").GetComponent<Image>();
        sklBtnImg.sprite = skill.GetComponent<SpriteRenderer>().sprite;
        sklBtnImg.color = skill.GetComponent<SpriteRenderer>().color;
        if (skill.transform.localScale.x < 0)
            sklBtnImg.transform.localScale = new Vector3(Vector3.left.x * sklBtnImg.transform.localScale.x, sklBtnImg.transform.localScale.y, sklBtnImg.transform.localScale.z);
    }

    public void MakedUpgradeBtn(GameObject btn, Upgrade upgrade, int skillIdx = -1, Sprite image = null)
    {
        Color imageC = new Color(142 / 255f, 142 / 255f, 142 / 255f, 255 / 255f);
        switch (upgrade.upgradeType)
        {
            case UpgradeType.upgrade:
                imageC = new Color(142 / 255f, 142 / 255f, 142 / 255f, 255 / 255f);
                break;
            case UpgradeType.attack:
                imageC = new Color(200 / 255f, 85 / 255f, 85 / 255f, 255 / 255f);
                break;
            case UpgradeType.skill:
                imageC = new Color(85 / 255f, 85 / 255f, 200 / 255f, 255 / 255f);
                break;
        }
        btn.GetComponent<Image>().color = imageC;
        btn.transform.Find("Image").GetComponent<Image>().sprite = image;
        btn.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = upgrade.name;

        if (upgrade.upgradeType == UpgradeType.upgrade)
        {
            btn.transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
                $"[ 스탯 ]\n<size=5> </size>\n{upgrade.text}\n";
        }
        else
        {
            Skill obj = null;
            string typeS = "스킬";
            if (upgrade.upgradeType == UpgradeType.attack)
            {
                obj = SkillManager.Instance.attackPrefabs[(int)System.Enum.Parse(typeof(SkillManager.AttackType), upgrade.name)].GetComponent<Skill>();
                typeS = "기본 공격";
            }
            else
            {
                obj = SkillManager.Instance.skillPrefabs[(int)System.Enum.Parse(typeof(SkillManager.SkillType), upgrade.name)].GetComponent<Skill>();
                typeS = "스킬";
            }
            string text = string.Join(", ", upgrade.tags);
            btn.transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
                $"[ {typeS} ]\n<size=5> </size>\n{upgrade.text}\n<size=5> </size>\n<color=red> 데미지 : {obj.damage} </color> <color=yellow> 속도 : {obj.speed} </color> <color=blue> 사거리 : {obj.range} </color> <color=green> 쿨타임 : {obj.coolTime} </color>\n<color=white>{text}</color>";
        }
    }

    public void CheckBossTime()
    {
        if (warningUI.activeSelf)
            return;

        foreach (Enemy boss in PoolManager.Instance.bosses)
        {
            if (boss.dontSpawn)
                continue;
            else if (boss.startSpawnTime - GameManager.Instance.gameTimer <= 5)
            {
                warningUI.SetActive(true);
                warningUI.GetComponent<WarningUI>().bossTime = boss.startSpawnTime;
            }
        }
    }

    public void UpdateStatsUI()
    {
        string[] type = { "기본 데미지", "이동 속도", "기본 발사 속도", "탄환 크기", "기본 사거리" };
        float[] statV = { GameManager.Instance.playerDmg, GameManager.Instance.playerSpeed, GameManager.Instance.playerShotSpeed + 1, GameManager.Instance.playerBulletScale + 1, GameManager.Instance.playerBulletRange + 1 };
        TextMeshProUGUI[] txt = statsUI.GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < txt.Length; i++)
        {
            txt[i].text = $"{type[i]} : {statV[i]:#0}";
        }
    }

    //     public void ScreenInit()
    //     {
    //         GameObject Canvas = GameObject.Find("Canvas");
    //         Vector2 center = Canvas.GetComponent<Canvas>().GetComponent<RectTransform>().anchoredPosition;

    // #if UNITY_ANDROID
    //         fingerC = new Color(0, 0, 0, 0);
    //         if (Screen.orientation == ScreenOrientation.Portrait)
    //         {
    //             Screen.SetResolution(1200, 1920, true);
    //             Canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;

    //             RectTransform rtLvUP = LvUpUI.GetComponent<RectTransform>();
    //             rtLvUP.sizeDelta = new Vector2(1100, 950);
    //             rtLvUP.position = new Vector3(0, 150, 0);
    //             for (int i = 0; i < 3; i++)
    //             {
    //                 rtLvUP.GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(340, 700);
    //                 rtLvUP.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(rtLvUP.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.x, 350);
    //                 rtLvUP.GetChild(i).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(rtLvUP.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.x, 345);
    //             }
    //             cancelBtnRT.position = new Vector3(0, -70, 0);
    //         }
    //         else if (Screen.orientation == ScreenOrientation.LandscapeRight)
    //         {
    //             Screen.SetResolution(1920, 1200, true);
    //             Canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;

    //             RectTransform rtLvUP = LvUpUI.GetComponent<RectTransform>();
    //             rtLvUP.sizeDelta = new Vector2(1000, 600);
    //             rtLvUP.position = new Vector3(0, 0, 0);
    //             for (int i = 0; i < 3; i++)
    //             {
    //                 rtLvUP.GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(340, 500);
    //                 rtLvUP.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(rtLvUP.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.x, 160);
    //                 rtLvUP.GetChild(i).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(rtLvUP.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.x, 160);
    //             }
    //             cancelBtnRT.position = new Vector3(0, -70, 0);
    //         }
    // #else
    //         Screen.orientation(1200, 1920, true);
    //         fingerC = new Color(0, 0, 0, 1);
    // #endif
    //         // 1200 x 1920 --> center : x-600 y-960
    //         float minX = center.x - (Screen.width / 2f) - 0.5f;
    //         // float minX = center.x - 600;
    //         float maxX = center.x + (Screen.width / 2f) + 0.5f;
    //         // float maxX = center.x + 600;
    //         float minY = center.y - (Screen.height / 2f) - 0.5f;
    //         // float minY = center.y - 960;
    //         float maxY = center.y + (Screen.height / 2f) + 0.5f;
    //         // float maxY = center.y + 960;

    //         GameManager.Instance.Guide[0] = new Vector2(minX, minY);
    //         GameManager.Instance.Guide[1] = new Vector2(minX, maxY);
    //         GameManager.Instance.Guide[2] = new Vector2(maxX, maxY);
    //         GameManager.Instance.Guide[3] = new Vector2(maxX, minY);
    //     }

    void Update()
    {
        CheckBossTime();
        UpdateStatsUI();
        AddScore();
        lvUI.text = $"LV {lvSetting.curLv:##0}";
        expUI.text = $"EXP : {lvSetting.curEXP:##0} / {lvSetting.maxEXP}";
        scoreUI.text = $"Score : {GameManager.Instance.score:#00}";
        GameObject.Find("curExp").GetComponent<Image>().fillAmount = lvSetting.curEXP / lvSetting.maxEXP;

        // if (Screen.orientation != screenOrientation)
        // {
        //     ScreenInit();
        // }
    }
}
