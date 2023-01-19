using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{

    void Awake()
    {

    }

    void Update()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            GameManager.Instance.JoystickM.OnKeyAxis(x, y);
        }
        else if (!GameManager.Instance.JoystickM.isCliked && Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            GameManager.Instance.JoystickM.OnKeyExit();
        }

        if (Input.GetKey(KeyCode.Z))
        {
            SkillManager.Instance.ShotAttack();
        }

        if (Input.GetKey(KeyCode.X))
        {
            SkillManager.Instance.ShotSkill();
        }

    }

    void FixedUpdate()
    {

    }
}
