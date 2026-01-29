using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThwompMovement : MonoBehaviour
{
    public GameObject idle;
    public GameObject peek;
    public GameObject fall;


    private Vector3 initialPosition;
    public GameObject detectionPoint;

    // Detecting Players
    public Transform player;
    public float detectionRadius = 10f;
    public float fallingRadius = 10f;

    public float fallSpeed = 0.0f; // Initial fall speed
    public float resetDelay = 2.0f; // Time before the Thwomp resets
    private bool landed = false;
    public LayerMask groundLayer;


    // Start is called before the first frame update
    void Start()
    {
        initialPosition = this.transform.position;
        idle.SetActive(true);
        peek.SetActive(false);
        fall.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(player.position, detectionPoint.transform.position) < fallingRadius && landed == false) // attack
        {
            StartCoroutine(Fall());
        }
        else if (Vector3.Distance(player.position, detectionPoint.transform.position) < detectionRadius) // peek
        {
            idle.SetActive(false);
            peek.SetActive(true);
        }
        else // idle
        {
            idle.SetActive(true);
            peek.SetActive(false);
            fall.SetActive(false);
        }


        if (landed == true)
        {

            // Apply gravity to upward speed
            fallSpeed += 9.81f * Time.deltaTime;
            // Move the enemy upward
            transform.position = new Vector3(transform.position.x, transform.position.y + fallSpeed * Time.deltaTime, transform.position.z);

        }


    }

    IEnumerator Fall()
    {

        idle.SetActive(false);
        peek.SetActive(false);
        fall.SetActive(true);

        // Apply gravity to fall speed
        fallSpeed += 9.81f * Time.deltaTime;

        // Move the enemy downward
        transform.position = new Vector3(transform.position.x, transform.position.y - fallSpeed * Time.deltaTime, transform.position.z);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.1f, groundLayer))
        {
            fallSpeed = 0.0f; // Reset fall speed
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            landed = true;

            yield return new WaitForSeconds(1f);

            transform.position = initialPosition;
            landed = false;
        }
      

    }



}
