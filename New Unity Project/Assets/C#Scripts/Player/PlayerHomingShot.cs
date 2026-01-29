using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHomingShot : PlayerShot
{
  
    public float rotationSpeed = 0.5f;
    private float homeTimer = 0.0f;

    // Homing onto enemies
    private Transform currentTarget;
    private bool hasHitTarget = false;

    // Start is called before the first frame update
    void Start()
    {
        FindNearestTarget();
    }

    // Update is called once per frame
    void Update()
    {
        homeTimer += Time.deltaTime;

        if (!hasHitTarget && currentTarget != null)
        {
            // Check line of sight and wait before moving towards the current target
            if (HasLineOfSight(currentTarget) && homeTimer > 1)
            {
                // Move towards the current target
                transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

                // Smoothly rotate towards the current target
                Vector3 direction = currentTarget.position - transform.position;
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }
            else // If no current target, move forward based on the current rotation
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        else // No current target, move forward based on the current rotation
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        
    }

    void FindNearestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Transform nearestTarget = null;
        float nearestDistance = float.MaxValue;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance <= 200 && distance < nearestDistance && HasLineOfSight(enemy.transform))
            {
                nearestDistance = distance;
                nearestTarget = enemy.transform;
            }
        }
        currentTarget = nearestTarget;
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

            hasHitTarget = true;
        }

        // The projectile is destroyed when it hits the ground, wall, etc.
        if (other.gameObject.CompareTag("HitSurface"))
        {
            if (Physics.CheckSphere(transform.position, 1f, groundMask) && residue != null) // Add residue
                Instantiate(residue, transform.position, transform.rotation);

            AudioSource.PlayClipAtPoint(hitObstacle, transform.position, 100f);
            Destroy(this.gameObject);
            Instantiate(particle, transform.position, transform.rotation);
        }
    }

}
