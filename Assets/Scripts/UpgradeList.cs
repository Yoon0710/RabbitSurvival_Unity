using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public enum UpgradeType
{
    upgrade = 0,
    attack,
    skill,
    CountIndex
}

public enum UpgradeStates
{
    damage = 0,
    speed,
    shotSpeed,
    bulletScale,
    bulletRange,
    coolTime,
    maxHP,
    coldShot,
    fireShot,
    sparkShot,
    CountIndex
}

[System.Serializable]
public struct Upgrade
{
    [SerializeField]
    public UpgradeType upgradeType;

    [SerializeField]
    public string name;

    public UpgradeStates upgradeStates;
    public float value;

    [TextArea]
    public string text;
    public string[] tags;

    public void OnBtn()
    {
        switch (upgradeType)
        {
            case UpgradeType.upgrade:
                UpgradeStat(upgradeStates, value);
                break;
            case UpgradeType.attack:
                ChangeAttack(name);
                break;
            case UpgradeType.skill:
                ChangeSkill(name);
                break;
            default:
                break;
        }
    }

    public void UpgradeStat(UpgradeStates stat, float value)
    {
        switch (stat)
        {
            case UpgradeStates.damage:
                GameManager.Instance.playerDmg += (int)value;
                break;
            case UpgradeStates.speed:
                GameManager.Instance.playerSpeed += value;
                break;
            case UpgradeStates.shotSpeed:
                GameManager.Instance.playerShotSpeed += value;
                break;
            case UpgradeStates.bulletScale:
                GameManager.Instance.playerBulletScale += value;
                break;
            case UpgradeStates.bulletRange:
                GameManager.Instance.playerBulletRange += value;
                break;
            case UpgradeStates.coolTime:
                GameManager.Instance.playerCoolTime += value;
                break;
            case UpgradeStates.coldShot:
                GameManager.Instance.playerColdShotLv += value;
                GameManager.Instance.playerPushPower -= 0.5f;
                if (GameManager.Instance.playerPushPower < 0)
                    GameManager.Instance.playerPushPower = 0;
                // GameManager.Instance.playerFireShotLv = 0;
                // GameManager.Instance.playerSparkShotLv = 0;
                break;
            case UpgradeStates.fireShot:
                GameManager.Instance.playerFireShotLv += value;
                GameManager.Instance.playerPushPower -= 0.5f;
                if (GameManager.Instance.playerPushPower < 0)
                    GameManager.Instance.playerPushPower = 0;
                // GameManager.Instance.playerColdShotLv = 0;
                // GameManager.Instance.playerSparkShotLv = 0;
                break;
            case UpgradeStates.sparkShot:
                GameManager.Instance.playerSparkShotLv += value;
                // GameManager.Instance.playerFireShotLv = 0;
                // GameManager.Instance.playerColdShotLv = 0;
                break;
            // case UpgradeStates.curHP:
            //     if (GameManager.Instance.playerHP + (int)value > GameManager.Instance.playerMaxHP)
            //         UIManager.Instance.HPUP(GameManager.Instance.playerMaxHP - GameManager.Instance.playerHP);
            //     UIManager.Instance.HPUP((int)value);
            //     break;
            case UpgradeStates.maxHP:
                UIManager.Instance.MaxHPUP((int)value);
                if (GameManager.Instance.playerHP + 1 > GameManager.Instance.playerMaxHP)
                    UIManager.Instance.HPUP(GameManager.Instance.playerMaxHP - GameManager.Instance.playerHP);
                UIManager.Instance.HPUP(1);
                break;
            default:
                break;
        }
    }

    public void ChangeAttack(string name)
    {
        if (!System.Enum.IsDefined(typeof(SkillManager.AttackType), name))
            return;

        SkillManager.AttackType atkType = (SkillManager.AttackType)System.Enum.Parse(typeof(SkillManager.AttackType), name);

        SkillManager.Instance.ChangeAtk((int)atkType);
    }

    public void ChangeSkill(string name)
    {
        if (!System.Enum.IsDefined(typeof(SkillManager.SkillType), name))
            return;

        SkillManager.SkillType sklType = (SkillManager.SkillType)System.Enum.Parse(typeof(SkillManager.SkillType), name);

        SkillManager.Instance.ChangeSkill((int)sklType);
    }
}


