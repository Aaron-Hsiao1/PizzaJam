using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovementTutorial : MonoBehaviour
{
	[Header("Movement")]
	private float moveSpeed;
	public float walkSpeed;
	public float sprintSpeed;

	public float dashSpeed;
	public float dashSpeedChangeFactor;

	public float groundDrag;

	public float jumpForce;
	public float jumpCooldown;
	public float airMultiplier;
	bool readyToJump;
	public float sprintingFOV;

	[SerializeField] private float JumpHeight = 1.5f;

	[Header("Crouching")]
	public float crouchSpeed;
	public float crouchYScale;
	private float startYScale;

	[Header("Keybinds")]
	public KeyCode jumpKey = KeyCode.Space;
	public KeyCode sprintKey = KeyCode.LeftShift;
	public KeyCode crouchKey = KeyCode.C;

	[Header("Ground Check")]
	public float playerHeight;
	public LayerMask whatIsGround;
	public bool grounded;

	[Header("Slope Handling")]
	public float maxSlopeAngle;
	private RaycastHit slopeHit;
	private bool exitingSlope;

	[Header("Sliding")]
	public float maxSlideTime;
	public float slideForce;
	private float slideTimer;
	public KeyCode slideKey = KeyCode.LeftControl;

	private bool sliding;

	[Header("References")]
	public Transform orientation;
	float horizontalInput;
	float verticalInput;
	Vector3 moveDirection;
	Rigidbody rb;
	public PlayerCam playerCam;

	private float _verticalVelocity;

	public MovementState state;
	public enum MovementState
	{
		walking,
		sprinting,
		crouching,
		sliding,
		dashing,
		air
	}

	public bool dashing;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;

		readyToJump = true;

		startYScale = transform.localScale.y;
	}

	private void Update()
	{
		// ground check
		grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

		StateHandler();
		MyInput();
		SpeedControl();


		// handle drag
		if (state == MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching)
		{
			rb.linearDamping = groundDrag;
		}
		else
		{
			rb.linearDamping = 0;
		}
	}

	private void FixedUpdate()
	{
		if (sliding)
		{
			SlidingMovement();
		}
		MovePlayer();
	}

	private void MyInput()
	{
		horizontalInput = Input.GetAxisRaw("Horizontal");
		verticalInput = Input.GetAxisRaw("Vertical");

		// when to jump
		if (Input.GetKey(jumpKey) && readyToJump && grounded)
		{
			readyToJump = false;

			Jump();

			Invoke(nameof(ResetJump), jumpCooldown);
		}

		if (Input.GetKeyDown(crouchKey) && !sliding)
		{
			transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
			rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
		}

		if (Input.GetKeyUp(crouchKey) && !sliding)
		{
			transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
		}
	}

	private void MovePlayer()
	{
		if (state == MovementState.dashing) return;

		// calculate movement direction
		moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
		rb.AddForce(moveDirection.normalized * moveSpeed * 10f + new Vector3(0.0f, _verticalVelocity, 0.0f), ForceMode.Force);

		//on slope
		if (OnSlope() && !exitingSlope)
		{
			rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

			if (rb.linearVelocity.y > 0)
			{
				rb.AddForce(Vector3.down * 80f, ForceMode.Force);
			}
		}

		// on ground
		if (grounded)
			rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

		// in air
		else if (!grounded)
			rb.AddForce(moveDirection.normalized * moveSpeed * 10f + new Vector3(0.0f, _verticalVelocity, 0.0f) * airMultiplier, ForceMode.Force);
	}

	private void SpeedControl()
	{
		if (OnSlope() && !exitingSlope)
		{
			if (rb.linearVelocity.magnitude > moveSpeed)
			{
				rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
			}
		}
		else
		{
			Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

			// limit velocity if needed
			if (flatVel.magnitude > moveSpeed)
			{
				Vector3 limitedVel = flatVel.normalized * moveSpeed;
				rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
			}
		}
	}

	private void Jump()
	{
		exitingSlope = true;

		// reset y velocity
		rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

		rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

		_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * -9.81f);
	}
	private void ResetJump()
	{
		readyToJump = true;
		exitingSlope = false;
	}

	private float desiredMoveSpeed;
	private float lastDesiredMoveSpeed;
	private MovementState lastState;
	private bool keepMomentum;

	private void StateHandler()
	{
		if (dashing)
		{
			state = MovementState.dashing;
			desiredMoveSpeed = dashSpeed;
			speedChangeFactor = dashSpeedChangeFactor;
		}
		else if (Input.GetKeyUp(slideKey) && sliding)
		{
			StopSlide();
		}
		else if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
		{
			StartSlide();
		}
		else if (Input.GetKey(crouchKey) && !sliding)
		{
			playerCam.DoFov(60f);
			state = MovementState.crouching;
			desiredMoveSpeed = crouchSpeed;
		}
		else if (grounded && Input.GetKey(sprintKey)) //SPRINTING
		{
			state = MovementState.sprinting;
			playerCam.DoFov(sprintingFOV);
			desiredMoveSpeed = sprintSpeed;
		}
		//walking
		else if (grounded)
		{
			playerCam.DoFov(60f);
			state = MovementState.walking;
			desiredMoveSpeed = walkSpeed;
		}
		else
		{
			playerCam.DoFov(60f);
			state = MovementState.air;

			if (desiredMoveSpeed < sprintSpeed)
			{
				desiredMoveSpeed = walkSpeed;
			}
			else
			{
				desiredMoveSpeed = sprintSpeed;
			}
		}

		bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
		if (lastState == MovementState.dashing) keepMomentum = true;

		if (desiredMoveSpeedHasChanged)
		{
			if (keepMomentum)
			{
				StopAllCoroutines();
				StartCoroutine(SmoothlyLerpMoveSpeed());
			}
			else
			{
				StopAllCoroutines();
				moveSpeed = desiredMoveSpeed;
			}
		}

		lastDesiredMoveSpeed = desiredMoveSpeed;
		lastState = state;
	}

	private float speedChangeFactor;
	private IEnumerator SmoothlyLerpMoveSpeed()
	{
		float time = 0;
		float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
		float startValue = moveSpeed;

		float boostFactor = speedChangeFactor;

		while (time < difference)
		{
			moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

			time += Time.deltaTime * boostFactor;

			yield return null;
		}

		moveSpeed = desiredMoveSpeed;
		speedChangeFactor = 1f;
		keepMomentum = false;
	}

	private bool OnSlope()
	{
		if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
		{
			float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
			return angle < maxSlopeAngle && angle != 0;
		}
		return false;
	}

	private Vector3 GetSlopeMoveDirection()
	{
		return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
	}

	private void StartSlide()
	{
		sliding = true;

		transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
		rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

		slideTimer = maxSlideTime;
	}

	private void SlidingMovement()
	{
		Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

		rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

		slideTimer -= Time.deltaTime;

		if (slideTimer <= 0)
		{
			StopSlide();
		}
	}

	private void StopSlide()
	{
		sliding = false;

		transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
	}
}