using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;


public class SkillManager : MonoBehaviour
{
    [SerializeField]
    private string NormalShot_Sound;


    private static SkillManager instance;
    public static SkillManager Instance


    {
        get
        {
            if (instance == null)
            {
                // monobehaviour를 안 쓴다면 
                // instance = new GameManager();

                GameObject go = new GameObject();
                go.name = "SkillManager";
                instance = go.AddComponent<SkillManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }


    public enum AttackType
    {
        normalShot = 0,
        lazer,
        sword,
        indexCount
    }

    public enum SkillType
    {
        guidedMissile = 0,
        chargedShot,
        indexCount
    }

    
    public AttackType attackType = AttackType.lazer;
    public SkillType skillType = SkillType.guidedMissile;
    public List<GameObject> attackPrefabs = new List<GameObject>();
    public List<GameObject> skillPrefabs = new List<GameObject>();
    public AudioClip clip;
    public Vector2 MoveDir = Vector2.up;
    public float atkCool = 1f;
    private float curAtkCool = 0f;
    public float skillCool = 1f;
    private float curSkillCool = 0f;
    public Button atkBtn;
    Button skillBtn;

    public GameObject iceParticle;
    public GameObject fireParticle;
    public GameObject sparkParticle;
    public Vector2 enemyPos;

    public float iceCool = 0;
    public float fireCool = 0;
    public float sparkCool = 0;
    public int sparkN = 3;
    


    void Awake()
    {
        Init();
    }

    public void Init()
    {
        atkBtn = GameObject.Find("ButtonAtk").GetComponent<Button>();
        skillBtn = GameObject.Find("ButtonSkill").GetComponent<Button>();

        // atkBtn.onClick.AddListener(ShotAttack);
        // skillBtn.onClick.AddListener(ShotSkill);

        for (AttackType i = (AttackType)0; i < AttackType.indexCount; i++)
        {
            GameObject go = Resources.Load($"Prefabs/{i.ToString()}") as GameObject;
            go.GetComponent<Skill>().skillLevel = 1;
            attackPrefabs.Add(go);
        }

        for (SkillType i = (SkillType)0; i < SkillType.indexCount; i++)
        {
            GameObject go = Resources.Load($"Prefabs/{i.ToString()}") as GameObject;
            go.GetComponent<Skill>().skillLevel = 1;
            skillPrefabs.Add(go);
        }

        ChangeAtk((int)AttackType.normalShot);
        ChangeSkill((int)SkillType.guidedMissile);

        iceParticle = Resources.Load("Particles/iceParticle") as GameObject;
        fireParticle = Resources.Load("Particles/fireParticle") as GameObject;
        sparkParticle = Resources.Load("Particles/sparkParticle") as GameObject;

        iceCool = 0;
        fireCool = 0;
        sparkCool = 0;
    }

    public void ChangeAtk(int idx)
    {
        attackType = (AttackType)idx;
        atkCool = attackPrefabs[idx].GetComponent<Skill>().coolTime;
        UIManager.Instance.ChangeAtkButtonImage(attackPrefabs[idx]);
    }

    public void ChangeSkill(int idx)
    {
        skillType = (SkillType)idx;
        skillCool = skillPrefabs[idx].GetComponent<Skill>().coolTime;
        UIManager.Instance.ChangeSklButtonImage(skillPrefabs[idx]);
    }

    public void ShotAttack()
    {       
        if (curAtkCool != 0)
            return;
       
        
        Skill atk = Instantiate(attackPrefabs[(int)attackType]).GetComponent<Skill>();
        atk.transform.position = GameManager.Instance.CharacterM.transform.position;

        // atk.SetDir(MoveDir);

        // #if UNITY_EDITOR
        //         Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //         Vector2 target = GameManager.Instance.CharacterM.transform.position;
        //         // float angle = Mathf.Atan2(mousePos.y - target.y, mousePos.x - target.x) * Mathf.Rad2Deg;
        //         Vector2 dir = (mousePos - target).normalized;
        //         atk.SetDir(dir);
        //         curAtkCool = atkCool;
        atk.SetDir(MoveDir);
        curAtkCool = atkCool;


        // Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Vector2 target = GameManager.Instance.CharacterM.transform.position;
        // // float angle = Mathf.Atan2(mousePos.y - target.y, mousePos.x - target.x) * Mathf.Rad2Deg;
        // Vector2 dir = (mousePos - target).normalized;
        // atk.SetDir(dir);
        // curAtkCool = atkCool;
        SoundManager.instance.PlaySE(NormalShot_Sound);
    }

    public void ShotSkill()
    {
        if (curSkillCool != 0)
            return;

        if (skillType == SkillType.guidedMissile)
        {
            StartCoroutine(ShootGuided());
        }
        else
            CreateSkillBullet();

        curSkillCool = skillCool;
    }

    public void CreateSkillBullet()
    {
        Skill skill = Instantiate(skillPrefabs[(int)skillType]).GetComponent<Skill>();
        skill.transform.position = GameManager.Instance.CharacterM.transform.position;
        // skill.SetDir(MoveDir);
#if UNITY_ANDROID
        skill.SetDir(MoveDir);

#else
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 target = GameManager.Instance.CharacterM.transform.position;
        // float angle = Mathf.Atan2(mousePos.y - target.y, mousePos.x - target.x) * Mathf.Rad2Deg;
        Vector2 dir = (mousePos - target).normalized;
        skill.SetDir(dir);

        
#endif
    }

    IEnumerator ShootGuided()
    {
        CreateSkillBullet();
        yield return new WaitForSeconds(0.2f);
        CreateSkillBullet();
        yield return new WaitForSeconds(0.2f);
        CreateSkillBullet();
        yield return new WaitForSeconds(0.2f);
        CreateSkillBullet();
        yield return new WaitForSeconds(0.2f);
        CreateSkillBullet();
    }

    public void StartParticle(GameObject particle)
    {
        StartCoroutine("SpawnParticle", particle);
    }

    public IEnumerator SpawnParticle(GameObject particle)
    {
        GameObject pt = Instantiate(particle);
        pt.transform.position = enemyPos;
        yield return new WaitForSeconds(2f);
        Destroy(pt);
    }

    void Update()
    {
        if (curAtkCool > 0)
        {
            curAtkCool -= Time.deltaTime;
        }
        else
            curAtkCool = 0;

        if (curSkillCool > 0)
        {
            curSkillCool -= Time.deltaTime;
        }
        else
            curSkillCool = 0;

        if (GameManager.Instance.playerColdShotLv > 0 && iceCool > 0)
        {
            iceCool -= Time.deltaTime;
        }
        else
            iceCool = 0;

        if (GameManager.Instance.playerFireShotLv > 0 && fireCool > 0)
        {
            fireCool -= Time.deltaTime;
        }
        else
            fireCool = 0;

        if (GameManager.Instance.playerSparkShotLv > 0 && sparkCool > 0)
        {
            sparkCool -= Time.deltaTime;
        }
        else
            sparkCool = 0;

        if (atkBtn == null || skillBtn == null)
            return;

        atkBtn.transform.Find("Timer").GetComponent<Image>().fillAmount = curAtkCool / atkCool;
        skillBtn.transform.Find("Timer").GetComponent<Image>().fillAmount = curSkillCool / skillCool;
    }
}
