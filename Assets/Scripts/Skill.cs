using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ShotType
{
    bullet = 0,
    charged,
    lazer,
    guided,
    trap,
    missile,
    sword,
    CountIndex
}

public class Skill : MonoBehaviour
{
    public ShotType shotType;
    public float damage = 1f;
    public float damageStep = 0.3f;
    public float range = 5f;
    public float coolTime = 3f;
    public float coolTimeStep = 0.3f;
    public float speed = 5f;
    public float speedStep = 0.3f;
    public float razerRange = 3f;
    public float rangeStep = 0.3f;
    public float push = 1f;
    public float pushStep = 0.2f;

    private float oriDmg;
    private float oriSpd;
    private float oriClT;
    private float oriPush;

    [System.NonSerialized]
    public Vector2 shootDir;

    private Vector2 inputDir;

    private GameObject guidedObj;
    private float rotationZ = -45f;
    private Quaternion oriEuler;
    public int skillLevel = 1;

    // [System.NonSerialized]
    // public float lazerDmgCool;

    void Awake()
    {
        float v = GameManager.Instance.playerBulletScale;
        transform.localScale += new Vector3(v, v, 0);
        oriDmg = damage;
        oriSpd = speed;
        oriClT = coolTime;
        oriPush = push;
        UpgradeSkill();

        if (shotType != ShotType.lazer)
            range += GameManager.Instance.playerBulletRange * 0.4f * rangeStep;
        else
            razerRange += GameManager.Instance.playerBulletRange * rangeStep;

        if (shotType == ShotType.sword)
            SetSwordRange();
    }

    public void UpgradeSkill()
    {
        damage = oriDmg + GameManager.Instance.playerDmg * damageStep;
        speed = oriSpd + GameManager.Instance.playerShotSpeed * speedStep;
        coolTime = oriClT - GameManager.Instance.playerCoolTime * coolTimeStep;
        push = oriPush + GameManager.Instance.playerPushPower * pushStep;
    }

    private GameObject FindNearestObjectByTag(Vector2 plyPos, string tag)
    {
        var objects = GameObject.FindGameObjectsWithTag(tag).ToList();
        if (objects == null)
            return null;

        // LINQ 메소드를 이용해 가장 가까운 적을 찾습니다.
        var neareastObject = objects
            .OrderBy(obj =>
        {
            return Vector2.Distance(plyPos, obj.transform.position);
        })
        .FirstOrDefault();

        return neareastObject;
    }

    public void SetLazor()
    {
        Vector2 playerPos = GameManager.Instance.CharacterM.transform.position;
        if (FindNearestObjectByTag(playerPos, "Enemy") == null)
        {
            shootDir = inputDir;
            Vector3 newDir = Quaternion.Euler(0, 0, 0) * inputDir;
            Quaternion tarDir = Quaternion.LookRotation(forward: Vector3.forward, upwards: newDir);
            this.transform.localRotation = Quaternion.Euler(0, 0, tarDir.eulerAngles.z + 90f);
            oriEuler = this.transform.localRotation;
            return;
        }
        Vector2 EnemyPos = FindNearestObjectByTag(playerPos, "Enemy").transform.position;

        float distance = Vector2.Distance(playerPos, EnemyPos);
        if (distance >= razerRange)
            distance = razerRange;
        else
            distance += 0.35f;

        transform.localScale = new Vector3(distance, transform.localScale.y, transform.localScale.z);
        // float moveX = EnemyPos.x - playerPos.x * 0.5f;
        // float moveY = EnemyPos.y - playerPos.y * 0.5f;
        // transform.position = playerPos + new Vector2(moveX, moveY);

        shootDir = (EnemyPos - playerPos).normalized;
        Vector3 qDir = Quaternion.Euler(0, 0, 0) * (EnemyPos - playerPos).normalized;
        Quaternion targetDir = Quaternion.LookRotation(forward: Vector3.forward, upwards: qDir);
        this.transform.localRotation = Quaternion.Euler(0, 0, targetDir.eulerAngles.z + 90f);
    }

    public void SetGuided()
    {
        Vector2 playerPos = GameManager.Instance.CharacterM.transform.position;
        if (FindNearestObjectByTag(playerPos, "Enemy") == null)
        {
            Destroy(this.gameObject);
            return;
        }
        guidedObj = FindNearestObjectByTag(playerPos, "Enemy").gameObject;
    }

    public void SetDir(Vector2 dir)
    {
        if (shotType == ShotType.lazer)
        {
            SetLazor();
            inputDir = dir;
            return;
        }
        shootDir = dir;
        Vector3 qDir = Quaternion.Euler(0, 0, 0) * dir;
        Quaternion targetDir = Quaternion.LookRotation(forward: Vector3.forward, upwards: qDir);
        this.transform.localRotation = Quaternion.Euler(0, 0, targetDir.eulerAngles.z + 90f);
        oriEuler = this.transform.localRotation;
    }

