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

// UI Canvasにそって回る
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

    public int level;
    private int index = 1;
    public float flTimer = 0f;
    public float flLvTime = 1f;
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
        //　親がキャンバス
        this.transform.localPosition = GameManager.Instance.Guide[0];
        // localPosition --> Guide : 親にキャンバスがいるから
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
        // ボスが直接弾幕を打つ場合が多いためオブジェクトとして弾幕を生成

        // 親との相対的な位置
        // Guide Sizeを使うことができる　<-- Guide Sizeはキャンバスを基準に作られたものだから
        this.transform.localPosition = Vector2.MoveTowards(this.transform.localPosition, GameManager.Instance.Guide[index], Time.deltaTime * 300f);

        // distance --> 各ベクトルの＾２をたした後にルート
        // float d1 = Vector2.SqrMagnitude(this.transform.position);
        // float d2 = Vector2.SqrMagnitude(Guide[index]);

        // Guideの端っこからキャンバスの辺に沿って移動するので
        // ｘかｙかのどっちかしか変わらない。
        // つまり、ベクトルの演算が必要なくなる。 
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

        // 既存の x(y)と 目的地 x(y)の差の絶対値が0.1より小さい場合、目的地に到達したとみなして
        // 次の目的地へ 0 -> 1 -> 2 -> 3 -> 0(４はないので３に移動)
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

        // LvTimeは難易度 (値が少なければ少ないほど敵が早くわく)
        // flTimerは time.deltatimeに影響されるので　LvTimeより大きくなると弾幕を生成
        // 0秒にリセット
        if (flLvTime < flTimer)
        {
            InitRanEnemyList();
            int enemyIndex = Random.Range(0, curRanEnemyList.Count);
            if (spawnEnemyList.Count > 100)
            {
                Debug.Log("100+!!!!");
                Destroy(spawnEnemyList[0]);
            }
            // 弾幕はキャンバスにはない。つまり positionで入れるべき
            GameObject go = Instantiate(curRanEnemyList[enemyIndex]);
            go.transform.position = this.transform.position;
            spawnEnemyList.Add(go);
            // go.transform.rotation = Quaternion.identity;
            // go.transform.parent = GameObject.Find("Canvas").transform;

            // 弾幕の色はＥｎｃｏｌｏｒからランダムに設定
            // go.GetComponent<Bullet>().Init((GameUtil.enColor)Random.Range(0, 3));

            flTimer = 0f;
            // if (flLvTime >= 0)
            //     flLvTime -= 0.03f;
            // else if (flLvTime <= 0)
            //     flLvTime = 3;



            level = Mathf.FloorToInt(GameManager.Instance.gameTimer / 10f);

            if (flTimer > (level == 0 ? 0.5f : 0.2f))
            {
                flTimer = 0;
            }
            else if (flLvTime < 0)
            {
                flLvTime = 3;
            }
            // レベルは徐々に上昇し、ーになると３に戻る

        }
        flTimer += Time.deltaTime;

        // Canvas --> UI
        // Sprite Renderer -- Canvasの外にオブジェクト設定
        // しかしキャンバスとUnity上のユニットは単位が違う（Canvasをカメラに合わせたから Canvasのスケイルは別）
    }
}
