using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShot : MonoBehaviour
{
    [SerializeField] protected ParticleSystem particle;
    [SerializeField] protected float speed = 20;
    [SerializeField] protected int attackPower = 10;

    protected AudioSource audioSource;
    [SerializeField] protected AudioClip hitObstacle, hitEnemy;

    [SerializeField] protected LayerMask groundMask;
    [SerializeField] protected LayerMask obstacleLayer; // The layer for obstacles that block line of sight
    [SerializeField] protected GameObject residue;

    protected enum elementalType { normal, fire, ice, electric, poison, water}

    [SerializeField] protected int burnRate = 25;
    [SerializeField] protected int poisonRate = 80;



    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Destroy(this.gameObject, 5.0f);
    }

    protected bool HasLineOfSight(Transform target)
    {
        RaycastHit hit;
        if (Physics.Linecast(transform.position, target.position, out hit))
        {
            // Check if the hit object is the target
            if (hit.collider.transform == target)
                return true;
        }
        return false;
    }

    protected void BurnEnemy(Collider victim) // Chance of Burn Effect
    {
        int chance = Random.Range(0, 100);
        if (chance < burnRate)
            victim.GetComponent<EnemyHealth>().StartBurning(2);
    }

    protected void FreezeEnemy(EnemyMovement victim)
    {
        victim.Freeze();
    }

    protected void DamageMultiplier(Collider other, int element)
    {
        EnemyHealth victim = other.gameObject.GetComponent<EnemyHealth>();
        if (victim != null)
        {
            if (victim.IsBurnable() && element == (int)elementalType.fire)
                BurnEnemy(other); // Chance to inflict burn ailment

            if (!victim.IsUnfreezable() && element == (int)elementalType.ice)
                FreezeEnemy(other.gameObject.GetComponent<EnemyMovement>()); // Apply freeze effect

            EnemyMovement shock = other.gameObject.GetComponent<EnemyMovement>();
            if (element == (int)elementalType.electric && (victim.GetElementalResistance(0) != (int)elementalType.electric || victim.GetElementalImmunity(0) != (int)elementalType.electric))
                shock.StartCoroutine(shock.Shock()); // Stun an enemy

            if (victim.GetElementalImmunity(0) == element)
                return;
            else if (victim.GetElementalResistance(0) == element)
                victim.TakeDamage(attackPower / 2);
            else if (victim.GetElementalWeakness(0) == element)
                victim.TakeDamage(attackPower * 2);
            else
                victim.TakeDamage(attackPower);
        }
    }
}