    public void SetSwordRange()
    {
        transform.localScale = new Vector3(range, range * 0.45f, 1);
    }

    public void Shoot(Vector2 dir)
    {
        // if (dir != null)
        //     this.transform.Translate(dir * Time.deltaTime * speed);
        // this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0f);
        // Vector2 joystickPos = GameManager.Instance.JoystickM.Coordinate();
        Vector2 joystickPos = dir;
        Vector3 newPos = new Vector3(transform.localPosition.x + joystickPos.x * speed * Time.deltaTime, transform.localPosition.y + joystickPos.y * speed * Time.deltaTime, transform.localPosition.z);
        transform.localPosition = newPos;
    }

    public void Sword()
    {
        transform.position = GameManager.Instance.CharacterM.transform.position + new Vector3(0, 0, 10);
        if (rotationZ >= 90f)
            Destroy(this.gameObject);
        rotationZ += speed * 100f * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, oriEuler.eulerAngles.z + rotationZ);
    }

    public void ShootLazer()
    {
        SetLazor();
    }

    public void GuidedShoot()
    {
        if (guidedObj == null)
        {
            SetGuided();
            return;
        }

        this.transform.position = Vector2.MoveTowards
        (
            this.transform.position,
            guidedObj.transform.position,
            speed * Time.deltaTime
        );
        shootDir = (guidedObj.transform.position - transform.position).normalized;
        SetDir(shootDir);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (shotType != ShotType.charged && shotType != ShotType.sword)
                Destroy(this.gameObject);
            Enemy targetE = other.GetComponent<Enemy>();
            if (targetE.isSpawn && !targetE.isDamaged)
            {
                if (shotType != ShotType.lazer)
                    other.GetComponent<Rigidbody2D>().AddForce(shootDir * push * other.GetComponent<Enemy>().pushed, ForceMode2D.Impulse);
                targetE.curHP -= damage;
                if (GameManager.Instance.playerColdShotLv >= 1 && SkillManager.Instance.iceCool <= 0)
                {
                    SkillManager.Instance.enemyPos = other.transform.position;
                    SkillManager.Instance.StartParticle(SkillManager.Instance.iceParticle);
                    SkillManager.Instance.iceCool = 1f;
                }
                if (GameManager.Instance.playerFireShotLv >= 1 && SkillManager.Instance.fireCool <= 0)
                {
                    SkillManager.Instance.enemyPos = other.transform.position;
                    SkillManager.Instance.StartParticle(SkillManager.Instance.fireParticle);
                    SkillManager.Instance.fireCool = 3f;
                }
                if (GameManager.Instance.playerSparkShotLv >= 1 && SkillManager.Instance.sparkCool <= 0)
                {
                    SkillManager.Instance.enemyPos = other.transform.position;
                    SkillManager.Instance.StartParticle(SkillManager.Instance.sparkParticle);
                    SkillManager.Instance.sparkCool = 2f;
                    SkillManager.Instance.sparkN = 3;
                }
                targetE.StartDamaged();
                if (shotType == ShotType.lazer)
                {
                    targetE.isRzor = true;
                }
            }
            else if (targetE.isRzor && shotType != ShotType.lazer)
            {
                other.GetComponent<Rigidbody2D>().AddForce(shootDir * push * other.GetComponent<Enemy>().pushed, ForceMode2D.Impulse);
                targetE.curHP -= damage;
                targetE.StartDamaged();
            }
        }

        if (other.gameObject.tag == "Bullet")
        {
            if (other.GetComponent<SpriteRenderer>().color != Color.green)
                return;

            if (shotType == ShotType.sword)
            {
                if (skillLevel <= 1)
                    Destroy(other.gameObject);
                else if (skillLevel > 1 && !other.GetComponent<Bullet>().reverse)
                    other.gameObject.GetComponent<Bullet>().ReverseDir();
            }
        }
    }

    void Update()
    {
        UpgradeSkill();

        if (shotType != ShotType.sword)
        {
            range -= Time.deltaTime;
            if (range <= 0)
                Destroy(this.gameObject);
        }

        if (shotType == ShotType.lazer)
            ShootLazer();
        else if (shotType == ShotType.guided)
            GuidedShoot();
        else if (shotType == ShotType.sword)
            Sword();
        else
            Shoot(shootDir * 3f);

        // if (shotType == ShotType.lazer && lazerDmgCool != 0)
        //     lazerDmgCool -= Time.deltaTime;
        // else if (shotType == ShotType.lazer && lazerDmgCool <= 0)
        //     lazerDmgCool = 0.3f;
    }
}
