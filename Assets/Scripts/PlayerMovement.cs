using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovementTutorial : MonoBehaviour
{
	[Header("Movement")]
	public float moveSpeed;

	public float groundDrag;

	public float jumpForce;
	public float jumpCooldown;
	public float airMultiplier;
	bool readyToJump;

	[HideInInspector] public float walkSpeed;
	[HideInInspector] public float sprintSpeed;

	[Header("Keybinds")]
	public KeyCode jumpKey = KeyCode.Space;

	[Header("Ground Check")]
	public float playerHeight;
	public LayerMask whatIsGround;
	bool grounded;

	public Transform orientation;

	float horizontalInput;
	float verticalInput;

	Vector3 moveDirection;

	Rigidbody rb;
	[SerializeField] private float JumpHeight = 1.5f;
	private float _verticalVelocity;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;

		readyToJump = true;
	}

	private void Update()
	{
		// ground check
		grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

		MyInput();
		SpeedControl();

		// handle drag
		if (grounded)
			rb.linearDamping = groundDrag;
		else
			rb.linearDamping = 0;
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
	}

	private void MovePlayer()
	{
		// calculate movement direction
		moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
		rb.AddForce(moveDirection.normalized * moveSpeed * 10f + new Vector3(0.0f, _verticalVelocity, 0.0f), ForceMode.Force);
		// on ground
		if (grounded)
			rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

		// in air
		else if (!grounded)
			rb.AddForce(moveDirection.normalized * moveSpeed * 10f + new Vector3(0.0f, _verticalVelocity, 0.0f) * airMultiplier, ForceMode.Force);
	}

	private void SpeedControl()
	{
		Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

		// limit velocity if needed
		if (flatVel.magnitude > moveSpeed)
		{
			Vector3 limitedVel = flatVel.normalized * moveSpeed;
			rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
		}
	}

	private void Jump()
	{
		// reset y velocity
		rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

		rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

		_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * -9.81f);
	}
	private void ResetJump()
	{
		readyToJump = true;
	}
}