using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private GameObject goBullet;

    // private int index = 1;
    public EnemyList type = (EnemyList)0;
    public EnemyRank rank = (EnemyRank)0;
    public float flTimer = 0f;
    public float flLvTime = 5f;
    public float maxHP = 10f;
    public float curHP;
    public float moveSpeed = 10f;
    public float shotSpeed = 5f;
    public int damage = 1;
    public float pushed = 1f;
    public float bulletLifeTime = 5f;

    public float startSpawnTime = 0f;
    public float spawnPercent = 5;

    [System.NonSerialized]
    public bool isSpawn = false;
    public bool isDamaged = false;
    public bool isRzor = false;

    private GameObject player;
    [System.NonSerialized]
    public bool dontSpawn = false;

    public List<Bullet> bullets = new List<Bullet>();
    public List<Bullet> bulletsActive = new List<Bullet>();

    private bool isIced = false;
    private bool isFired = false;
    private bool isSparked = false;

    private void Start()
    {
        goBullet = Resources.Load("Prefabs/Bullet") as GameObject;
        curHP = maxHP;
        Invoke("SpawnEnemy", 0.5f);
        player = GameObject.Find("Character");
    }

    public void SpawnEnemy()
    {
        Color originC = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(originC.r, originC.g, originC.b, 255);
        isSpawn = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isSpawn && !isDamaged)
            return;

        if (other.gameObject.tag == "Bullet" && other.GetComponent<SpriteRenderer>().color == new Color(160f / 255f, 160f / 255f, 255f / 255f, 255f / 255f))
        {
            GetComponent<Rigidbody2D>().AddForce(other.GetComponent<Bullet>().playerDir * 2f * pushed, ForceMode2D.Impulse);
            curHP -= 2f;
            StartDamaged();
        }
    }

    public void IceDamaged()
    {
        if (isIced)
            return;

        StartCoroutine(IceDmged());
    }

    IEnumerator IceDmged()
    {
        isIced = true;
        moveSpeed *= 0.5f;
        yield return new WaitForSeconds(1.3f);
        isIced = false;
        moveSpeed *= 2f;
    }

    public void FireDamaged()
    {
        if (isFired)
            return;

        StartCoroutine(FireDmged());
    }

    IEnumerator FireDmged()
    {
        isFired = true;
        yield return new WaitForSeconds(0.3f);
        curHP -= 0.5f;
        StartDamaged();
        yield return new WaitForSeconds(0.3f);
        curHP -= 0.5f;
        StartDamaged();
        yield return new WaitForSeconds(0.3f);
        curHP -= 0.5f;
        StartDamaged();
        yield return new WaitForSeconds(0.3f);
        curHP -= 0.5f;
        StartDamaged();
        isFired = false;
    }

    public Transform SparkDamaged()
    {
        if (isSparked)
            return null;

        StartCoroutine(SparkDmged());

        var objects = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        objects.RemoveAll(t => t.GetComponent<Enemy>().isSparked);
        objects.Remove(this.gameObject);

        if (objects == null)
        {
            return null;
        }

        // LINQ 메소드를 이용해 가장 가까운 적을 찾습니다.
        var neareastObject = objects
            .OrderBy(obj =>
        {
            return Vector2.Distance(this.transform.position, obj.transform.position);
        })
        .FirstOrDefault();

        if (neareastObject == null)
        {
            return null;
        }

        return neareastObject.transform;
    }

    IEnumerator SparkDmged()
    {
        isSparked = true;
        curHP -= 1f;
        StartDamaged();
        yield return new WaitForSeconds(0.1f);
        isSparked = false;
    }

    public void StartDamaged()
    {
        isDamaged = true;
        StartCoroutine(Damaged());
    }

    public IEnumerator Damaged()
    {
        SpriteRenderer sp = GetComponent<SpriteRenderer>();
        Color origin = sp.color;
        Color damagedC = new Color(255, 0, 0, 50);

        sp.color = damagedC;
        yield return new WaitForSeconds(0.02f);
        sp.color = origin;
        yield return new WaitForSeconds(0.02f);
        sp.color = damagedC;
        yield return new WaitForSeconds(0.02f);
        sp.color = origin;
        isDamaged = false;
        isRzor = false;
        yield return new WaitForSeconds(0.2f);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    private void Update()
    {
        if (!isSpawn || (isDamaged && !isRzor))
            return;

        if (curHP <= 0)
        {
            int maxCount = Random.Range(1, 5);
            if (rank == EnemyRank.epik)
            {
                maxCount = Random.Range(4, 10);
            }
            else if (rank == EnemyRank.boss)
            {
                maxCount = Random.Range(7, 15);
            }
            for (int i = 0; i < maxCount; i++)
            {
                GameObject go = Instantiate(goBullet);
                float posX = i > 5 ? (i > 10 ? i - 10 : i - 5) : i;
                float posY = i > 5 ? (i > 10 ? -1f : -0.5f) : 0;
                go.transform.position = this.transform.position + new Vector3(posX * 0.2f, posY, 0);
                Bullet bullet = go.GetComponent<Bullet>();
                bullet.damage = 0;
                bullet.speed = 0;
                bullet.lifeTime = 30f;
                bullet.Init((GameUtil.enColor)0);
            }

            int heartPercent = Random.Range(0, 10);
            if (rank == EnemyRank.epik)
            {
                heartPercent = Random.Range(4, 10);
            }
            else if (rank == EnemyRank.boss)
            {
                heartPercent = Random.Range(6, 13);
            }

            if (heartPercent >= 6)
            {
                int heartCount = (int)heartPercent / 6;

                for (int i = 0; i < heartCount; i++)
                {
                    GameObject heart = Instantiate(Resources.Load("Prefabs/Heart") as GameObject);
                    heart.name = "Heart";
                    heart.transform.position = this.transform.position + new Vector3(i * 0.5f, 0.5f, 0);
                    Bullet heartB = heart.GetComponent<Bullet>();
                    heartB.damage = 0;
                    heartB.speed = 0;
                    heartB.lifeTime = 20f;
                    heartB.Init(GameUtil.enColor.White);
                }
            }

            Destroy(this.gameObject);
        }

        // this.transform.localPosition = Vector2.MoveTowards(this.transform.localPosition, GameObject.Find("Character").transform.position, Time.deltaTime * moveSpeed);
        this.transform.position = Vector2.MoveTowards
        (
            this.transform.position,
            player.transform.position,
            moveSpeed * Time.deltaTime
        );

        // Debug.Log(player.transform.localPosition);
        if (flLvTime < flTimer)
        {
            if (bullets.Count == 0)
            {
                foreach (Bullet b in bulletsActive)
                {
                    bullets.Add(b);
                }

                bulletsActive.Clear();
            }

            int RndNum = Random.Range(0, bullets.Count - 1);
            // 총알은 캔버스 아래에 없으므로 position으로 넣어야함 

            GameObject go = Instantiate(bullets[RndNum].gameObject);
            go.transform.position = this.transform.position;
            // go.transform.rotation = Quaternion.identity;
            // go.transform.parent = GameObject.Find("Canvas").transform;

            // 총알 색상을 enColor에서 랜덤으로 설정 
            Bullet bullet = go.GetComponent<Bullet>();
            bullet.damage = damage;
            bullet.speed = shotSpeed;
            bullet.lifeTime = bulletLifeTime;
            bullet.Init((GameUtil.enColor)1);

            bulletsActive.Add(bullets[RndNum]);
            bullets.Remove(bullets[RndNum]);
            flTimer = 0f;
        }
        flTimer += Time.deltaTime;
    }
}
