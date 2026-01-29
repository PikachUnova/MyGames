using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemMovement : EnemyMovement
{
    private int choice = 99;
    private float choiceTimer = 0.0f;
    private float choiceCoolDownTime = 2f;

    // Depends on the kind of golem
    public bool canUseEarthquake = false;
    public bool canUseFortify = false;

    private float cooldown = 1f; // Cooldown duration for its action
    private float timer = 0f;   // Timer to track cooldown
    public GameObject shockwave;

    private float rockCooldown = 0.01f; // Cooldown duration for its action
    private float rockTimer = 0f;   // Timer to track cooldown
    public GameObject rock;


    // Update is called once per frame
    void Update()
    {
        if (timer > 0f)
            timer -= Time.deltaTime; // Decrease cooldown timer

        if (rockTimer > 0f)
            rockTimer -= Time.deltaTime; // Decrease cooldown timer for the rocks


        //check sight range
        playerInDetectionRange = Physics.CheckSphere(transform.position, detectionRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInDetectionRange && !playerInAttackRange && HasLineOfSightToPlayer())
            Idle();
        else if (playerInDetectionRange && !playerInAttackRange && HasLineOfSightToPlayer())
            ChasePlayer();
        else if (playerInDetectionRange && playerInAttackRange && HasLineOfSightToPlayer())
            AttackPlayer();
    }

    void Idle()
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

        if (choice < 25) // 25% chance of using the attack
            CreateShockWave();
        else if (canUseEarthquake && (choice >= 35 && choice < 60)) // // 25% chance of using the attack
            UseEarthquake();
        else
        {
            agent.SetDestination(player.position);
            agent.isStopped = false;
        }
    }

    void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("AttackStart"))
            anim.SetTrigger("Attacking");
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("AttackEnd"))
            PlayerHealth.playerHealth.TakeDamage(attackPower);
    }


    void Rockslide()
    {
        int randomLocation = Random.Range(-30, 30);
        GameObject rSlide = rock;
        if (rockTimer <= 0.0f)
        {
            Vector3 abovePlayerTransform = 
                new Vector3(player.transform.position.x + randomLocation, player.transform.position.y + 30, player.transform.position.z + randomLocation);
            rSlide = Instantiate(rSlide, abovePlayerTransform, player.transform.rotation);
            rSlide.transform.Rotate(rSlide.transform.rotation.x + 90, rSlide.transform.rotation.y, rSlide.transform.rotation.z);
            rockTimer = rockCooldown;
        }
    }

    void CreateShockWave()
    {
        Vector3 playerPosition = player.transform.position;

        playerPosition.y = transform.position.y; // Set the Y component to be the same as the object's Y component
        transform.LookAt(playerPosition); // Rotate towards the player but only in the yaw direction (Y-axis)

        choiceCoolDownTime = 2f;
        agent.isStopped = true;

        if (shockwave != null && timer <= 0f)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("AttackStart"))
                anim.SetTrigger("Attacking");

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("AttackEnd"))
            {
                timer = cooldown; // Reset cooldown timer
                if (timer == cooldown) // Release a shockwave
                {
                    agent.isStopped = false;
                    Vector3 firingPoint = new Vector3(transform.position.x, transform.position.y - 1.7f, transform.position.z);
                    Instantiate(shockwave, firingPoint, transform.rotation);
                }
            }
        }

    }

    void UseEarthquake()
    {
        if (canUseEarthquake == true)
        {
            Vector3 playerPosition = player.transform.position;

            // Set the Y component to be the same as the object's Y component
            playerPosition.y = transform.position.y;

            // Rotate towards the player but only in the yaw direction (Y-axis)
            transform.LookAt(playerPosition);
            choiceCoolDownTime = 2f;
            agent.isStopped = true;

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("AttackStart"))
            {
                anim.SetTrigger("Attacking");
                agent.isStopped = true;
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("AttackEnd"))
            {
                Rockslide();
                agent.isStopped = false;
            }

            // Stun the player
            //if (PlayerMovement.playerMovement.IsGrounded())
                //PlayerHealth.playerHealth.TakeDamage(attackPower);
        }
    }

    void Fortify()
    {
        if (canUseFortify == true)
        {
            EnemyHealth fortifyEnemy = gameObject.GetComponent<EnemyHealth>();
            fortifyEnemy.defense += 1;
        }
            
    }

}
