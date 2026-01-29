using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{

    // Variables that manages health
    public static PlayerHealth playerHealth;
    public int currentHealth;
    public int maxHealth = 100;

    // Player's starting point
    public Vector3 savePoint = new Vector3(0.0f, 0.5f, 0.0f);

    // Checks the character is dead or seriously injured
    public bool isDead = false;
    private bool isLowHP = false;

    // Invulnerability after taking damage.
    private float InvulnerabilityTime = 0;
    private bool isInvincible = false;

    private AudioSource audioSource;
    public AudioClip hurt, death;
    private bool alreadyPlayed = false;

    public GameObject playermodelAnimation;

    //Status Effects
    private bool isBurining = false;
    private float burnTimer = 0.0f;
    private float burnDuration = 10f;

    private Coroutine BurnCoroutine;
    public GameObject burnEffect;

    //private bool isPoisoned = false;
    private Coroutine poisonCoroutine;

    // Drowning
    private float airLeft = 100f;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerHealth.playerHealth != null)
        {
            Destroy(this.gameObject);
            return;
        }
        playerHealth = this;
        DontDestroyOnLoad(this);
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
        burnEffect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        checkHealth();

        InvulnerabilityTime -= Time.deltaTime;
        if (InvulnerabilityTime <= 0)
            isInvincible = false;

        DrainAir();

        if (isBurining)
        {
            burnTimer += Time.deltaTime;
            burnEffect.SetActive(true);
        }

        if (burnTimer > burnDuration)
        {
            StopBurning();
            burnTimer = 0;
        }
    }

    private void checkHealth()
    {
        playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().SetWeight();
        if (currentHealth <= 20)
            isLowHP = true;
        else
            isLowHP = false;

        
        if (currentHealth <= 0) // Check if the player's health runs out
        {
            currentHealth = 0;
            GameUIHandler.gameUIHandler.health = 0;
            GameUIHandler.gameUIHandler.healthFill.rectTransform.sizeDelta = new Vector2(0f,0f);
            StopBurning();

            if (alreadyPlayed == false)
            {
                alreadyPlayed = true;
                if (PlayerMovement.playerMovement.IsInWater())
                    playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayWaterDead();
                else if (!PlayerMovement.playerMovement.IsGrounded())
                    playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayAirDead();
                else
                    playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayDead();

                audioSource.PlayOneShot(death);

                isDead = true;

                StartCoroutine(respawn(3f)); // Respawn after death
            }
        }
    }

    public void TakeDamage(int damage)
    {
        // The player loses health taking damage.
        if (isInvincible != true && isDead == false)
        {
            playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayHurt();

            GameUIHandler.gameUIHandler.health -= damage;
            currentHealth -= damage;
            audioSource.PlayOneShot(hurt);
            InvinciblePeriod();
        }
    }

    public void DrainAir()
    {
        // The player loses air over time when underwater
        if (PlayerMovement.playerMovement.IsInWater())
        {
            GameUIHandler.gameUIHandler.air -= Time.deltaTime;
            airLeft -= Time.deltaTime;
            if (airLeft <= 0)
            {
                TakeDamage(20);
                airLeft = 1f;
            }
        }
        else
        {
            GameUIHandler.gameUIHandler.air = 100.0f;
            airLeft = 100.0f;
        }
    }


    public void StartBurning(int dps)
    {
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
        int damagePerTick = Mathf.FloorToInt(minTime) + 1;

        TakeDamage(damagePerTick);
        while (isBurining)
        {
            yield return wait;
            TakeDamage(damagePerTick);
        }
    }

    public void StopBurning()
    {
        isBurining = false;
        burnEffect.SetActive(false);
        if (BurnCoroutine != null)
            StopCoroutine(BurnCoroutine);
    }


    private void InvinciblePeriod()
    {
        InvulnerabilityTime = 2;
        if (InvulnerabilityTime > 0)
            isInvincible = true;
    }


    private IEnumerator respawn(float time)
    {
        yield return new WaitForSeconds(time);

        CharacterController controller = this.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            this.transform.position = savePoint;
            controller.enabled = true;
        }
        GameUIHandler.gameUIHandler.healthFill.rectTransform.sizeDelta = new Vector2(10f,0f);
        currentHealth = 100;
        GameUIHandler.gameUIHandler.health = 100;

        airLeft = 100f;
        GameUIHandler.gameUIHandler.air = 100f;

        PlayerAnimator animatePlayer = playermodelAnimation.gameObject.GetComponent<PlayerAnimator>();
        animatePlayer.SetLocomotive(0f);
        isLowHP = false;
        isDead = false; 
        alreadyPlayed = false;
        GetComponent<MusicManager>().StartPlay();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(respawn(0.0f));
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public bool IsLowHP()
    {
        return isLowHP;
    }

}
