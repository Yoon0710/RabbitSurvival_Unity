using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningUI : MonoBehaviour
{
    public float bossTime = 30000;
    public bool isOn = false;

    void Awake()
    {
        bossTime = 30000;
    }

    void OnEnable()
    {
        isOn = true;
    }

    public void WarningOff()
    {
        if (bossTime <= GameManager.Instance.gameTimer)
        {
            isOn = false;
            this.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        WarningOff();
    }
}
