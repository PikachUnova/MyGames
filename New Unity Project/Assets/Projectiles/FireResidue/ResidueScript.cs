using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResidueScript : MonoBehaviour
{
    public int attackPower = 1;
    private int burnRate = 25;

    private float cooldown = 1.5f; // Cooldown duration for its action
    private float timer = 0f;   // Timer to track cooldown

    void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Destroy(this.gameObject, 9f);
    }

    void Update()
    {
        if (timer > 0.0f)
            timer -= Time.deltaTime; // Decrease cooldown timer

        if (timer <= 0.0f)
        {
            HitInRange();
            timer = cooldown;
        }
    }

    void HitInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(this.transform.position, enemy.transform.position);
            if (distance <= 1.5f)
            {
                EnemyHealth dealDamage = enemy.gameObject.GetComponent<EnemyHealth>();
                dealDamage.TakeDamage(attackPower);
                burnEnemy(dealDamage); // Chance to inflict burn ailment
            }
        }
    }

    void burnEnemy(EnemyHealth enemy)
    {
        int chance = Random.Range(0,100);
        if (chance < burnRate)
            enemy.StartBurning(3);
    }
}
