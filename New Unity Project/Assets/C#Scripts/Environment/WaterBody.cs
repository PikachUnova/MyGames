using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBody : MonoBehaviour
{
	public PlayerMovement player;
    public PlayerAnimator animator;
    void Start()
	{
		player = FindObjectOfType<PlayerMovement>();
        animator = FindObjectOfType<PlayerAnimator>();
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<PlayerMovement>() == player)
		{
			if (!player.IsInWater())
			{
                player.EnterOrExitWater(true);
            }

		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<PlayerMovement>() == player)
		{
			if (player.IsInWater())
			{
				player.EnterOrExitWater(false);
			}
		}
	}
}
