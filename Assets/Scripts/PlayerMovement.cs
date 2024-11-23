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

	public float groundDrag;

	public float jumpForce;
	public float jumpCooldown;
	public float airMultiplier;
	bool readyToJump;

	[Header("Crouching")]
	public float crouchSpeed;
	public float crouchYScale;
	private float startYScale;

	[Header("Keybinds")]
	public KeyCode jumpKey = KeyCode.Space;
	public KeyCode sprintKey = KeyCode.LeftShift;
	public KeyCode crouchKey = KeyCode.LeftControl;

	[Header("Ground Check")]
	public float playerHeight;
	public LayerMask whatIsGround;
	public bool grounded;

	[Header("Slope Handling")]
	public float maxSlopeAngle;
	private RaycastHit slopeHit;
	private bool exitingSlope;

	public Transform orientation;

	float horizontalInput;
	float verticalInput;

	Vector3 moveDirection;

	Rigidbody rb;
	[SerializeField] private float JumpHeight = 1.5f;
	private float _verticalVelocity;

	public MovementState state;
	public enum MovementState
	{
		walking,
		sprinting,
		crouching,
		air
	}

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
		if (grounded)
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

		if (Input.GetKeyDown(crouchKey))
		{
			transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
			rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
		}

		if (Input.GetKeyUp(crouchKey))
		{
			transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
		}
	}

	private void MovePlayer()
	{
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

	private void StateHandler()
	{
		Debug.Log("STate Handler");
		//sprinting
		if (Input.GetKey(crouchKey))
		{
			state = MovementState.crouching;
			moveSpeed = crouchSpeed;
		}
		if (grounded && Input.GetKey(sprintKey))
		{
			state = MovementState.sprinting;
			moveSpeed = sprintSpeed;
		}
		//walking
		else if (grounded)
		{
			state = MovementState.walking;
			moveSpeed = walkSpeed;
		}
		else
		{
			state = MovementState.air;
		}
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
}