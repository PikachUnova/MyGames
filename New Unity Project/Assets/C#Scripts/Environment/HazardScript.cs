using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerHealth victim = other.gameObject.GetComponent<PlayerHealth>();
        // Deals damage to the player
        if (other.gameObject.CompareTag("Player") && victim.isDead == false)
        {
            if (victim != null)
                victim.TakeDamage(100);
        }

    }

}
