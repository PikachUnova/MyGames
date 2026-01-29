using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooter : MonoBehaviour
{ 
    // Start is called before the first frame update
    public GameObject fire;
    public GameObject fireHome;
    public GameObject fireRebound;
    public GameObject fireExplode;
    public GameObject ice;
    public GameObject electroBall;
    public GameObject poisonBall;

    public GameObject fireBreath;
    public GameObject RainOfFireballs;

    // Limits overshooting
    public bool canShoot;
    public float timeBetweenShots = 0.5f;
    private float timeUntilNextShot;

    private float fireRainTimer = 0.0f;
    private bool usedFireRain = false;

    // Projectile types
    public enum projectileType { fire, ice, electric, poison }
    private projectileType currentType;

    public int maxUpgradeLevel = 3;
    public int [] projectileUpgrades = { 0, 0, 0, 0};

    // Muzzles
    public ParticleSystem fireMuzzle;
    public ParticleSystem fireMuzzleHome;
    public ParticleSystem fireMuzzleRebound;
    public ParticleSystem iceMuzzle;
    public ParticleSystem electricMuzzle;

    // Audio Source
    private AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip iceSound;
    public AudioClip electroSound;

    public GameObject playermodelAnimation;
    private GameObject player; 

    public GameObject [] shotIcons; // Fire, Ice, Electric Icons

    public static PlayerShooter upgradeShooter;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        upgradeShooter = this;

        shotIcons[0].SetActive(true);
        shotIcons[1].SetActive(false);
        shotIcons[2].SetActive(false);
        shotIcons[3].SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<PlayerMovement>().IsTalking()) // Don't shoot while talking
            return;

        FireBreath();
        //FireRain();
        if (usedFireRain)
            fireRainTimer += Time.deltaTime;

        if (Time.time > timeUntilNextShot) // The player can shoot once again
            canShoot = true;

        // The player shoots a projectile and limits shooting
        if (Input.GetMouseButtonDown(0) && canShoot && Time.timeScale == 1 && player.GetComponent<PlayerHealth>().isDead == false)
        {
            Shoot();

            if (player.GetComponent<PlayerMovement>().IsGrounded())
            {
                if ((player.GetComponent<PlayerMovement>().IsMoving() || player.GetComponent<PlayerMovement>().IsRunning()) && !player.GetComponent<PlayerMovement>().IsCrouching())
                    playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayShootInMotion();
                else if (!player.GetComponent<PlayerMovement>().IsCrouching())
                    playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayShoot();
                else
                    playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayShootInMotion();
            }
            else
            {
                playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayShootInMotion();
            }

        }

        Change();   // Change projectile type
    }

    void Shoot()
    {
        canShoot = false;
        timeUntilNextShot = Time.time + timeBetweenShots;

        // Shoots various projectiles depending on the type
        switch (currentType)
        {
            case projectileType.fire:
                ShootFire();
                break;
            case projectileType.ice:
                ShootIce();
                break;
            case projectileType.electric:
                ShootElectroBall();
                break;
            case projectileType.poison:
                ShootPoisonBall();
                break;
        }
    }


    void ShootFire()
    {
        int currentLevel = projectileUpgrades[(int)projectileType.fire]; // current level of the fire projectile
        int randomFunct = Random.Range(0,3);

        GameObject fireType = null;
        if (randomFunct == 0)
        {
            fireType = fire;
            fireMuzzle.GetComponent<ParticleSystem>().Play();
        }
        else if (randomFunct == 1)
        {
            fireType = fireHome;
            fireMuzzleHome.GetComponent<ParticleSystem>().Play();
        }
        else if (randomFunct == 2)
        {
            fireType = fireRebound;
            fireMuzzleRebound.GetComponent<ParticleSystem>().Play();
        }
/*
        else if (randomFunct == 3)
        {
            fireMuzzleType = fireMuzzle;
        }
*/
         switch (currentLevel)
         {
            case 0:
                audioSource.PlayOneShot(fireSound);
                Instantiate(fireType, this.transform.position, this.transform.rotation);
                break;
            case 1:
                TripleShot(fireType, fireSound);
                break;
            case 2:
                QuadrubleShot(fireType, fireSound);
                break;
            case 3:
                FiveShotBurst(fireType, fireSound);
                break;

         }
    }

    void TripleShot(GameObject projectile, AudioClip shootSound)
    {
        GameObject shot1 = projectile;
        GameObject shot3 = projectile;

        audioSource.PlayOneShot(shootSound);

        shot1 = Instantiate(shot1, this.transform.position, this.transform.rotation);
        Instantiate(projectile, this.transform.position, this.transform.rotation);
        shot3 = Instantiate(shot3, this.transform.position, this.transform.rotation);
        shot1.transform.Rotate(projectile.transform.rotation.x, projectile.transform.rotation.y + 10, projectile.transform.rotation.z);
        shot3.transform.Rotate(projectile.transform.rotation.x, projectile.transform.rotation.y - 10, projectile.transform.rotation.z);
    }

    void QuadrubleShot(GameObject projectile, AudioClip shootSound)
    {
        GameObject shot1 = projectile;
        GameObject shot2 = projectile;
        GameObject shot3 = projectile;
        GameObject shot4 = projectile;
        audioSource.PlayOneShot(shootSound);

        shot1 = Instantiate(shot1, this.transform.position, this.transform.rotation);
        shot2 = Instantiate(shot2, this.transform.position, this.transform.rotation);
        shot3 = Instantiate(shot3, this.transform.position, this.transform.rotation);
        shot4 = Instantiate(shot4, this.transform.position, this.transform.rotation);

        shot1.transform.Rotate(projectile.transform.rotation.x, projectile.transform.rotation.y + 10, projectile.transform.rotation.z);
        shot2.transform.Rotate(projectile.transform.rotation.x, projectile.transform.rotation.y + 2, projectile.transform.rotation.z);
        shot3.transform.Rotate(projectile.transform.rotation.x, projectile.transform.rotation.y - 2, projectile.transform.rotation.z);
        shot4.transform.Rotate(projectile.transform.rotation.x, projectile.transform.rotation.y - 10, projectile.transform.rotation.z);
    }

    void FiveShotBurst(GameObject projectile, AudioClip shootSound)
    {
        GameObject shot1 = projectile;
        GameObject shot2 = projectile;
        GameObject shot3 = projectile;
        GameObject shot4 = projectile;
        audioSource.PlayOneShot(shootSound);

        shot1 = Instantiate(shot1, this.transform.position, this.transform.rotation);
        shot2 = Instantiate(shot2, this.transform.position, this.transform.rotation);
        Instantiate(projectile, this.transform.position, this.transform.rotation); // middle shot
        shot3 = Instantiate(shot3, this.transform.position, this.transform.rotation);
        shot4 = Instantiate(shot4, this.transform.position, this.transform.rotation);

        shot1.transform.Rotate(projectile.transform.rotation.x, projectile.transform.rotation.y + 10, projectile.transform.rotation.z);
        shot2.transform.Rotate(projectile.transform.rotation.x, projectile.transform.rotation.y + 5, projectile.transform.rotation.z);
        shot3.transform.Rotate(projectile.transform.rotation.x, projectile.transform.rotation.y - 5, projectile.transform.rotation.z);
        shot4.transform.Rotate(projectile.transform.rotation.x, projectile.transform.rotation.y - 10, projectile.transform.rotation.z);
    }


    void ShootIce()
    {
        int currentLevel = projectileUpgrades[(int)projectileType.ice]; // current level of the ice projectile
        switch (currentLevel)
        {
            case 0:
                audioSource.PlayOneShot(iceSound);
                iceMuzzle.GetComponent<ParticleSystem>().Play();
                Instantiate(ice, this.transform.position, this.transform.rotation);
                break;
            case 1:
                TripleShot(ice, iceSound);
                break;
            case 2:
                QuadrubleShot(ice, iceSound);
                break;
            case 3:
                FiveShotBurst(ice, iceSound);
                break;
        }
    }

    void ShootElectroBall()
    {
        int currentLevel = projectileUpgrades[(int)projectileType.electric]; // current level of the ice projectile
        switch (currentLevel)
        {
            case 0:
                audioSource.PlayOneShot(electroSound);
                electricMuzzle.GetComponent<ParticleSystem>().Play();
                Instantiate(electroBall, this.transform.position, this.transform.rotation);
                break;
            case 1:
                TripleShot(electroBall, electroSound);
                break;
            case 2:
                QuadrubleShot(electroBall, electroSound);
                break;
            case 3:
                FiveShotBurst(electroBall, electroSound);
                break;
        }
    }

    void ShootPoisonBall()
    {
        int currentLevel = projectileUpgrades[(int)projectileType.poison]; // current level of the ice projectile
        switch (currentLevel)
        {
            case 0:
                audioSource.PlayOneShot(fireSound);
                //Instantiate(poisonMuzzle, this.transform.position, this.transform.rotation);
                Instantiate(poisonBall, this.transform.position, this.transform.rotation);
                break;
            case 1:
                TripleShot(poisonBall, fireSound);
                break;
            case 2:
                QuadrubleShot(poisonBall, fireSound);
                break;
            case 3:
                FiveShotBurst(poisonBall, fireSound);
                break;
        }
    }

    void Change()
    {
        if (Input.GetKeyDown("f"))
        {
            currentType++;
            if ((int)currentType >= 4)
                currentType = 0;

            switch (currentType)
            {
                case projectileType.fire:
                    shotIcons[0].SetActive(true);
                    shotIcons[1].SetActive(false);
                    shotIcons[2].SetActive(false);
                    shotIcons[3].SetActive(false);
                    break;
                case projectileType.ice:
                    shotIcons[0].SetActive(false);
                    shotIcons[1].SetActive(true);
                    shotIcons[2].SetActive(false);
                    shotIcons[3].SetActive(false);
                    break;
                case projectileType.electric:
                    shotIcons[0].SetActive(false);
                    shotIcons[1].SetActive(false);
                    shotIcons[2].SetActive(true);
                    shotIcons[3].SetActive(false);
                    break;
                case projectileType.poison:
                    shotIcons[0].SetActive(false);
                    shotIcons[1].SetActive(false);
                    shotIcons[2].SetActive(false);
                    shotIcons[3].SetActive(true);
                    break;
            }
        }
    }

    public void FireBreath()
    {
        PlayerAnimator animatePlayer = playermodelAnimation.gameObject.GetComponent<PlayerAnimator>();
        if (Input.GetKeyDown("l")) {
            fireBreath.GetComponent<ParticleSystem>().Play();
            animatePlayer.PlayBreathAttack();
        } 
        else if(Input.GetKeyUp("l")){
            fireBreath.GetComponent<ParticleSystem>().Stop();
            animatePlayer.PlayStopBreathAttack();
        }
    }

    public void FireRain() //Testing
    {
        if (Time.time > timeUntilNextShot) // The player can shoot once again
            canShoot = true;

        if (player.GetComponent<PlayerMovement>().IsCrouching() || player.GetComponent<PlayerMovement>().IsAiming())
            return;

        PlayerAnimator animatePlayer = playermodelAnimation.gameObject.GetComponent<PlayerAnimator>();
        if (Input.GetKeyDown("l") && !usedFireRain)
        {
            animatePlayer.PlayBreathAttack();
            player.GetComponent<PlayerMovement>().MoveLookTarget(0f, 4.14f, 0f);
            usedFireRain = true;
            player.GetComponent<PlayerMovement>().SetUsingSpecialStatus(true);
        }


        if (fireRainTimer >= 0.5f && fireRainTimer < 5f)
        {
            if (canShoot)
            {
                Instantiate(fire, this.transform.position, this.transform.rotation);
                fireMuzzle.GetComponent<ParticleSystem>().Play();
                audioSource.PlayOneShot(fireSound);
                canShoot = false;
                Debug.Log("count");
                timeUntilNextShot = Time.time + 0.08f;
            }
        }
        else if (fireRainTimer >= 5f)
        {
            animatePlayer.PlayStopBreathAttack();
            player.GetComponent<PlayerMovement>().MoveLookTarget(0f, -4.14f, 0f);
            fireRainTimer = 0f;
            usedFireRain = false;
            player.GetComponent<PlayerMovement>().SetUsingSpecialStatus(false);
            Vector3 rainingPosition =
                new Vector3(player.transform.position.x + 35f, player.transform.position.y + 30f, player.transform.position.z);
            Instantiate(RainOfFireballs, rainingPosition, RainOfFireballs.transform.rotation);
        }
    }
    
}
