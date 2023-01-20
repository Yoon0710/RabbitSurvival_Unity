using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagescript : MonoBehaviour
{
    public GameObject hudDamageText;
    public Transform hudPos;
    public void TakeDamage(int damage)
    {
        GameObject hudText = Instantiate(hudDamageText);
        hudText.transform.position = hudPos.position; 
        hudText.GetComponent<DamageText>().damage = damage;
        Debug.Log(damage);
    }
}