using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBlasterScript : MonoBehaviour
{
    public GameObject firepoint;
    private float shotTimer = 0.0f;

    private Animator anim;

    private void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyShooter fireBill = firepoint.gameObject.GetComponent<EnemyShooter>();
        fireBill.Shoot();
        shotTimer += Time.deltaTime;
        
        if (shotTimer > 5)
        {
            anim.Play("Fire");
            shotTimer = 0;
        }
    }
}
