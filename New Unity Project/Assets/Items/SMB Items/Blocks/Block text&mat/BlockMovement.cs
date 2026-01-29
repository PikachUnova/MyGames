using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockMovement : MonoBehaviour
{
    public GameObject emptyBlock;
    public GameObject fractures;

    public GameObject item;

    private AudioSource audioSource;
    public AudioClip breakBlock;
    public AudioClip revealItem;

    public enum blockType { breakable, item, multiItem }
    public blockType currentType;

    private bool alreadyHit = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }



    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {

            switch (currentType)
            {
                case blockType.breakable:
                    if (alreadyHit == false)
                        StartCoroutine(BreakBlock());
                    break;
                case blockType.item:
                    if (emptyBlock != null && alreadyHit == false)
                        StartCoroutine(RevealItem());
                    break;
                case blockType.multiItem:

                    break;
                default:
                    currentType = 0;
                    break;
            }
            
        }
    }

    IEnumerator BreakBlock()
    {
        alreadyHit = true;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        yield return new WaitForSeconds(0.05f);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
        yield return new WaitForSeconds(0.1f);
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.75f, transform.position.z);
        yield return new WaitForSeconds(0.05f);
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        yield return new WaitForSeconds(0.1f);
        
        if (fractures != null)
        {
            GameObject fractObj = Instantiate(fractures, transform.position, transform.rotation) as GameObject;

            // Loop through each child of the fractures object
            foreach (Transform child in fractObj.transform)
            {

                var rb = child.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    child.parent = null;

                    // Add random force for scattering
                    Vector3 randomDirection = Random.insideUnitSphere; // Random direction
                    float randomForce = Random.Range(5f, 15f); // Random force magnitude
                    rb.AddForce(randomDirection * randomForce, ForceMode.Impulse);

                    // Add random torque for rotation
                    Vector3 randomTorque = Random.insideUnitSphere * Random.Range(5f, 10f);
                    rb.AddTorque(randomTorque, ForceMode.Impulse);

                    Destroy(child.gameObject, 3f);
                }

                
            }
            Destroy(fractObj, 3f);

        }

        AudioSource.PlayClipAtPoint(breakBlock, transform.position, 10f);
        Destroy(this.gameObject);
    }

    IEnumerator RevealItem()
    {
        alreadyHit = true;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        yield return new WaitForSeconds(0.05f);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
        yield return new WaitForSeconds(0.1f);
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.75f, transform.position.z);
        yield return new WaitForSeconds(0.05f);
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        yield return new WaitForSeconds(0.1f);

        if (item != null)
        {
            Vector3 position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            Instantiate(item, position, transform.rotation);
            AudioSource.PlayClipAtPoint(revealItem, transform.position, 10f);
        }
        yield return new WaitForSeconds(1f);

        Instantiate(emptyBlock, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }



}
