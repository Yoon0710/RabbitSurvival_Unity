using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class Skillstick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler //, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // public RectTransform Background;
    // public RectTransform Handle;
    // public float offset;
    public Vector2 PointPosition { get; set; }

    public bool isClicked = false;

    private void Start()
    {
        if (this.gameObject.name == "ButtonAtk")
        {
            GameManager.Instance.InitAttackJoystick(this);
        }
        else
        {
            GameManager.Instance.InitSkillJoystick(this);
        }
    }

    private Vector2 FindNearestEnemy()
    {
        var objects = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        if (objects.Count == 0 || objects == null)
            return Vector2.zero;

        // LINQ 메소드를 이용해 가장 가까운 적을 찾습니다.
        var neareastObject = objects
            .OrderBy(obj =>
        {
            return Vector2.Distance(GameManager.Instance.CharacterM.transform.position, obj.transform.position);
        })
        .FirstOrDefault();

        Vector2 shootDir = (neareastObject.transform.position - GameManager.Instance.CharacterM.transform.position).normalized;

        return shootDir;
    }

    private void Update()
    {
        Vector2 dir = FindNearestEnemy();
        if (dir != Vector2.zero)
            PointPosition = dir;

        if (isClicked && this.gameObject.name == "ButtonAtk")
        {
            SkillManager.Instance.ShotAttack();
        }
        else if (isClicked && this.gameObject.name == "ButtonSkill")
        {
            SkillManager.Instance.ShotSkill();
        }
    }

    // public void OnBeginDrag(PointerEventData eventData)
    // {

    // }

    // public void OnDrag(PointerEventData eventData)
    // {
    //     // background = 조이스틱 배경 동그라미, handle = 조이스틱 동그라미 
    //     // (이동한 x - 기본 조이스틱 x) / ((background의 x길이 - handle의 x 길이) / 2 → 파란 부분) → 이동한 비율을 구함 (-1 ~ 1이 나올듯)
    //     // 수치값(비율) --> 변수에 저장 
    //     // PointPosition = new Vector2((eventData.position.x - Background.position.x) / ((Background.rect.size.x - Handle.rect.size.x) / 2), (eventData.position.y - Background.position.y) / ((Background.rect.size.y - Handle.rect.size.y) / 2));

    //     // 범위 바깥 이상으로 드래그 못 하도록 방지 
    //     // PointPosition = (PointPosition.magnitude > 1.0f) ? PointPosition.normalized : PointPosition;

    //     // 변수의 저장된 위치로 핸들의 위치를 옮기기 
    //     // 비율 --> 수치 하기 위해서 위에서 아까 나눴던 값을 다시 곱해주되, offset을 줘서 약간의 차이를 줌
    //     // Handle.transform.position = new Vector2((PointPosition.x * ((Background.rect.size.x - Handle.rect.size.x) / 2) * offset) + Background.position.x, (PointPosition.y * ((Background.rect.size.y - Handle.rect.size.y) / 2) * offset) + Background.position.y);

    //     PointPosition =
    //         new Vector2
    //         (
    //             (eventData.position.x - (Screen.width - Mathf.Abs(Background.anchoredPosition.x) - Mathf.Abs(Background.rect.center.x))) / ((Background.rect.size.x - Handle.rect.size.x) / 2),
    //             (eventData.position.y - Background.rect.center.y) / ((Background.rect.size.y - Handle.rect.size.y) / 2)
    //         );

    //     PointPosition = (PointPosition.magnitude > 1.0f) ? PointPosition.normalized : PointPosition;

    //     // Debug.Log(PointPosition.x + "," + PointPosition.y);

    //     // 핸들의 기준점을 포인트 좌표(-1 ~ 1) x 백그라운드 사이즈(500)를 한 것에 1/2 --> 백그라운드를 넘지 않음 
    //     Handle.anchoredPosition =
    //         new Vector2
    //         (
    //             PointPosition.x * Background.rect.size.x / 2 * offset,
    //             PointPosition.y * Background.rect.size.y / 2 * offset
    //         );
    // }

    // public void OnEndDrag(PointerEventData eventData)
    // {
    //     // 핸들, 포인트 초기화 (드래그 중단, 포인터 중단 시에)
    //     PointPosition = new Vector2(0f, 0f);
    //     Handle.anchoredPosition = new Vector2(0f, 0f);
    // }

    public void OnPointerDown(PointerEventData eventData)
    {
        isClicked = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isClicked = false;
    }

    public Vector2 Coordinate()
    {
        return PointPosition;
    }
}