using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackRangeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 2.0f);
        HitInRange();
    }

    void HitInRange()
    {
        // Slash Down nearby Enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Vector3 scratchRange = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 1f);
            float distance = Vector3.Distance(scratchRange, enemy.transform.position);
            if (distance <= 2)
            {
                EnemyHealth dealDamage = enemy.gameObject.GetComponent<EnemyHealth>();
                dealDamage.TakeDamage(2);
            }
        }
    }

}
