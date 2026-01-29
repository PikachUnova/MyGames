using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMovement : EnemyMovement
{
    // Special Skills depending on the slime type
    public bool canUseHealingMagic = false;
    public bool canUseGrowthMagic = false;

    public GameObject healEffect;
    private bool healingUsed = false;

    public int dodgeRate = 50;

    // Update is called once per frame
    void Update()
    {
        // Grow bigger and stronger at the certain remaining health reached
        EnemyHealth checkHealth = this.gameObject.GetComponent<EnemyHealth>();
        if (canUseGrowthMagic)
        {
            if (checkHealth.health <= checkHealth.maxHealth / 2)
            {
                Grow();
                canUseGrowthMagic = false;
            }
        }       

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

        // Use healing magic
        if (healingUsed == false && canUseHealingMagic)
            StartCoroutine(Heal());
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
        anim.Play("Walking");

        // Use healing magic
        if (healingUsed == false && canUseHealingMagic)
            StartCoroutine(Heal());
    }

    void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        Vector3 playerPosition = player.transform.position;
        playerPosition.y = transform.position.y; // Set the Y component to be the same as the object's Y component
        transform.LookAt(playerPosition); // Rotate towards the player but only in the yaw direction (Y-axis)

        if (!alreadyAttacked)
            StartCoroutine(AttackTime());
    }

    IEnumerator AttackTime()
    {
        alreadyAttacked = true;
        anim.Play("Attacking");
        yield return new WaitForSeconds(0.5f);
        if (playerInAttackRange) // Whether it hits or misses the player by range
            PlayerHealth.playerHealth.TakeDamage(attackPower);
        yield return new WaitForSeconds(2.0f);
        alreadyAttacked = false;
    }

    public bool Dodge()
    {
        int chance = Random.Range(0, 100);
        if (chance <= dodgeRate)
        {
            // The slime occasionally evades the player's melee attack
            anim.Play("Dodge");
            return true;
        }
        return false;
    }

    IEnumerator Heal()
    {
        healingUsed = true;
        anim.Play("Skill");
        agent.isStopped = true;

        // Heal its allies nearby
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyHealth healEnemy = enemy.gameObject.GetComponent<EnemyHealth>();
            float distance = Vector3.Distance(this.transform.position, enemy.transform.position);

            if (distance <= 20)
            {
                // Heal enemy without exceeding beyond its capacity
                if (healEnemy.health <= healEnemy.maxHealth-10)
                    healEnemy.GainHealth(10);
                else
                    healEnemy.GainHealth(healEnemy.maxHealth - healEnemy.health);

                if (healEffect != null)
                    Instantiate(healEffect, enemy.transform.position, enemy.transform.rotation);
            }
        }

        yield return new WaitForSeconds(2f);
        agent.isStopped = false;

        yield return new WaitForSeconds(8f);
        healingUsed = false;
    }

    void Grow()
    {
        this.transform.localScale *= 2f;
        attackPower += 10;
    }

}
