using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LevelSetting))]
public class Character : MonoBehaviour
{
    public float speed = 5f;

    private SpriteRenderer spriteRenderer;
    private GameUtil.enColor encolor = GameUtil.enColor.Red;
    private LevelSetting lvSetting;
    private bool isDamaged = false;

    void Start()
    {
        GameManager.Instance.InitCharacter(this);
        GameManager.Instance.Init();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        // spriteRenderer.color = GameUtil.GetColor(GameUtil.enColor.Red);
        lvSetting = GetComponent<LevelSetting>();
        speed = GameManager.Instance.playerSpeed * 2f;
        GameManager.Instance.StartTime();
    }

    void Update()
    {
        Vector2 joystickPos = GameManager.Instance.JoystickM.Coordinate();
        Vector2 SkillstickPos = GameManager.Instance.SkillJoystickM.Coordinate();
        Vector2 AttackstickPos = GameManager.Instance.AttackJoystickM.Coordinate();
        // if (joystickPos != Vector2.zero)
        // {
        //     Vector3 qDir = Quaternion.Euler(0, 0, 0) * joystickPos.normalized;
        //     Quaternion targetDir = Quaternion.LookRotation(forward: Vector3.forward, upwards: qDir);
        //     transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, targetDir.eulerAngles.z);
        // }

        Vector2 characterPos = GameManager.Instance.CharacterM.transform.position;
        // #if UNITY_ANDROID
        // Vector3 newPos = new Vector3(transform.localPosition.x + SkillstickPos.x * speed * Time.deltaTime, transform.localPosition.y + SkillstickPos.y * speed * Time.deltaTime, transform.localPosition.z);
        // if (joystickPos != Vector2.zero)
        if (SkillstickPos != Vector2.zero)
        {
            SkillManager.Instance.MoveDir = SkillstickPos.normalized;
            Vector3 qDir = Quaternion.Euler(0, 0, 0) * SkillstickPos.normalized;
            Quaternion targetDir = Quaternion.LookRotation(forward: Vector3.forward, upwards: qDir);
            this.transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, targetDir.eulerAngles.z);
        }
        else if (AttackstickPos != Vector2.zero)
        {
            SkillManager.Instance.MoveDir = AttackstickPos.normalized;
            Vector3 qDir = Quaternion.Euler(0, 0, 0) * AttackstickPos.normalized;
            Quaternion targetDir = Quaternion.LookRotation(forward: Vector3.forward, upwards: qDir);
            this.transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, targetDir.eulerAngles.z);
        }

        // #else
        // Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Vector3 qDir = Quaternion.Euler(0, 0, 0) * (mousePos - characterPos).normalized;
        // Quaternion targetDir = Quaternion.LookRotation(forward: Vector3.forward, upwards: qDir);
        // this.transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, targetDir.eulerAngles.z);
        // #endif
        // Quaternion targetDir = Quaternion.LookRotation(qDir);
        // transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, targetDir.eulerAngles.z);

        if (joystickPos.magnitude > 0.2f)
        {
            Vector3 newPos = new Vector3(transform.localPosition.x + joystickPos.x * speed * Time.deltaTime, transform.localPosition.y + joystickPos.y * speed * Time.deltaTime, transform.localPosition.z);
            // if (joystickPos != Vector2.zero)
            // SkillManager.Instance.MoveDir = joystickPos.normalized;
            transform.localPosition = newPos;
        }

        if (GameManager.Instance.playerHP <= 0)
        {
            UIManager.Instance.GameOver();
        }
    }

    void LateUpdate()
    {
        ManageScreen();
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.transform.GetComponent<SpriteRenderer>().name == "Heart")
        {
            // Vector2 dir = other.transform.localPosition - transform.localPosition;
            // other.gameObject.GetComponent<Rigidbody2D>().AddForce(dir, ForceMode2D.Force);
            if (GameManager.Instance.playerHP + 1 > GameManager.Instance.playerMaxHP)
                return;
            UIManager.Instance.HPUP(1);
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            if (other.GetComponent<SpriteRenderer>().color == Color.red)
            {
                Destroy(other.gameObject);
                UIManager.Instance.AddScore();
                lvSetting.curEXP += Random.Range(1f, 5f);
            }
            else if (other.GetComponent<SpriteRenderer>().color == new Color(160f / 255f, 160f / 255f, 255f / 255f, 255f / 255f))
            {
                return;
            }
            else
            {
                Destroy(other.gameObject);
                if (isDamaged)
                    return;

                UIManager.Instance.HPDown();
                StartDamaged();
            }
        }
        else if (other.gameObject.tag == "Enemy" && !isDamaged)
        {
            UIManager.Instance.HPDown();
            StartDamaged();
        }
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
        yield return new WaitForSeconds(0.08f);
        sp.color = origin;
        yield return new WaitForSeconds(0.08f);
        sp.color = damagedC;
        yield return new WaitForSeconds(0.08f);
        sp.color = origin;
        isDamaged = false;
    }

    void ManageScreen()
    {
        Vector3 charPos = GameManager.Instance.CharacterM.transform.position;
        if (charPos.x < -50f) charPos.x = -50f;
        if (charPos.x > 50f) charPos.x = 50f;
        if (charPos.y < -50f) charPos.y = -50f;
        if (charPos.y > 50f) charPos.y = 50f;
        this.transform.position = charPos;
        // transform.position = Camera.main.ViewportToWorldPoint(viewPos);

        Camera.main.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -10);
        Camera uiCamera = GameObject.Find("UI Camera").GetComponent<Camera>();
        uiCamera.transform.position = Camera.main.transform.position;
    }

    void ChangeColor(GameUtil.enColor color)
    {
        spriteRenderer.color = GameUtil.GetColor(color);
    }
}
