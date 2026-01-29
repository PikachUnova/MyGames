using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimator : MonoBehaviour
{
    public MultiAimConstraint aimConstraint;
    private Animator anim;

    // Audio Source
    private AudioSource audioSource;
    public AudioClip landSound;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("MovementSpeed", 0f);
        audioSource = GetComponent<AudioSource>();
    }

    public void SetLocomotive(float magnitude)
    {
        if (magnitude > -0.05f && magnitude < 0.05f)
            SetDirection(0f);
        anim.SetFloat("MovementSpeed", magnitude, .2f, Time.deltaTime);
    }

    public void SetDirection(float axis)
    {
        anim.SetFloat("Direction", axis, .2f, Time.deltaTime);
    }

    public void PlayShootInMotion()
    {
        anim.Play("ShootInMotion");
    }

    public void PlayShoot()
    {
        anim.Play("Shooting");
    }

    public void PlayClawAttack1()
    {
        anim.Play("Attack1");
    }

    public void PlayClawAttack2()
    {
        anim.Play("Attack2");
    }

    public void PlayBiteAttack()
    {
        anim.Play("Bite");
    }

    public void PlayTailAttack()
    {
        anim.Play("TailWhip");
    }

    public void PlayJumpStart()
    {
        anim.Play("JumpStart");
    }

    public void PlayJumpCrouching()
    {
        anim.Play("CrouchJump");
    }

    public void PlayFall()
    {
        anim.Play("Falling");
    }

    public void PlayFallCrouching()
    {
        anim.Play("CrouchFall");
    }

    public void PlayLand()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
        {
            anim.Play("Land");
            audioSource.PlayOneShot(landSound); // Play jump sound
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("CrouchFall"))
        {
            anim.Play("CrouchLand");
            audioSource.PlayOneShot(landSound); // Play jump sound
        }
    }

    public void PlayCrouch()
    {
        anim.SetTrigger("Crouch");
    }

    public void PlayCrouchGetUp()
    {
        anim.SetTrigger("StandUp");
    }

    public void PlayCrawl()
    {
        anim.SetFloat("MovementSpeed", 0.5f, .2f, Time.deltaTime);
    }

    public void PlayHurt()
    {
        anim.Play("Hurt");
    }

    public void SetWeight()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("FireBreathStart") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("TailAttack") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Hurt") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Death") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("MidairDeath") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("FallingImpact") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("LeftDodge") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("RightDodge") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("BackDodge") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("WallCling") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("GrabLedge") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("ClimbLocomotive")
            )
        {
            var data = aimConstraint.data;
            WeightedTransformArray sources = data.sourceObjects;

            var firstSource = sources.GetTransform(0);
            sources.SetWeight(0, 1.0f); // Set weight

            data.sourceObjects = sources;
            aimConstraint.data = data;
        }
        else
        {
            var data = aimConstraint.data;
            WeightedTransformArray sources = data.sourceObjects;

            var firstSource = sources.GetTransform(0);
            sources.SetWeight(0, 0.0f); // Set weight

            data.sourceObjects = sources;
            aimConstraint.data = data;
        }
    }

    public void PlayDead()
    {
        anim.Play("Death");  
    }

    public void PlayAirDead()
    {
        anim.Play("MidairDeath");
    }

    public void StartSwimming()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("SwimLocomotion"))
            anim.SetTrigger("StartSwim");
    }

    public void PlayWaterDead()
    {
        anim.Play("SwimDeath"); 
    }

    public void PlayBreathAttack()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Locomotive"))
            anim.Play("BreathInMotion");
        else
            anim.Play("FireBreathStart");
    }

    public void PlayStopBreathAttack()
    {
        anim.SetTrigger("StopBreathAttack");
    }

    public bool GetCurrentDodgeAnimation()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("LeftDodge") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("RightDodge") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("BackDodge");
    }

    public void PlayDodgeLeft()
    {
        anim.Play("LeftDodge");
    }
    public void PlayDodgeRight()
    { 
        anim.Play("RightDodge");
    }
    public void PlayDodgeBack()
    { 
        anim.Play("BackDodge");
    }

    public void PlayGrabLedge()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Falling") || anim.GetCurrentAnimatorStateInfo(0).IsName("JumpStart"))
        {
            anim.Play("GrabLedge");
        }
    }

    public void PlayWallCling()
    {
        anim.Play("WallCling");
    }

    public void PlayTakeOff()
    {
        anim.SetTrigger("TakeOff");
    }

    public void PlayClimbLadder(float magnitude)
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("ClimbLocomotive"))
            anim.SetTrigger("Climb");
        anim.SetFloat("MovementSpeed", magnitude, .2f, Time.deltaTime);
    }

    public void PlayGetOffLadder()
    {
        anim.SetTrigger("ClimbOff");
    }

    public void EnableAnimation()
    {
        anim.enabled = true;
    }
    public void DisableAnimation()
    {
        anim.enabled = false;
    }
}

