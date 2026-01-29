using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    // Range
    public float detectionRange, attackRange, shootingRange;

    //States
    public bool playerInDetectionRange, playerInAttackRange, playerInShootingRange;

    //Movement and Rotation
    public float movementSpeed = 3f;
    public float rotationSpeed = 180f;

    //rotational contraints
    private Quaternion originalHeadRotation; // Original rotation of the head when the player is within range
    private Quaternion targetHeadRotation; // Desired rotation of the head
    public float rotationSmoothing = 5f; // Smoothing factor for rotation
    private float startY; // Store the starting y-coordinate

    public NavMeshAgent agent;
    protected Transform player;

    // Layer
    public LayerMask groundLayer, playerLayer, obstacleLayer;

    //Patroling
    public Vector3 walkPoint;
    protected bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public int attackPower = 5;
    protected bool alreadyAttacked = false;
    public BoxCollider boxCollider;

    // Audio Source
    protected AudioSource audioSource;
    public AudioClip spot;
    protected bool alreadyPlayed = false;
    public AudioClip attackSound;

    //Animation 
    public Animator anim;

    // Freeze Effect
    public GameObject freezeEffect; // Visual effect for freezing
    public AudioClip breakIce;
    public ParticleSystem brokenIcePieces;
    private bool isFrozen = false;

    // Shock Effect
    public ParticleSystem shockParticles;


    // Start is called before the first frame update
    void Start()
    {
        // Start position and rotation
        startY = transform.position.y;
        originalHeadRotation = transform.rotation;
        targetHeadRotation = originalHeadRotation;

        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        boxCollider = GetComponent<BoxCollider>();

        anim = this.GetComponent<Animator>();

        if (freezeEffect != null) // The enemy is freezable
            freezeEffect.SetActive(false);

    }


    public bool HasLineOfSightToPlayer()
    {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Use Physics.Linecast to check for obstacles
            RaycastHit hit;
            if (Physics.Linecast(transform.position, player.position, out hit, obstacleLayer))
            {
                // If an obstacle is hit before reaching the player, it's not a clear line of sight
                if (hit.collider.CompareTag("Player"))
                    return true;
                else
                    return false;
            }

        // If no obstacles are hit, there is a clear line of sight
        return true;
    }
    public void Freeze()
    {
        // Check if the enemy is already frozen or unfreezable
        if (isFrozen)
            return;

        // Instantiate a visual effect for freezing
        if (freezeEffect != null && isFrozen == false)
        {
            this.enabled = false;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            anim.enabled = false;
            isFrozen = true;
            freezeEffect.SetActive(true);
            StartCoroutine(UnfreezeEnemyCoroutine(5f));
        }
    }

    private IEnumerator UnfreezeEnemyCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        Unfreeze();
    }

    public void Unfreeze()
    {
        // Check if the enemy is already unfrozen
        if (isFrozen)
        {
            this.enabled = true;
            agent.isStopped = false;
            DestroyIce();
        }
    }

    public void DestroyIce()
    {
        // Deactivate the freeze effect prefab
        if (freezeEffect != null)
        {
            AudioSource.PlayClipAtPoint(breakIce, transform.position, 100f);
            Instantiate(brokenIcePieces, transform.position, transform.rotation);
            freezeEffect.SetActive(false);
            isFrozen = false;
            anim.enabled = true;
        }
    }

    public bool IsFrozen()
    {
        return isFrozen;
    }


    public IEnumerator Shock()
    {
        anim.Play("Hurt");
        this.enabled = false;
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        Instantiate(shockParticles, transform.position, transform.rotation);

        yield return new WaitForSeconds(1f);

        this.enabled = true;
        agent.isStopped = false;
    }

    protected void PlaySound(AudioClip clip)
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

}

