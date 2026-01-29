using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPoisonShot : PlayerShot
{

    // Update is called once per frame
    void FixedUpdate()
    {
        // Move projectile
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Hit an enemy and deal damage
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth dealDamage = other.gameObject.GetComponent<EnemyHealth>();
            if (dealDamage != null)
            {
                dealDamage.TakeDamage(attackPower);
                AudioSource.PlayClipAtPoint(hitEnemy, transform.position, 100f);
                Instantiate(particle, transform.position, transform.rotation);
                poisonEnemy(other); // Chance to inflict burn ailment
                Destroy(this.gameObject);

            }
        }

        // The projectile is destroyed when it hits the ground, wall, etc.
        if (other.gameObject.CompareTag("HitSurface"))
        {
            AudioSource.PlayClipAtPoint(hitObstacle, transform.position, 100f);
            Instantiate(particle, transform.position, transform.rotation);
            Destroy(this.gameObject);
            
        }
    }


    void poisonEnemy(Collider enemy)
    {
        int chance = Random.Range(0, 100);
        if (chance < poisonRate)
            enemy.GetComponent<EnemyHealth>().StartPoison(1);
    }
}
