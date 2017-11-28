using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float gravity = 20.0f;
	public float groudFriction = 6;

	public float changeStanceSpeed = 7.0f;
	public float moveSpeed = 9.0f;                // Ground move speed
	public float moveSpeedCrouched = 5.0f;        // Ground move speed while crouched
	public float runAcceleration = 14.0f;         // Ground accel
	public float runDeacceleration = 10.0f;       // Deacceleration that occurs when running on the ground
	public float airAcceleration = 2.0f;          // Air accel
	public float airDecceleration = 2.0f;         // Deacceleration experienced when ooposite strafing
	public float airControl = 0.3f;               // How precise air control is
	public float sideStrafeAcceleration = 50.0f;  // How fast acceleration occurs to get up to sideStrafeSpeed when
	public float sideStrafeSpeed = 1.0f;          // What the max speed to generate when side strafing
	public float jumpSpeed = 8.0f;                // The speed at which the character's up axis gains when hitting jump
	public float moveScale = 1.0f;

	public bool moveAllowed = true;

	private CharacterController characterController;

	private Vector3 playerInputVector;
	private Vector3 moveDirectionNormalized = Vector3.zero;
	private Vector3 playerVelocity = Vector3.zero; // Reference this to get the movement of the player - make a local vector of it
	private float playerTopVelocity = 0.0f;

	private bool wishJump = false;
	private bool isCrouched = false;

	private float playerFriction = 0.0f;

	private void Start()
	{
		characterController = GetComponent<CharacterController>();
	}

	private void Update()
	{
		QueueJump();
		if (moveAllowed)
		{
			if (characterController.isGrounded)
				GroundMove();
			else if (!characterController.isGrounded)
				AirMove();

			characterController.Move(playerVelocity * Time.deltaTime);
		}

		Vector3 udp = playerVelocity;
		udp.y = 0.0f;
		if (playerVelocity.magnitude > playerTopVelocity)
			playerTopVelocity = playerVelocity.magnitude;
	}

	#region QuakeMovementLogic
	private void SetMovementDir()
	{
		playerInputVector.z = Input.GetAxisRaw("Vertical");
		playerInputVector.x = Input.GetAxisRaw("Horizontal");
	}

	private void QueueJump()
	{
		if (Input.GetButtonDown("Jump") && !wishJump)
			wishJump = true;
		if (Input.GetButtonUp("Jump"))
			wishJump = false;
	}

	private void AirMove()
	{
		float wishVelocity = airAcceleration;
		float acceleration;
		float scale = CommandScale();

		SetMovementDir();

		var wishDirection = new Vector3(playerInputVector.x, 0, playerInputVector.z);
		wishDirection = transform.TransformDirection(wishDirection);

		float wishSpeed = wishDirection.magnitude;
		wishSpeed *= (isCrouched ? moveSpeedCrouched : moveSpeed);

		wishDirection.Normalize();
		moveDirectionNormalized = wishDirection;
		wishSpeed *= scale;

		// CPM: Aircontrol
		float wishspeed2 = wishSpeed;
		if (Vector3.Dot(playerVelocity, wishDirection) < 0)
			acceleration = airDecceleration;
		else
			acceleration = airAcceleration;
		// If the player is ONLY strafing left or right
		if (playerInputVector.z == 0 && playerInputVector.x != 0)
		{
			if (wishSpeed > sideStrafeSpeed)
				wishSpeed = sideStrafeSpeed;
			acceleration = sideStrafeAcceleration;
		}

		Accelerate(wishDirection, wishSpeed, acceleration);
		if (airControl > 0)
			AirControl(wishDirection, wishspeed2);
		// !CPM: Aircontrol

		// Apply gravity
		playerVelocity.y -= gravity * Time.deltaTime;
	}


	private void AirControl(Vector3 wishdir, float wishspeed)
	{
		if (Mathf.Abs(playerInputVector.z) < 0.001 || Mathf.Abs(wishspeed) < 0.001)
			return;

		var zSpeed = playerVelocity.y;
		playerVelocity.y = 0;

		var speed = playerVelocity.magnitude;
		playerVelocity.Normalize();

		var dot = Vector3.Dot(playerVelocity, wishdir);
		var k = 32f;
		k *= airControl * dot * dot * Time.deltaTime;

		// Change direction while slowing down
		if (dot > 0)
		{
			playerVelocity.x = playerVelocity.x * speed + wishdir.x * k;
			playerVelocity.y = playerVelocity.y * speed + wishdir.y * k;
			playerVelocity.z = playerVelocity.z * speed + wishdir.z * k;

			playerVelocity.Normalize();
			moveDirectionNormalized = playerVelocity;
		}

		playerVelocity.x *= speed;
		playerVelocity.y = zSpeed; // Note this line
		playerVelocity.z *= speed;
	}

	private void GroundMove()
	{
		if (!wishJump)
			ApplyFriction(1.0f);
		else
			ApplyFriction(0);

		float scale = CommandScale();

		var wishDirection = new Vector3(playerInputVector.x, 0, playerInputVector.z);
		wishDirection = transform.TransformDirection(wishDirection);
		wishDirection.Normalize();
		moveDirectionNormalized = wishDirection;

		var wishSpeed = wishDirection.magnitude;
		wishSpeed *= (isCrouched ? moveSpeedCrouched : moveSpeed);

		Accelerate(wishDirection, wishSpeed, runAcceleration);

		playerVelocity.y = 0;

		if (wishJump)
		{
			playerVelocity.y = jumpSpeed;
			wishJump = false;
		}
	}

	private void ApplyFriction(float t)
	{
		Vector3 currentPlayerVelocity = playerVelocity;

		currentPlayerVelocity.y = 0.0f;
		var currentSpeed = currentPlayerVelocity.magnitude;
		var control = 0.0f;
		var drop = 0.0f;

		if (characterController.isGrounded)
		{
			control = currentSpeed < runDeacceleration ? runDeacceleration : currentSpeed;
			drop = control * groudFriction * Time.deltaTime * t;
		}

		var newSpeed = currentSpeed - drop;
		playerFriction = newSpeed;
		if (newSpeed < 0)
			newSpeed = 0;
		if (currentSpeed > 0)
			newSpeed /= currentSpeed;

		playerVelocity.x *= newSpeed;
		playerVelocity.z *= newSpeed;
	}

	private void Accelerate(Vector3 wishDirection, float wishSpeed, float acceleration)
	{
		var currentSpeed = Vector3.Dot(playerVelocity, wishDirection);
		var speedToAdd = wishSpeed - currentSpeed;
		if (speedToAdd <= 0)
			return;
		var accelerationSpeed = acceleration * Time.deltaTime * wishSpeed;
		if (accelerationSpeed > speedToAdd)
			accelerationSpeed = speedToAdd;

		playerVelocity.x += accelerationSpeed * wishDirection.x;
		playerVelocity.z += accelerationSpeed * wishDirection.z;
	}
	#endregion

	internal void GoToCrouching()
	{
		isCrouched = true;

		if (PlayerCamera.currentViewYOffset > PlayerCamera.PLAYER_CROUCHING_VIEW_Y_OFFSET)
		{
			PlayerCamera.currentViewYOffset -= changeStanceSpeed * Time.deltaTime;
			if (PlayerCamera.currentViewYOffset < PlayerCamera.PLAYER_CROUCHING_VIEW_Y_OFFSET)
			{
				PlayerCamera.currentViewYOffset = PlayerCamera.PLAYER_CROUCHING_VIEW_Y_OFFSET;
			}

			characterController.height = PlayerCamera.currentViewYOffset;
		}
	}

	internal void GoToStanding()
	{
		isCrouched = false;

		if (PlayerCamera.currentViewYOffset < PlayerCamera.PLAYER_STANDING_VIEW_Y_OFFSET)
		{
			PlayerCamera.currentViewYOffset += changeStanceSpeed * Time.deltaTime;
			if (PlayerCamera.currentViewYOffset > PlayerCamera.PLAYER_STANDING_VIEW_Y_OFFSET)
			{
				PlayerCamera.currentViewYOffset = PlayerCamera.PLAYER_STANDING_VIEW_Y_OFFSET;
			}

			characterController.height = PlayerCamera.currentViewYOffset;
		}
	}

	/*
	============
	PM_CmdScale
	Returns the scale factor to apply to cmd movements
	This allows the clients to use axial -127 to 127 values for all directions
	without getting a sqrt(2) distortion in speed.
	============
	*/
	private float CommandScale()
	{
		var max = (int)Mathf.Abs(playerInputVector.z);
		if (Mathf.Abs(playerInputVector.x) > max)
			max = (int)Mathf.Abs(playerInputVector.x);
		if (max <= 0)
			return 0;

		var total = Mathf.Sqrt(playerInputVector.z * playerInputVector.z + playerInputVector.x * playerInputVector.x);
		var scale = moveSpeed * max / (moveScale * total);

		return scale;
	}
}