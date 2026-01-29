using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoopaTroopMovement : EnemyMovement
{

    // Facial Expressions
    public GameObject openEyes;
    public GameObject qrtOpenEyes;
    public GameObject halfOpenEyes;
    public GameObject closedEyes;
    public GameObject angryEyes;

    public List<GameObject> walkpoints;
    private int index = 0;
    public bool canMove = true;

    private bool alreadySpotted = false;
    //private bool alreadyGaveUp = false;

    void Start()
    {
        player = GameObject.Find("Player").transform;

        openEyes.SetActive(true);
        qrtOpenEyes.SetActive(false);
        halfOpenEyes.SetActive(false);
        closedEyes.SetActive(false);
        angryEyes.SetActive(false);

        agent.destination = walkpoints[0].transform.position;

        if (canMove == false)
            anim.Play("Idle");
        else
            anim.Play("Walk");

        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 7f));

            openEyes.SetActive(false);
            qrtOpenEyes.SetActive(true);
            yield return new WaitForSeconds(.1f);

            qrtOpenEyes.SetActive(false);
            halfOpenEyes.SetActive(true);
            yield return new WaitForSeconds(.2f);

            halfOpenEyes.SetActive(false);
            closedEyes.SetActive(true);
            yield return new WaitForSeconds(.3f);

            halfOpenEyes.SetActive(true);
            closedEyes.SetActive(false);
            yield return new WaitForSeconds(.2f);

            qrtOpenEyes.SetActive(true);
            halfOpenEyes.SetActive(false);
            yield return new WaitForSeconds(.1f);

            openEyes.SetActive(true);
            qrtOpenEyes.SetActive(false);
        }
    }


    void Update()
    {

        playerInDetectionRange = Physics.CheckSphere(transform.position, detectionRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);


        if (!playerInDetectionRange && !playerInAttackRange && HasLineOfSightToPlayer())
        {
            
            openEyes.SetActive(true);
            angryEyes.SetActive(false);
            Patrol();
        }
        else if (playerInDetectionRange && !playerInAttackRange && HasLineOfSightToPlayer())
        {
            ChasePlayer();
        }
        else if (playerInDetectionRange && playerInAttackRange && HasLineOfSightToPlayer())
            AttackPlayer();
    }

    void Patrol()
    {

        if (alreadySpotted == true)
        {
            StartCoroutine(GiveUp());
            alreadySpotted = false;
        }

        if (canMove == false && agent.isStopped == false)
        {
            agent.destination = walkpoints[index].transform.position;
            if (this.transform.position.x == walkpoints[index].transform.position.x)
                anim.Play("Idle");
            else
                anim.Play("Walk");
        }
        else if (agent.remainingDistance < .15f && agent.isStopped == false)
        {
            StartCoroutine(Turn());
        }

    }

    IEnumerator Turn()
    {
        anim.Play("WalkEnd");
        index = Random.Range(0, walkpoints.Count);
        agent.destination = walkpoints[index].transform.position;
        yield return new WaitForSeconds(.2f);
        anim.Play("Turn");
        yield return new WaitForSeconds(.5f);
        anim.Play("Walk");
    }

    IEnumerator SpotPlayer()
    {
        agent.isStopped = true;
        anim.Play("Find");
        yield return new WaitForSeconds(1f);

        openEyes.SetActive(false);
        angryEyes.SetActive(true);
        anim.Play("RunStart");

        yield return new WaitForSeconds(0.1f);
        agent.isStopped = false;
        anim.Play("Run");

    }


    void ChasePlayer()
    {
        if (alreadySpotted == false)
        {
            StartCoroutine(SpotPlayer());
            alreadySpotted = true;
        }
        else
            agent.SetDestination(player.position);

    }

    IEnumerator GiveUp()
    {
        anim.Play("RunEnd");
        agent.isStopped = true;
        yield return new WaitForSeconds(0.3f);

        anim.Play("GiveUp");
        openEyes.SetActive(true);
        angryEyes.SetActive(false);
        yield return new WaitForSeconds(3f);

        agent.isStopped = false;
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
        agent.isStopped = true;
        yield return new WaitForSeconds(0.5f);
        PlayerHealth.playerHealth.TakeDamage(attackPower);
        yield return new WaitForSeconds(1.0f);
        agent.isStopped = false;
        alreadyAttacked = false;
    }



}