public class UpgradeList : MonoBehaviour
{
    public List<Button> btns = new List<Button>();

    int[] selected = new int[3];

    [SerializeField]
    Upgrade[] upgradeList;

    private GameObject finger;
    private int selectedIdx = 0;

    private bool stopFinger = false;

    // public struct DamageUP
    // {
    //     public DamageUP(int dmg)
    //     {

    //     }

    //     // public UpgradeType Utype = UpgradeType.upgrade;
    //     public override
    // }

    void Awake()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            btns.Add(buttons[i]);
            buttons[i].onClick.AddListener(GameManager.Instance.StartTime);
        }
        btns[0].onClick.AddListener(OnBtn01);
        btns[1].onClick.AddListener(OnBtn02);
        btns[2].onClick.AddListener(OnBtn03);
        finger = transform.Find("Finger").gameObject;

        // DamageUP DamageUP = new DamageUP();
        // upgrades.Add(DamageUP);
        // SpeedUP SpeedUP = new SpeedUP();
        // upgrades.Add(SpeedUP);
        // Lazer Lazer = new Lazer();
        // upgrades.Add(Lazer);
    }

    private void OnEnable()
    {
        selectedIdx = -2;
        int[] idx = new int[3] { -1, -1, -1 };
        for (int i = 0; i < 3; i++)
        {
            int n = -1;
            while (idx.Contains(n))
            {
                n = Random.Range(0, upgradeList.Length);
                if (upgradeList[n].upgradeType == UpgradeType.attack)
                {
                    // Debug.Log(SkillManager.Instance.attackType.ToString());
                    // Debug.Log(upgradeList[n].name);
                    if (SkillManager.Instance.attackType.ToString() == upgradeList[n].name)
                    {
                        n = -1;
                    }
                    else
                    {
                        int skillN = (int)System.Enum.Parse(typeof(SkillManager.AttackType), upgradeList[n].name);
                        UIManager.Instance.MakedUpgradeBtn(btns[i].gameObject, upgradeList[n], skillN, SkillManager.Instance.attackPrefabs[skillN].GetComponent<SpriteRenderer>().sprite);
                    }
                }
                else if (upgradeList[n].upgradeType == UpgradeType.skill)
                {
                    // Debug.Log(SkillManager.Instance.skillType.ToString());
                    // Debug.Log(upgradeList[n].name);
                    if (SkillManager.Instance.skillType.ToString() == upgradeList[n].name)
                    {
                        n = -1;
                    }
                    else
                    {
                        int skillN = (int)System.Enum.Parse(typeof(SkillManager.SkillType), upgradeList[n].name);
                        UIManager.Instance.MakedUpgradeBtn(btns[i].gameObject, upgradeList[n], skillN, SkillManager.Instance.skillPrefabs[skillN].GetComponent<SpriteRenderer>().sprite);
                    }
                }
                // else if (upgradeList[n].upgradeType == UpgradeType.upgrade && upgradeList[n].upgradeStates == UpgradeStates.curHP)
                // {
                //     if (GameManager.Instance.playerHP >= GameManager.Instance.playerMaxHP)
                //     {
                //         n = -1;
                //     }
                //     else
                //     {
                //         UIManager.Instance.MakedUpgradeBtn(btns[i].gameObject, upgradeList[n]);
                //     }
                // }
                else if (upgradeList[n].upgradeType == UpgradeType.upgrade && upgradeList[n].upgradeStates == UpgradeStates.maxHP)
                {
                    if (GameManager.Instance.playerMaxHP >= GameManager.Instance.fullMaxHP)
                    {
                        n = -1;
                    }
                    else
                    {
                        UIManager.Instance.MakedUpgradeBtn(btns[i].gameObject, upgradeList[n]);
                    }
                }
                else if (upgradeList[n].upgradeType == UpgradeType.upgrade && upgradeList[n].upgradeStates == UpgradeStates.damage)
                {
                    if (GameManager.Instance.playerDmg >= GameManager.Instance.fullPlayerDmg)
                    {
                        n = -1;
                    }
                    else
                    {
                        UIManager.Instance.MakedUpgradeBtn(btns[i].gameObject, upgradeList[n]);
                    }
                }
                else if (upgradeList[n].upgradeType == UpgradeType.upgrade && upgradeList[n].upgradeStates == UpgradeStates.speed)
                {
                    if (GameManager.Instance.playerSpeed >= GameManager.Instance.fullPlayerSpeed)
                    {
                        n = -1;
                    }
                    else
                    {
                        UIManager.Instance.MakedUpgradeBtn(btns[i].gameObject, upgradeList[n]);
                    }
                }
                else if (upgradeList[n].upgradeType == UpgradeType.upgrade && upgradeList[n].upgradeStates == UpgradeStates.coldShot)
                {
                    if (GameManager.Instance.playerColdShotLv == 1)
                    {
                        n = -1;
                    }
                    else
                    {
                        UIManager.Instance.MakedUpgradeBtn(btns[i].gameObject, upgradeList[n]);
                    }
                }
                else if (upgradeList[n].upgradeType == UpgradeType.upgrade && upgradeList[n].upgradeStates == UpgradeStates.fireShot)
                {
                    if (GameManager.Instance.playerFireShotLv == 1)
                    {
                        n = -1;
                    }
                    else
                    {
                        UIManager.Instance.MakedUpgradeBtn(btns[i].gameObject, upgradeList[n]);
                    }
                }
                else if (upgradeList[n].upgradeType == UpgradeType.upgrade && upgradeList[n].upgradeStates == UpgradeStates.sparkShot)
                {
                    if (GameManager.Instance.playerSparkShotLv == 1)
                    {
                        n = -1;
                    }
                    else
                    {
                        UIManager.Instance.MakedUpgradeBtn(btns[i].gameObject, upgradeList[n]);
                    }
                }
                else
                {
                    UIManager.Instance.MakedUpgradeBtn(btns[i].gameObject, upgradeList[n]);
                }
            }
            idx[i] = n;
        }
        selected = idx;
    }

    IEnumerator WaitForTime()
    {
        stopFinger = true;
        yield return new WaitForSecondsRealtime(0.2f);
        stopFinger = false;
    }

    public void moveFinger()
    {
        if (stopFinger)
            return;

        Vector2 dir = GameManager.Instance.JoystickM.Coordinate().normalized;
        if (selectedIdx == -2 && Input.anyKeyDown || selectedIdx == -2 && dir.x != 0)
        {
            selectedIdx = 0;
            StartCoroutine(WaitForTime());
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || dir.x > 0.4f)
        {
            if (selectedIdx == 2 || selectedIdx == -1)
                return;

            selectedIdx++;
            StartCoroutine(WaitForTime());
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || dir.x < -0.4f)
        {
            if (selectedIdx <= 0)
                return;

            selectedIdx--;
            StartCoroutine(WaitForTime());
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || dir.y < -0.4f)
        {
            selectedIdx = -1;
            StartCoroutine(WaitForTime());
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && selectedIdx == -1 || dir.y > 0.4f && selectedIdx == -1)
        {
            selectedIdx = 1;
            StartCoroutine(WaitForTime());
        }

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || SkillManager.Instance.atkBtn.GetComponent<Skillstick>().isClicked)
        {
            if (selectedIdx == -1)
                GameObject.Find("NoChoose").GetComponent<Button>().onClick?.Invoke();
            else
                btns[selectedIdx].onClick?.Invoke();
        }
    }

    public void OnBtn01()
    {
        upgradeList[selected[0]].OnBtn();
    }

    public void OnBtn02()
    {
        upgradeList[selected[1]].OnBtn();
    }

    public void OnBtn03()
    {
        upgradeList[selected[2]].OnBtn();
    }

    void Update()
    {
        moveFinger();
        // finger.GetComponent<RectTransform>().position = new Vector2(-470 + 340 * selectedIdx, finger.GetComponent<RectTransform>().position.y);
        if (selectedIdx == -1)
        {
            finger.GetComponent<RectTransform>().localPosition = new Vector3(-80, -530, 0);
            return;
        }
        finger.GetComponent<RectTransform>().localPosition = new Vector3(-470 + 340 * selectedIdx, -250, 0);
    }
}