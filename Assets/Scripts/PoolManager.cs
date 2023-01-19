using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyList
{
    Virus = 0,
    DarkVirus,
    BossVirus,
    Virus2,
    DarkVirus2,
    BossVirus2,
    BossVirus3,
    CountIndex
}

public enum EnemyRank
{
    normal = 0,
    epik,
    boss,
    CountIndex
}

// UI Canvas를 따라 돌음 
public class PoolManager : MonoBehaviour
{
    public enum enColor
    {
        Red = 0,
        Green,
        Blue
    }

    private static PoolManager instance;
    public static PoolManager Instance
    {
        get
        {
            if (instance == null)
            {
                // monobehaviour를 안 쓴다면 
                // instance = new GameManager();

                GameObject go = new GameObject();
                go.name = "PoolManager";
                instance = go.AddComponent<PoolManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    public List<GameObject> enemyList = new List<GameObject>();
    public List<GameObject> curRanEnemyList = new List<GameObject>();
    public List<Enemy> bosses = new List<Enemy>();

    [SerializeField]
    private GameObject enemy;

    private int index = 1;
    public float flTimer = 0f;
    public float flLvTime = 3f;
    public List<GameObject> spawnEnemyList = new List<GameObject>();

    private void Start()
    {
        for (EnemyList i = (EnemyList)0; i < EnemyList.CountIndex; i++)
        {
            enemy = Resources.Load($"Prefabs/{i.ToString()}") as GameObject;
            enemy.GetComponent<Enemy>().dontSpawn = false;
            enemyList.Add(enemy);
            if (enemy.GetComponent<Enemy>().rank == EnemyRank.boss)
            {
                bosses.Add(enemy.GetComponent<Enemy>());
            }
        }
        StartGame();
    }

    public void StartGame()
    {
        this.transform.parent = GameManager.Instance.Canvas.transform;
        // 부모로 캔버스 
        this.transform.localPosition = GameManager.Instance.Guide[0];
        // localPosition --> Guide : 부모로 캠버스가 있어서
    }

    public void InitRanEnemyList()
    {
        curRanEnemyList.Clear();
        foreach (GameObject enem in enemyList)
        {
            if (enem.GetComponent<Enemy>().startSpawnTime <= GameManager.Instance.gameTimer)
            {
                if (enem.GetComponent<Enemy>().dontSpawn)
                    continue;
                else if (enem.GetComponent<Enemy>().rank == EnemyRank.boss)
                {
                    curRanEnemyList.Clear();
                    curRanEnemyList.Add(enem);
                    // enem.GetComponent<Enemy>().spawnPercent = 0;
                    enem.GetComponent<Enemy>().dontSpawn = true;
                    return;
                }
                else if (enem.GetComponent<Enemy>().spawnPercent == 0)
                {
                    continue;
                }
                else
                {
                    for (int i = 0; i < enem.GetComponent<Enemy>().spawnPercent; i++)
                    {
                        curRanEnemyList.Add(enem);
                    }
                }
            }
        }
    }

    private void Update()
    {
        // 보스가 탄막을 직접 쏘는 경우가 많아서 오브젝트로 탄막 생성 
        // 캐릭터를 향해서 풀매니저 오브젝트가 보게 됨 

        // 부모와의 상대적 위치
        // Guide Size를 쓸 수 있음 <-- Guide Size는 캔버스를 기준으로 생성된 것이므로 
        this.transform.localPosition = Vector2.MoveTowards(this.transform.localPosition, GameManager.Instance.Guide[index], Time.deltaTime * 300f);

        // distance --> 루트 연산(무거움) : 각 벡터의 성분의 제곱끼리 더한 후에 루트
        // float d1 = Vector2.SqrMagnitude(this.transform.position);
        // float d2 = Vector2.SqrMagnitude(Guide[index]);

        // Guide의 꼭지점에서 캔버스 변을 따라 이동하므로 
        // Guide의 각 꼭지점으로 이동하는데 항상 x만 변하거나 y만 변하게 됨
        // 즉, 무거운 거리연산을 할 필요가 없음 
        // index
        // 0 x
        // 1 y
        // 2 x
        // 3 y

        // % 2 ==> 0 -> x
        // % 2 ==> 1 -> y

        float f = index % 2 == 0
                ? GameManager.Instance.Guide[index].x - this.transform.localPosition.x
                : GameManager.Instance.Guide[index].y - this.transform.localPosition.y;

        // 원래 x(y)와 목적지 x(y)의 차의 절댓값이 0.1보다 작으면 (목적지에 다 도착했다 치고)
        // 다음 목적지로 변경! 0 -> 1 -> 2 -> 3 -> 0(4는 없으므로 3으로 가도록 해야함)
        if (Mathf.Abs(f) < 0.1f)
        {
            if (index == 3)
            {
                index = 0;
            }
            else
            {
                index++;
            }
        }

        // LvTime은 난이도 (작아지면 작아질 수록 더 빨리 생성)
        // flTimer는 time.deltatime에 따라 계속 증가되어서 LvTime보다 커지면 총알 생성 
        // 0초로 초기화 
        if (flLvTime < flTimer)
        {
            InitRanEnemyList();
            int enemyIndex = Random.Range(0, curRanEnemyList.Count);
            if (spawnEnemyList.Count > 100)
            {
                Debug.Log("100+!!!!");
                Destroy(spawnEnemyList[0]);
            }
            // 총알은 캔버스 아래에 없으므로 position으로 넣어야함 
            GameObject go = Instantiate(curRanEnemyList[enemyIndex]);
            go.transform.position = this.transform.position;
            spawnEnemyList.Add(go);
            // go.transform.rotation = Quaternion.identity;
            // go.transform.parent = GameObject.Find("Canvas").transform;

            // 총알 색상을 enColor에서 랜덤으로 설정 
            // go.GetComponent<Bullet>().Init((GameUtil.enColor)Random.Range(0, 3));

            flTimer = 0f;
            if (flLvTime >= 0)
                flLvTime -= 0.03f;
            else if (flLvTime <= 0)
                flLvTime = 3;
        }
        flTimer += Time.deltaTime;

        // Canvas --> UI
        // Sprite Renderer -- Canvas 바깥에 오브젝트 위치 세팅
        // 캔버스랑 유니티 상의 유닛이랑 단위가 다름 (Canvas를 카메라에 맞추면서 Canvas의 스케일이 따로 들어가 있음)
    }
}
