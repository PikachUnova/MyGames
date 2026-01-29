using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReboundShot : PlayerShot
{

    // Update is called once per frame
    void Update()
    {
        // Move the projectile forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Check for collisions
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, speed * Time.deltaTime, obstacleLayer))
        {
            AudioSource.PlayClipAtPoint(hitObstacle, transform.position, 100f);

            // Calculate reflection direction and then update projectile direction 
            Vector3 reflection = Vector3.Reflect(transform.forward, hit.normal);
            transform.forward = reflection;
        } 
    }
    void OnTriggerEnter(Collider other)
    {
        // Hit an enemy and deal damage
        if (other.gameObject.CompareTag("Enemy"))
        {
            DamageMultiplier(other, (int)elementalType.fire);
            AudioSource.PlayClipAtPoint(hitEnemy, transform.position, 100f);
            Instantiate(particle, transform.position, transform.rotation);
            Destroy(this.gameObject);
            
        }
    }
}
