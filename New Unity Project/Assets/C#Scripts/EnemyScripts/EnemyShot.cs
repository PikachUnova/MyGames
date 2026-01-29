using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShot : MonoBehaviour
{
    public Transform playerTransform;
    public float speed = 15f;
    public float lifetime = 10f;
    private int attackPower = 10;

    public ParticleSystem particle;

    // Raining fire
    private float verticalSpeed;
    public float gravity = 9.81f; // Gravity affecting the fireball

    private AudioSource audioSource;
    public AudioClip hitObstacle, hitEnemy;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerTransform = GameObject.FindWithTag("Player").transform;

        anim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (anim != null)
            anim.Play("Move");

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        Destroy(this.gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Deals damage to the player
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth victim = other.gameObject.GetComponent<PlayerHealth>();
            if (victim != null)
                victim.TakeDamage(attackPower);
            audioSource.PlayOneShot(hitEnemy);
            Destroy(this.gameObject);
            Instantiate(particle, transform.position, transform.rotation);
        }

        // Destroyed upon hitting the wall, ground, etc.
        if (other.gameObject.CompareTag("HitSurface"))
        {
            audioSource.PlayOneShot(hitObstacle);
            Destroy(this.gameObject);
            Instantiate(particle, transform.position, transform.rotation);
        }
    }
}
