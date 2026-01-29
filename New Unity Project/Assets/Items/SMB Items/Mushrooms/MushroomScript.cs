using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomScript : MonoBehaviour
{
    private bool poppedUp = false;

    private AudioSource audioSource;
    public AudioClip getItemSound;

    Rigidbody rb;
    BoxCollider boxCollision;

    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        boxCollision = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Start Couroutine if popped up
        if (!poppedUp)
            StartCoroutine(PopUpFromBlock());
        else
        {
            // Move the mushroom

            transform.Translate(Vector3.forward * 2 * Time.deltaTime);
            rb.useGravity = true;
            boxCollision.isTrigger = false;
        }
    }



    IEnumerator PopUpFromBlock()
    {
        boxCollision.isTrigger = true;
        transform.Translate(Vector3.up * 1f * Time.deltaTime, Space.World);
        
        yield return new WaitForSeconds(1.5f);
        poppedUp = true;
    }

    private void OnCollisionEnter(Collision other)
    {

        // Give the player an effect and then destroy itself
        if (other.gameObject.name == "Player")
        {
            AudioSource.PlayClipAtPoint(getItemSound, transform.position, 10f);
            other.transform.localScale *= 1.5f;
            Destroy(this.gameObject);
        }
    }

}
