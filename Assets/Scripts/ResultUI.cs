using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    void Awake()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            GetComponentInChildren<Button>().onClick?.Invoke();
    }
}
