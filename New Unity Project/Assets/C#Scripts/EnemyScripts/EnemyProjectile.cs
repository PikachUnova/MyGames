using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    int attackPower = 8;

    void OnTriggerEnter(Collider other)
    {
        // Deals damage to the player
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth victim = other.gameObject.GetComponent<PlayerHealth>();
            if (victim != null)
                victim.TakeDamage(attackPower);
            Destroy(this.gameObject);
        }

        // Destroyed upon hitting the wall, ground, etc.
        if (other.gameObject.CompareTag("HitSurface"))
        {
            Destroy(this.gameObject);
        }
    }
}
