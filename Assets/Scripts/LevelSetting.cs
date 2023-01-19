using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetting : MonoBehaviour
{
    public int curLv = 1;
    public float curEXP = 0f;
    public float maxEXP = 10f;
    public GameObject LvUpPanel;

    private void Start()
    {
        LvUpPanel = GameObject.Find("LvUP");
        LvUpPanel.SetActive(false);
    }

    private void Update()
    {
        if (curEXP >= (int)maxEXP)
        {
            curLv++;
            curEXP = 0;
            maxEXP = (int)maxEXP + curLv * 5f;
            LvUpPanel.SetActive(true);
            GameManager.Instance.StopTime();
        }
    }
}
