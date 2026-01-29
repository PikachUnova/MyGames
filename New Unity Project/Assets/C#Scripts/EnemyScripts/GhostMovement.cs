using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostMovement : EnemyMovement
{

    // Update is called once per frame
    void Update()
    {
        //check sight range
        playerInDetectionRange = Physics.CheckSphere(transform.position, detectionRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInDetectionRange && !playerInAttackRange && HasLineOfSightToPlayer())
            Patrol();
        else if (playerInDetectionRange && !playerInAttackRange && HasLineOfSightToPlayer())
            ChasePlayer();
        else if (playerInDetectionRange && playerInAttackRange && HasLineOfSightToPlayer())
            AttackPlayer();
    }



    void Patrol()
    {
        if (!walkPointSet)
        {
            alreadyPlayed = false;
            SearchWalkPoint();
        }
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);


        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
            walkPointSet = true;
    }

    void ChasePlayer()
    {
        // Player spotted sound effect
        if (alreadyPlayed == false)
        {
            audioSource.PlayOneShot(spot);
            alreadyPlayed = true;
        }
        agent.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        
        if (!alreadyAttacked)
        {
            StartCoroutine(AttackTime());
        }
    }

    IEnumerator AttackTime()
    {
        alreadyAttacked = true;
        anim.Play("Attacking");
        PlaySound(attackSound);
        yield return new WaitForSeconds(0.1f);
        if (playerInAttackRange) // Hit or miss the player
            PlayerHealth.playerHealth.TakeDamage(attackPower);
        yield return new WaitForSeconds(2.0f);
        alreadyAttacked = false;
    }

}
