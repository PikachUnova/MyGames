using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    public int durability = 10;

    private AudioSource audioSource;
    public AudioClip blockShield;
    public AudioClip breakShield;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Update()
    {
        if (durability <= 0) // The shield breaks if it runs out of durability
        {
            AudioSource.PlayClipAtPoint(breakShield, transform.position, 100f);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerProjectile"))
        {
            AudioSource.PlayClipAtPoint(blockShield, transform.position, 100f);
            durability--;
        }
    }
}
