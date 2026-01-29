using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWaveMovement : MonoBehaviour
{
    private float speed = 20f;

    public bool doesMove = false;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (doesMove)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Hit an enemy and deal damage
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth.playerHealth.TakeDamage(10);
        }
    }

}
