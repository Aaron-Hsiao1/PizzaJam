using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footsteps : MonoBehaviour
{
	public AudioSource footstepsSound;
	public PlayerMovementTutorial pm;
	public float stepRate = 0.4f;
	public float sprintStepRate;
	public float stepCoolDown;

	// Update is called once per frame
	void Update()
	{
		if (pm.state == PlayerMovementTutorial.MovementState.sprinting)
		{
			stepRate = sprintStepRate;
		}
		else
		{
			stepRate = 0.4f;
		}

		stepCoolDown -= Time.deltaTime;
		if ((Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f) && stepCoolDown < 0f && pm.grounded && !pm.sliding)
		{
			footstepsSound.pitch = 1f + Random.Range(-0.2f, 0.2f);
			footstepsSound.Play();
			stepCoolDown = stepRate;
		}
	}
}