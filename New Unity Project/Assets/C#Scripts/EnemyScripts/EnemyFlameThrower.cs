using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlameThrower : MonoBehaviour
{
    ParticleSystem system;

    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        system = this.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {

        LayerMask playerMask = LayerMask.GetMask("Player");

        RaycastHit hit;

        // Hurt Player
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, playerMask) && HasLineOfSightToPlayer())
        {
            PlayerHealth.playerHealth.TakeDamage(10);
            PlayerHealth.playerHealth.StartBurning(3);
        }
        

    }

    public bool HasLineOfSightToPlayer()
    {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            LayerMask wallMask = LayerMask.GetMask("Wall");

            // Use Physics.Linecast to check for obstacles
            RaycastHit hit;
            if (Physics.Linecast(transform.position, player.position, out hit, wallMask))
            {
                // If an obstacle is hit before reaching the player, it's not a clear line of sight
                if (hit.collider.CompareTag("Player"))
                    return true;
                else
                    return false;
            }

        // If no obstacles are hit, there is a clear line of sight
        return true;
    }
}
