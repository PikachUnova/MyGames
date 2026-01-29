using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehavior : MonoBehaviour
{

    public ParticleSystem particle;
    private AudioSource audioSource;
    public AudioClip collect;

    public bool insideBlock = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // The coin "jumps" especially if the player hits the block
        if (insideBlock)
            StartCoroutine(reveal());
        else
            transform.Rotate(0, 0, 90 * Time.deltaTime);
    }

    IEnumerator reveal()
    {
        transform.Rotate(0, 360 * Time.deltaTime, 0);
        transform.Translate(Vector3.up * 4 * Time.deltaTime);
        yield return new WaitForSeconds(0.5f);
        transform.Translate(Vector3.up * -4 * Time.deltaTime);
        yield return new WaitForSeconds(0.5f);

        AudioSource.PlayClipAtPoint(collect, transform.position, 100f);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(collect, transform.position, 100f);
            Destroy(this.gameObject);
            Instantiate(particle, transform.position, transform.rotation);
        }
    }
}
