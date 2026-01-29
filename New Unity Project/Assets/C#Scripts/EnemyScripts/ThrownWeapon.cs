using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownWeapon : MonoBehaviour
{
    public float speed = 16f;
    public float lifetime = 10f;
    public int attackPower = 12;

    public bool isSpear = true;

    private AudioSource audioSource;
    public AudioClip throwWeapon, hitObstacle;
    private float playCooldown = 1.5f; // Cooldown duration for throwing spears
    private float playTimer = 0f;   // Timer to track cooldown

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playTimer > 0f)
            playTimer -= Time.deltaTime; // Decrease cooldown timer

        if (isSpear)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            transform.Rotate(0, 0, 1800 * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
            transform.Rotate(1800 * Time.deltaTime, 0, 0);
        }


        if (playTimer <= 0f) // Check if cooldown is complete
        {
            audioSource.PlayOneShot(throwWeapon);
            playTimer = playCooldown; // Reset cooldown timer
        }
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
            Destroy(this.gameObject);
            //Instantiate(particle, transform.position, transform.rotation);
        }

        // Destroyed upon hitting the wall, ground, etc.
        if (other.gameObject.CompareTag("HitSurface"))
        {
            //audioSource.PlayOneShot(hitObstacle);
            //Destroy(this.gameObject);
            
        }
    }
}
