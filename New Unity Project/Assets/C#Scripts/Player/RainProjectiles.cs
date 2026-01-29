using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainProjectiles : MonoBehaviour
{
    // Avoid overinstantiating
    public bool canShoot;
    public float timeBetweenShots = 0.16f;
    private float timeUntilNextShot;

    public GameObject projectile;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 4.7f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timeUntilNextShot) // The player can shoot once again
            canShoot = true;

        if (canShoot)
        {
            int randomX = Random.Range(-60, 60);
            int randomZ = Random.Range(-60, 60);
            Vector3 randomPosition =
                new Vector3(this.transform.position.x + randomX, this.transform.position.y, this.transform.position.z + randomZ);
            canShoot = false;
            timeUntilNextShot = Time.time + timeBetweenShots;
            Instantiate(projectile, randomPosition, this.transform.rotation);
        }
    }
}
