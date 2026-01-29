using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonMovement : EnemyMovement
{

    // Shooting
    public GameObject gun;
    public Transform origin;

    private bool specialAttackUsed = false;
    private int choice = 1;
    private float choiceTimer = 0.0f;
    private float choiceCoolDownTime = 2f;

    // Update is called once per frame
    void Update()
    {
        //check sight range
        playerInDetectionRange = Physics.CheckSphere(transform.position, detectionRange, playerLayer);
        playerInShootingRange = Physics.CheckSphere(transform.position, shootingRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInDetectionRange && !playerInAttackRange && !playerInShootingRange && HasLineOfSightToPlayer())
            Stand();
        else if (playerInDetectionRange && !playerInShootingRange && HasLineOfSightToPlayer())
            ChasePlayer();
        else if (playerInShootingRange && !playerInAttackRange && HasLineOfSightToPlayer())
            Shoot();
        else if (playerInShootingRange && playerInAttackRange && HasLineOfSightToPlayer())
            AttackPlayer();
    }

    void Stand()
    {
        alreadyPlayed = false;
        agent.SetDestination(origin.position);

        if (this.transform.position.x == origin.position.x)
            anim.Play("Idle");
        else
            anim.Play("Walking");

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
    }

    void Shoot()
    {
        // Choose different ways to shoot
        choiceTimer += Time.deltaTime;
        if (choiceTimer > choiceCoolDownTime)
        {
            choice = Random.Range(1, 10);
            choiceTimer = 0;
        }

        // 90% chance of shooting fireballs, 10% raining down fireballs
        if (choice < 9)
        {
            agent.SetDestination(transform.position);
            transform.LookAt(player);
            transform.Rotate(transform.rotation.x - 5, transform.rotation.y, transform.rotation.z);
            anim.Play("Shoot");

            EnemyShooter shootPlayer = gun.gameObject.GetComponent<EnemyShooter>();
            shootPlayer.Shoot();
        }
        else 
        {
            agent.SetDestination(transform.position);
            choiceCoolDownTime = 10;
            if (specialAttackUsed == false)
            {
                
                anim.Play("SpecialAttack");
                StartCoroutine(SpecialAttack());
               
            }

        }
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
        yield return new WaitForSeconds(0.5f);
        PlayerHealth.playerHealth.TakeDamage(attackPower);
        yield return new WaitForSeconds(1.0f);
        alreadyAttacked = false;
    }

    IEnumerator SpecialAttack()
    {
        EnemyShooter shootPlayer = gun.gameObject.GetComponent<EnemyShooter>();
        shootPlayer.ShootUpwards();
        yield return new WaitForSeconds(1f);

        specialAttackUsed = true;
        yield return new WaitForSeconds(1f);
        shootPlayer.ShootDownwards();
        yield return new WaitForSeconds(1f);
        specialAttackUsed = false;
        choiceCoolDownTime = 2;

    }

}
