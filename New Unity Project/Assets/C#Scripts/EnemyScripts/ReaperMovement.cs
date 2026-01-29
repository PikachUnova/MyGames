using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReaperMovement : EnemyMovement
{

    private int choice = 99;
    private float choiceTimer = 0.0f;
    private float choiceCoolDownTime = 2f;

    // Depends on the kind of reaper
    public GameObject curseGasEffect;
    public GameObject barrierGasEffect;
    public bool useCurseGas = true;
    public bool canRevive = false;

    //private float cooldown = 1f; // Cooldown duration for its action
    //private float timer = 0f;   // Timer to track cooldown


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
            Attack();
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
            // Cast a gas spell 
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Skill"))
                anim.SetTrigger("Special");
        }

        // Choose different ways to do
        choiceTimer += Time.deltaTime;
        if (choiceTimer > choiceCoolDownTime)
        {
            choice = Random.Range(0, 100);
            choiceTimer = 0;
        }

        if (choice < 25) // 25% chance of using the attack
            SpecialAttack();
        else if (choice >= 25 && choice < 50 && canRevive)
            UseRevive();
        else 
        {

            agent.SetDestination(player.position);
        }
    }

    void Attack()
    {
        agent.SetDestination(transform.position);

        Vector3 playerPosition = player.transform.position;
        playerPosition.y = transform.position.y; // Set the Y component to be the same as the object's Y component
        transform.LookAt(playerPosition); // Rotate towards the player but only in the yaw direction (Y-axis)

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("AttackStart"))
            anim.SetTrigger("Attacking");
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("AttackEnd"))
            PlayerHealth.playerHealth.TakeDamage(attackPower);
    }

    void UseCurseGas()
    {
        if (curseGasEffect != null)
            Instantiate(curseGasEffect, transform.position, transform.rotation);
    }

    void UseBarrierGas()
    {
        if (barrierGasEffect != null)
            Instantiate(barrierGasEffect, transform.position, transform.rotation);
    }

    void UseRevive()
    {
        GameObject[] partners = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in partners)
        {
            EnemyHealth healEnemy = enemy.gameObject.GetComponent<EnemyHealth>();
            float distance = Vector3.Distance(this.transform.position, enemy.transform.position);

            if (distance <= 20)
            {
                healEnemy.GainHealth(100);
            }
        }
    }

    void SpecialAttack()
    {
        agent.SetDestination(transform.position);

        // Instant-Kill Scythe
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("AttackStart"))
            anim.SetTrigger("Attacking");
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("AttackEnd"))
            PlayerHealth.playerHealth.TakeDamage(999);
    }

}
