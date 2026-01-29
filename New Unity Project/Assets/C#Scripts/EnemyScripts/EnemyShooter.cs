using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    private float shotTimer = 0.0f;
    public float shotCooldown = 1;
    public GameObject enemyShot;

    public ParticleSystem fireMuzzle;

    private AudioSource audioSource;
    public AudioClip fireSound;

    public Transform player;

    // Start is called before the first frame update
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void Shoot()
    {
        shotTimer += Time.deltaTime;
        if (shotTimer > shotCooldown)
        {

            Instantiate(fireMuzzle, this.transform.position, this.transform.rotation);
            Instantiate(enemyShot, this.transform.position, this.transform.rotation);
            audioSource.PlayOneShot(fireSound);
            shotTimer = 0;
        }

    }



    public void ShootUpwards()
    {
        GameObject shot = enemyShot;
        shotTimer += Time.deltaTime;
        if (shotTimer > 0.1f)
        {
            Instantiate(fireMuzzle, this.transform.position, this.transform.rotation);
            shot = Instantiate(enemyShot, this.transform.position, this.transform.rotation);
            shot.transform.Rotate(enemyShot.transform.rotation.x -80, enemyShot.transform.rotation.y, enemyShot.transform.rotation.z);
            audioSource.PlayOneShot(fireSound);
            Destroy(shot.gameObject, 2);
            shotTimer = 0;
        }

    }

    public void ShootDownwards()
    {
        GameObject shot = enemyShot;
        shotTimer += Time.deltaTime;
        if (shotTimer > 0.1f)
        {
            Vector3 abovePlayerTransform = new Vector3(player.transform.position.x, player.transform.position.y + 10, player.transform.position.z);
            shot = Instantiate(enemyShot, abovePlayerTransform, player.transform.rotation);
            shot.transform.Rotate(enemyShot.transform.rotation.x + 90, enemyShot.transform.rotation.y, enemyShot.transform.rotation.z);
            audioSource.PlayOneShot(fireSound);
            shotTimer = 0;
        }
    }

}
