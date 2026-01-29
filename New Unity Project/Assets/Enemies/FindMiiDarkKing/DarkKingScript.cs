using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkKingScript : EnemyMovement
{
    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        UseSkill();
    }

    // Update is called once per frame
    void Update()
    {
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
        if (playerInAttackRange)
            Attack();
        else
            agent.SetDestination(player.position);
    }


    void UseSkill()
    {
        anim.Play("SpecialSkill");
    }
    void Attack()
    {
        if (!alreadyAttacked)
        {
            StartCoroutine(AttackTime());
        }
        StopCoroutine(AttackTime());
    }

    IEnumerator AttackTime()
    {
        alreadyAttacked = true;
        anim.Play("Attacking");
        yield return new WaitForSeconds(3.0f);

        // Unleash a powerful roaring attack
        if (playerInAttackRange)
            PlayerHealth.playerHealth.TakeDamage(attackPower*3);
        alreadyAttacked = false;
        
    }

}
