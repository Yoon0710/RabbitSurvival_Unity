using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spark : MonoBehaviour
{
    GameObject thisEnemyPos;
    GameObject nextEnemyPos;
    // bool isSparked = false;
    private float lifeTime = 0.5f;

    void Awake()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            thisEnemyPos = other.gameObject;
            Transform nextSparkPos = other.GetComponent<Enemy>().SparkDamaged();

            if (nextSparkPos == null || SkillManager.Instance.sparkN == 0)
            {
                this.transform.localScale = new Vector2(1, 0.7f);
                return;
            }

            nextEnemyPos = nextSparkPos.gameObject;

            float distance = Vector2.Distance(this.transform.position, nextEnemyPos.transform.position) + 0.35f;
            this.transform.localScale = new Vector2(distance <= 0.036f ? 0.01f : distance, 0.7f);

            Vector3 shootDir = (nextEnemyPos.transform.position - this.transform.position).normalized;
            Vector3 qDir = Quaternion.Euler(0, 0, 0) * shootDir;
            Quaternion targetDir = Quaternion.LookRotation(forward: Vector3.forward, upwards: qDir);
            this.transform.localRotation = Quaternion.Euler(0, 0, targetDir.eulerAngles.z + 90f);

            // isSparked = true;

            SkillManager.Instance.sparkN--;
        }
    }

    void Update()
    {
        // if (isSparked)
        // {
        //     float distance = Vector2.Distance(this.transform.position, nextEnemyPos.transform.position);
        //     this.transform.localScale = new Vector2(distance - 0.8f, 2);

        //     Vector3 shootDir = (nextEnemyPos.transform.position - this.transform.position).normalized;
        //     Vector3 qDir = Quaternion.Euler(0, 0, 0) * shootDir;
        //     Quaternion targetDir = Quaternion.LookRotation(forward: Vector3.forward, upwards: qDir);
        //     this.transform.localRotation = Quaternion.Euler(0, 0, targetDir.eulerAngles.z + 90f);
        // }

        if (lifeTime <= 0)
            Destroy(this.gameObject);
        else
            lifeTime -= Time.deltaTime;
    }
}
