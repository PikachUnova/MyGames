using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIceShot : PlayerShot
{

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime); // Move projectile
    }

    void OnTriggerEnter(Collider other)
    {
        // Hit an enemy and deal damage
        if (other.gameObject.CompareTag("Enemy"))
        {
            DamageMultiplier(other, (int)elementalType.ice);
            AudioSource.PlayClipAtPoint(hitEnemy, transform.position, 100f);
            Instantiate(particle, this.transform.position, this.transform.rotation);
            Destroy(this.gameObject);
        }

        // The projectile is destroyed when it hits the ground, wall, etc.
        if (other.gameObject.CompareTag("HitSurface"))
        {
            if (Physics.CheckSphere(transform.position, 1f, groundMask) && residue != null) // Add residue
                Instantiate(residue, transform.position, transform.rotation);
                
            AudioSource.PlayClipAtPoint(hitObstacle, transform.position, 100f);
            Instantiate(particle, this.transform.position, this.transform.rotation);
            Destroy(this.gameObject);
        }
    }

}
