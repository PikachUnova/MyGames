using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    public ParticleSystem particle;
    private AudioSource audioSource;
    public AudioClip collect;
    public LayerMask groundLayer;

    public int value; // Coin's worth

    private Rigidbody rb;
    BoxCollider boxCollision;

    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        rb = GetComponent<Rigidbody>();
        boxCollision = GetComponent<BoxCollider>();

        float randDir = Random.Range(0, 2);

        float randomForceXR = Random.Range(1f, 5f); // Random force magnitude
        float randomForceXL = Random.Range(1f, 5f); // Random force magnitude
        float randomForceY = Random.Range(5f, 8f); // Random force magnitude
        rb.AddForce(Vector3.up * randomForceY, ForceMode.Impulse);

        if (randDir == 0)
            rb.AddForce(Vector3.left * randomForceXL, ForceMode.Impulse);
        else
            rb.AddForce(Vector3.right * randomForceXR, ForceMode.Impulse);
        rb.useGravity = true;
    }

    void Update()
    {
        transform.Rotate(0, 90 * Time.deltaTime, 0);
        timer += Time.deltaTime; // Decrease cooldown timer
    }

    void FixedUpdate()
    {
        // Check if the enemy is grounded using raycasting
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.2f, groundLayer) && timer > 0.8f)
        {
            rb.useGravity = false;
            boxCollision.isTrigger = true;
            rb.isKinematic = true;
        }

    }

        private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameUIHandler.gameUIHandler.score += value;

            AudioSource.PlayClipAtPoint(collect, transform.position, 100f);
            Destroy(this.gameObject);
            Instantiate(particle, transform.position, transform.rotation);
        }
    }
}
