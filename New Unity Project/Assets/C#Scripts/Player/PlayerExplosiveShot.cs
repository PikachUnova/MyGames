using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExplosiveShot : PlayerShot
{
    public GameObject explosion;

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
            dealDamage.TakeDamage(attackPower);
            AudioSource.PlayClipAtPoint(hitObstacle, transform.position, 100f);
            Instantiate(explosion, transform.position, transform.rotation);
            ExplodeInRange();
            Destroy(this.gameObject);
        }

        // The projectile is destroyed when it hits the ground, wall, etc.
        if (other.gameObject.CompareTag("HitSurface"))
        {
            AudioSource.PlayClipAtPoint(hitObstacle, transform.position, 100f);
            
            Instantiate(explosion, transform.position, transform.rotation);
            ExplodeInRange();
            Destroy(this.gameObject);
        }
    }


    void ExplodeInRange()
    {
        // Blow Up nearby Enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(this.transform.position, enemy.transform.position);
            if (distance <= 10)
            {
                EnemyHealth dealDamage = enemy.gameObject.GetComponent<EnemyHealth>();
                dealDamage.TakeDamage(20);
            }
        }
    }


}