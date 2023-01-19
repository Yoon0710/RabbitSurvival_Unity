using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum BulletType
{
    normal = 0,
    lazer,
    guided,
    missile,
    CountIndex
}

public class Bullet : MonoBehaviour
{
    [System.NonSerialized]
    public GameUtil.enColor mColor;
    public BulletType Btype = BulletType.normal;
    public float lifeTime = 5f;
    private Vector3 playerPos;
    public Vector3 playerDir;
    public float speed = 50f;
    public int damage = 1;
    public bool reverse = false;
    public float range = 2f;

    private Vector3 originPos;

    public void Init(GameUtil.enColor color)
    {
        mColor = color;
        this.GetComponent<SpriteRenderer>().color = GameUtil.GetColor(mColor);
        playerPos = GameManager.Instance.CharacterM.transform.position;
        playerDir = (playerPos - transform.position).normalized;
        originPos = this.transform.position;
    }

    private void Awake()
    {

    }

    private Vector2 FindPlayerPos(float range)
    {
        if (Vector2.Distance(transform.position, GameObject.Find("Character").transform.position) <= range)
        {
            return GameObject.Find("Character").transform.position;
        }

        return Vector2.zero;
    }

    private void ShootLazer()
    {
        StartCoroutine(LazerSpawn());
        float distance = range;

        transform.localScale = new Vector3(distance, transform.localScale.y, transform.localScale.z);
        // float moveX = EnemyPos.x - playerPos.x * 0.5f;
        // float moveY = EnemyPos.y - playerPos.y * 0.5f;
        // transform.position = playerPos + new Vector2(moveX, moveY);

        Vector3 qDir = Quaternion.Euler(0, 0, 0) * (playerPos - originPos).normalized;
        Quaternion targetDir = Quaternion.LookRotation(forward: Vector3.forward, upwards: qDir);
        this.transform.localRotation = Quaternion.Euler(0, 0, targetDir.eulerAngles.z + 90f);
    }

    IEnumerator LazerSpawn()
    {
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().color = Color.green - new Color(0, 0, 0, 0.6f);
        yield return new WaitForSeconds(1f);
        GetComponent<CapsuleCollider2D>().enabled = true;
        GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void ReverseDir()
    {
        if (playerDir != null && !reverse)
        {
            reverse = true;
            playerPos = playerPos * -1;
            playerDir = playerDir * -1;
            speed += 3;
            this.GetComponent<SpriteRenderer>().color = GameUtil.GetColor(GameUtil.enColor.Blue);
        }
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0f)
        {
            Destroy(gameObject);
        }

        // this.transform.position = Vector3.MoveTowards
        // (
        //     this.transform.position,
        //     playerPos,
        //     speed * Time.deltaTime
        // );
        // this.transform.position = this.transform.position + moveDir * speed * Time.deltaTime;

        if (Btype == BulletType.guided)
        {
            Vector2 nPos = FindPlayerPos(range);
            if (nPos != Vector2.zero)
            {
                playerPos = nPos;
                playerDir = nPos.normalized;
            }
        }

        if (Btype != BulletType.lazer)
        {
            if (Vector2.Distance(playerPos, this.transform.localPosition) < 0.1f)
            {
                playerPos += playerDir * 2;
            }

            if (playerDir != null)
                this.transform.localPosition = Vector2.MoveTowards(this.transform.position, playerPos, Time.deltaTime * speed * 3f);
        }
        else
            ShootLazer();
    }
}
