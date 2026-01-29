using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyMovement : EnemyMovement
{
    private int choice = 100;
    private float choiceTimer = 0.0f;
    private float choiceCoolDownTime = 2f;

    public ParticleSystem healParticle;
    public bool canHeal = false;
    public bool canCallMummies = false;

    private float cooldown = 1f; // Cooldown duration for its action
    private float timer = 0f;   // Timer to track cooldown

    public GameObject EnemyPrefab;

    // Update is called once per frame
    void Update()
    {
        if (timer > 0f)
            timer -= Time.deltaTime; // Decrease cooldown timer

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
        anim.Play("Wait");
    }

    void ChasePlayer()
    {
        // Choose different ways to do
        choiceTimer += Time.deltaTime;
        if (choiceTimer > choiceCoolDownTime)
        {
            choice = Random.Range(0, 100);
            choiceTimer = 0;
        }


        if (canCallMummies && choice < 10) // 10% chance of calling its minions
            CallMummies();
        else if (canHeal && (choice >= 10 && choice < 30)) // 20% chance of healing
            Heal();
        else if (choice >= 30 && choice < 50) // 20% chance of crouching to dodge higher attacks
            Crouch();
        else
        {
            agent.SetDestination(player.position);
            agent.isStopped = false;
        }
    }

    void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        Vector3 playerPosition = player.transform.position;
        playerPosition.y = transform.position.y; // Set the Y component to be the same as the object's Y component
        transform.LookAt(playerPosition); // Rotate towards the player but only in the yaw direction (Y-axis)

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attacking"))
        {
            anim.SetTrigger("Attacking");
            PlayerHealth.playerHealth.TakeDamage(attackPower);
        }
    }

    void Crouch()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch"))
        {
            anim.SetTrigger("Crouching");
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
        else
        {
            agent.isStopped = false;
        }
    }

    void Heal()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Skill"))
        {
            // Heal its allies nearby
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (timer <= 0.9f)
            {
                anim.SetTrigger("Special");
                foreach (GameObject enemy in enemies)
                {
                    EnemyHealth healEnemy = enemy.gameObject.GetComponent<EnemyHealth>();
                    float distance = Vector3.Distance(this.transform.position, enemy.transform.position);

                    if (distance <= 20)
                    {
                        // Heal enemy without exceeding beyond its capacity
                        if (healEnemy.health <= healEnemy.maxHealth - 10)
                            healEnemy.GainHealth(10);
                        else
                            healEnemy.GainHealth(healEnemy.maxHealth - healEnemy.health);

                        if (healParticle != null)
                            Instantiate(healParticle, enemy.transform.position, enemy.transform.rotation);
                    }
                }
                timer = cooldown;
            }
        }
    }

    void CallMummies()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Skill"))
        {
            // Summon Enemies
            if (timer <= 0.8f)
            {
                anim.SetTrigger("Special");

                if (EnemyPrefab != null)
                {
                    int randomLocation;

                    randomLocation = Random.Range(-25, 25);

                    Vector3 spawnPoint = new Vector3(transform.position.x + randomLocation, transform.position.y, transform.position.z + randomLocation);
                    GameObject EnemyClone = Instantiate(EnemyPrefab, spawnPoint, Quaternion.identity);
                }
                timer = cooldown;
            }
        }

    }
}
