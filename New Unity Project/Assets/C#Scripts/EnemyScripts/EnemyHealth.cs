using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    // Managing health and healthbar
    public int health;
    public int maxHealth;
    public EnemyHealthBar healthBar;

    //Defense
    public int defense = 0;

    public ParticleSystem deathParticle;
    private float particleTimer = 0.0f;

    // Audio
    private AudioSource audioSource;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip hurt;
    [SerializeField] float deathAnimationDuration = 0.8f;

    //Status Effects
    [SerializeField] bool isBurnable = true;
    private bool isBurining = false;
    private float burnTimer = 0.0f;
    private float burnDuration = 10f;
    private Coroutine BurnCoroutine;
    public GameObject burnEffect;

    [SerializeField] bool unfreezable = false;

    [SerializeField] bool canBePoisoned = true;
    private bool isPoisoned = false;
    private float poisonTimer = 0.0f;
    private float poisonDuration = 15f;
    private Coroutine PoisonCoroutine;
    public ParticleSystem poisonEffect;

    [SerializeField] int[] elementalWeaknesses;
    [SerializeField] int [] elementalResistances;
    [SerializeField] int [] elementalImmunities;

    [SerializeField] GameObject itemLoot;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        audioSource = GetComponent<AudioSource>();

        if (burnEffect != null)
        burnEffect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0) // Checks if the health reaches to 0
        {
            healthBar.fill.rectTransform.sizeDelta = new Vector2(0f,0f);
            StartCoroutine(DyingTime());
            if (!unfreezable) // Destroy ice block when defeated frozen
            { 
                EnemyMovement deadEnemy = gameObject.GetComponent<EnemyMovement>();
                if (deadEnemy != null && deadEnemy.IsFrozen() == true)
                    deadEnemy.DestroyIce();             
            }
        }


        if (isBurining)
        {
            burnTimer += Time.deltaTime;
            particleTimer += Time.deltaTime;
            if (particleTimer > 1f)
            {
                burnEffect.SetActive(true);
                particleTimer = 0;
            }
        }

        if (burnTimer > burnDuration)
        {
            StopBurning();
            burnTimer = 0;
        }

        if (isPoisoned)
        {
            poisonTimer += Time.deltaTime;
            particleTimer += Time.deltaTime;
            if (particleTimer > 1f)
            {
                Instantiate(poisonEffect, transform.position, transform.rotation);
                particleTimer = 0;
            }
        }

        if (poisonTimer > poisonDuration)
        {
            StopPoison();
            poisonTimer = 0;
        }

    }

    public void TakeDamage(int damage)
    {
        int totalDamage = damage - defense;

        if (damage - defense < 0)
            totalDamage = 0;
        PlaySound(hurt);
        health -= (totalDamage);
        healthBar.SetHealth(health);
    }

    public void GainHealth(int healthValue)
    {
        health += healthValue;
        healthBar.SetHealth(health);
    }

    IEnumerator DyingTime()
    {
        EnemyMovement dyingAnim = gameObject.GetComponent<EnemyMovement>();
        if (dyingAnim != null)
        {
            dyingAnim.anim.Play("Death");
            dyingAnim.movementSpeed = 0;
            dyingAnim.rotationSpeed = 0;
            dyingAnim.enabled = false;
            dyingAnim.agent.velocity = Vector3.zero;
            dyingAnim.agent.isStopped = true;
        }

        KoopaTroopMovement eyes = gameObject.GetComponent<KoopaTroopMovement>();
        if (eyes != null)
            eyes.closedEyes.SetActive(true);

        yield return new WaitForSeconds(deathAnimationDuration);
        PlaySound(death);
        Instantiate(deathParticle, transform.position, transform.rotation);

        // Drop only if the enemy has an item
        if (itemLoot != null)
        {
            int dropRandomizer = Random.Range(0,100);
            int numDrops = 0;

            if (dropRandomizer < 30)
                numDrops = 1;
            else if (dropRandomizer >= 30 && dropRandomizer < 55)
                numDrops = 2;
            else if (dropRandomizer >= 55 && dropRandomizer < 80)
                numDrops = 3;
            else if (dropRandomizer >= 80)
                numDrops = 5;

            for (int i = 0; i < numDrops; i++)
                Instantiate(itemLoot, transform.position, transform.rotation);
        }
        Destroy(this.gameObject);
    }


    private void PlaySound(AudioClip clip)
    {
        AudioSource tempAudio = new GameObject("Player").AddComponent<AudioSource>();
        tempAudio.transform.position = transform.position;
        tempAudio.clip = clip;
        tempAudio.spatialBlend = 1f; // fully 3D sound
        tempAudio.minDistance = 5f; // distance where it starts to fade
        tempAudio.maxDistance = 30f; 
        tempAudio.volume = 1f;
        tempAudio.Play();
        Destroy(tempAudio.gameObject, clip.length + 0.1f);
    }

    public void StartBurning(int dps)
    {
        if (!isBurnable) return; // Not Burnable

        isBurining = true;
        if (BurnCoroutine != null)
            StopCoroutine(BurnCoroutine);

        if (burnEffect != null)
            BurnCoroutine = StartCoroutine(Burn(dps));
    }

    private IEnumerator Burn(int dps)
    {
        float minTime = 1f / dps;
        WaitForSeconds wait = new WaitForSeconds(minTime);
        int damagePerTick = Mathf.FloorToInt(minTime) + 2;
        
        TakeDamage(damagePerTick + defense);
        while (isBurining)
        {
            yield return wait;
            TakeDamage(damagePerTick + defense);
        }
    }

    public void StopBurning()
    {
        isBurining = false;
        burnEffect.SetActive(false);
        if (BurnCoroutine != null)
            StopCoroutine(BurnCoroutine);
    }

    public void StartPoison(int dps)
    {
        isPoisoned = true;
        if (PoisonCoroutine != null)
            StopCoroutine(PoisonCoroutine);

        if (poisonEffect != null)
            PoisonCoroutine = StartCoroutine(InflictPoison(dps));
    }

    private IEnumerator InflictPoison(int dps)
    {
        float minTime = 1f / dps;
        WaitForSeconds wait = new WaitForSeconds(minTime);
        int damagePerTick = Mathf.FloorToInt(minTime) + 4;

        TakeDamage(damagePerTick + defense);
        while (isPoisoned)
        {
            yield return wait;
            TakeDamage(damagePerTick + defense);
        }
    }

    public void StopPoison()
    {
        isPoisoned = false;
        if (PoisonCoroutine != null)
            StopCoroutine(PoisonCoroutine);
    }

    public bool IsBurnable()
    {
        return isBurnable;
    }

    public bool IsUnfreezable()
    { 
        return unfreezable;
    }

    public bool CanBePoisoned()
    {
        return canBePoisoned;
    }

    public int GetElementalWeakness(int index)
    {
        if (elementalWeaknesses.Length == 0)
            return -1;
        return elementalWeaknesses[index];
    }
    public int GetElementalResistance(int index)
    {
        if (elementalResistances.Length == 0)
            return -1;
        return elementalResistances[index];
    }
    public int GetElementalImmunity(int index)
    {
        if (elementalImmunities.Length == 0)
            return -1;
        return elementalImmunities[index];
    }

}
