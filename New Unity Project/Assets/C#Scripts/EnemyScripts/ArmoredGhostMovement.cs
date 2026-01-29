using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoredGhostMovement : EnemyMovement
{
    private int choice = 99;
    private float choiceTimer = 0.0f;
    private float choiceCoolDownTime = 4f;

    public GameObject weapon; // Weapon to throw
    private float Cooldown = 2f; // Cooldown duration for action
    private float Timer = 0f;   // Timer to track cooldown
    public bool canThrow = true;

    public bool canUseFortify = false;
    public bool canCreateShockwave = false;
    public GameObject shockwaveParticle;

    public int throwingClass = 0;

    // Update is called once per frame
    void Update()
    {
        if (Timer > 0f)
            Timer -= Time.deltaTime; // Decrease cooldown timer

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
        agent.isStopped = false;
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
        agent.isStopped = false;

        // Player spotted sound effect
        if (alreadyPlayed == false)
        {
            audioSource.PlayOneShot(spot);
            alreadyPlayed = true;
        }


        // Choose different ways to do
        choiceTimer += Time.deltaTime;
        if (choiceTimer > choiceCoolDownTime)
        {
            choice = Random.Range(0, 100);
            Debug.Log(choice);
            choiceTimer = 0;
        }

        if (choice < 50) // Throw Weapon it holds
        {
            if (canThrow)
                ThrowWeapon();
            else if (canCreateShockwave)
                MakeShockwave();
        }
        else
            agent.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        agent.isStopped = true;
        if (Timer <= 0f) 
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attacking"))
            {
                anim.SetTrigger("Attacking");
                Timer = Cooldown; // Reset cooldown timer
            }
        }
        float rg = 0.83f;
        if (Timer >= rg - 0.008f && Timer <= rg) 
        {
            PlayerHealth.playerHealth.TakeDamage(attackPower);
        }
    }

    void ThrowWeapon()
    {
        if (Timer <= 0f) 
        {
            choiceCoolDownTime = 4f;
            agent.isStopped = true;
            agent.SetDestination(transform.position);

            transform.LookAt(player);

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attacking"))
            {
                anim.SetTrigger("Attacking");
                Timer = Cooldown; // Reset cooldown timer
            }
        }

        float rg = 1.0f;
        if (Timer >= rg - 0.008f && Timer <= rg)  // Release a weapon when done throwing
        {
            Vector3 throwingPoint = new Vector3(transform.position.x + 0.5f, transform.position.y + 1f, transform.position.z);
            if (throwingClass == 1)
                throwingPoint = new Vector3(transform.position.x + 1f, transform.position.y + 2f, transform.position.z + 2f);
            else if (throwingClass == 2)
                throwingPoint = new Vector3(transform.position.x + 2f, transform.position.y + 2f, transform.position.z + 4f);

            Instantiate(weapon, throwingPoint, transform.rotation);
        }

    }


    void MakeShockwave()
    {
        if (Timer <= 0f) // Check if cooldown is complete
        {
            choiceCoolDownTime = 5;
            agent.isStopped = true;
            agent.SetDestination(transform.position);

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attacking"))
            {
                anim.SetTrigger("Attacking");
               Timer = Cooldown; // Reset cooldown timer
            }
        }
        float rg = 0.8f;
        if (Timer >= rg - 0.008f && Timer <= rg)
        { 
            if (playerInAttackRange)
                PlayerHealth.playerHealth.TakeDamage(attackPower);

            Vector3 spawnPoint = new Vector3(transform.position.x + 2f, transform.position.y - 2f, transform.position.z); ;

            Instantiate(shockwaveParticle, spawnPoint, shockwaveParticle.transform.rotation);
        }
    }

}
