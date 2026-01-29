using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    private Vector3 startPos;
    public float speedx = 2.0f;
    public float speedy = 2.0f;
    public float speedz = 2.0f;
    public float difference = 5.0f;
    public bool positiveDirection = true;

    private Rigidbody rb;

    //Platform types
    public enum platformType {stationary, moving, falling}
    public platformType currentType;

    // Falling platform
    private bool isFalling = false;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch(currentType)
        {
            case platformType.stationary:
                break;
            case platformType.moving:
                Move();
                break;
            case platformType.falling:
                if (isFalling)
                    StartCoroutine(DelayFalling());  
                break;
        }
    }

    IEnumerator DelayFalling()
    {
        yield return new WaitForSeconds(3f);
        transform.Translate(Vector3.down * 9.8f * Time.deltaTime);
    }

    void Move()
    {
        if (difference <= 0)
            difference = 5.0f;
        if (positiveDirection)
        {
            transform.position = new Vector3(startPos.x + (Mathf.PingPong(speedx * Time.time, difference)), transform.position.y, transform.position.z);
            transform.position = new Vector3(transform.position.x, startPos.y + (Mathf.PingPong(speedy * Time.time, difference)), transform.position.z);
            transform.position = new Vector3(transform.position.x, transform.position.y, startPos.z + (Mathf.PingPong(speedz * Time.time, difference)));
        }
        else
        {
            transform.position = new Vector3(startPos.x - (Mathf.PingPong(speedx * Time.time, difference)), transform.position.y, transform.position.z);
            transform.position = new Vector3(transform.position.x, startPos.y - (Mathf.PingPong(speedy * Time.time, difference)), transform.position.z);
            transform.position = new Vector3(transform.position.x, transform.position.y, startPos.z - (Mathf.PingPong(speedz * Time.time, difference)));
        }
    }

    void OnTriggerEnter(Collider other)
    {

        // Moves the player if the player stands on it.
        if (other.gameObject.tag == "Player")
            other.transform.parent = transform;            

        if (platformType.falling == currentType)
            isFalling = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.transform.parent = null;
            DontDestroyOnLoad(other.gameObject);
        }
    }

}
