using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    void Awake()
    {

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.GetComponent<Enemy>().FireDamaged();
        }
    }

    void Update()
    {

    }
}
