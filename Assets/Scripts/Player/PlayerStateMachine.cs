using System;
using UnityEngine;

public class PlayerStateMachine : SuperStateMachine
{
	//public float jumpKickVelocity = 18f;
	//public float slideKickVelocity = 14f;

	//private void Update()
	//{
	//	switch (playerController.playerState)
	//	{
	//		case PlayerController.PlayerState.FreeMove:
	//			QueueJump();

	//			if (MaintainingGround)
	//			{ GroundMove(); Debug.Log("GroundMoving"); }
	//			else if (!MaintainingGround)
	//			{ AirMove(); Debug.Log("AirMoving"); }

	//			characterController.transform.Translate(playerVelocity * Time.deltaTime);

	//			Vector3 udp = playerVelocity;
	//			udp.y = 0.0f;
	//			if (playerVelocity.magnitude > playerTopVelocity)
	//				playerTopVelocity = playerVelocity.magnitude;
	//			break;
	//		case PlayerController.PlayerState.JumpKicking:
	//			playerVelocity = transform.forward * jumpKickVelocity;
	//			characterController.transform.Translate(playerVelocity * Time.deltaTime);
	//			break;
	//		case PlayerController.PlayerState.SlideKicking:
	//			playerVelocity = transform.forward * slideKickVelocity;
	//			characterController.transform.Translate(playerVelocity * Time.deltaTime);
	//			break;
	//	}
	//}

	//private void QueueJump()
	//{
	//	if (Input.GetButtonDown("Jump") && !wishJump)
	//		wishJump = true;
	//	if (Input.GetButtonUp("Jump"))
	//		wishJump = false;
	//}

	public const float RunSpeed = 8.0f;
	public const float RunAcceleration = RunSpeed * 9.0f;
	public const float RunDeceleration = RunSpeed * 8.0f;
	public const float CrouchSpeed = 5.0f;
	public const float CrouchAcceleration = CrouchSpeed * 9.0f;
	public const float CrouchDeceleration = CrouchSpeed * 8.0f;
	public const float JumpHeight = 1.3f;
	public const float Gravity = 25.0f;
	public const float AirControl = 0.6f;
	public const float ChangeStanceSpeed = 0.3f;

	private SuperCharacterController controller;

	public Vector3 moveDirection;
	public Vector3 lookDirection { get; private set; }

	private PlayerInputManager playerInputManager;
	private GameObject playerCamera;

	public Enum CurrentState { get { return currentState; } private set { currentState = value; ChangeState(); } }

	public float TimeSinceEnteringCurrentState { get { return Time.time - timeEnteredState; } }

	private void ChangeState()
	{
		lastState = state.currentState;
		timeEnteredState = Time.time;
	}

	void Awake()
	{
		playerInputManager = gameObject.GetComponent<PlayerInputManager>();
		controller = gameObject.GetComponent<SuperCharacterController>();
		playerCamera = GameObject.FindWithTag(Helpers.Tags.PlayerCamera);
		lookDirection = transform.forward;
		currentState = PlayerStates.Standing;
	}

	protected override void EarlyGlobalSuperUpdate()
	{
		lookDirection = transform.forward;
	}

	protected override void LateGlobalSuperUpdate()
	{
		if (((PlayerStates)CurrentState == PlayerStates.Crouching || (PlayerStates)CurrentState == PlayerStates.CrouchRunning) && playerInputManager.Current.CrouchInput)
			GoToCrouching();
		else
			GoToStanding();

		transform.position += moveDirection * Time.deltaTime;
	}

	public bool AcquiringGround()
	{
		return controller.currentGround.IsGrounded(false, 0.01f);
	}

	public bool MaintainingGround()
	{
		return controller.currentGround.IsGrounded(true, 0.5f);
	}

	public void RotateGravity(Vector3 up)
	{
		lookDirection = Quaternion.FromToRotation(transform.up, up) * lookDirection;
	}

	private Vector3 LocalMovement()
	{
		Vector3 right = Vector3.Cross(controller.up, lookDirection);

		Vector3 local = Vector3.zero;

		if (playerInputManager.Current.MoveInput.x != 0)
		{
			local += right * playerInputManager.Current.MoveInput.x;
		}

		if (playerInputManager.Current.MoveInput.z != 0)
		{
			local += lookDirection * playerInputManager.Current.MoveInput.z;
		}

		return local.normalized;
	}

	private float CalculateJumpSpeed(float jumpHeight, float gravity)
	{
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

	#region Movement

	#region Standing
	void Standing_EnterState()
	{
		controller.EnableSlopeLimit();
		controller.EnableClamping();
	}

	void Standing_SuperUpdate()
	{
		if (playerInputManager.Current.JumpInput)
		{
			CurrentState = PlayerStates.Jumping;
			return;
		}

		if (playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerStates.Crouching;
			return;
		}

		if (!MaintainingGround())
		{
			CurrentState = PlayerStates.Falling;
			return;
		}

		if (playerInputManager.Current.MoveInput != Vector3.zero)
		{
			if (!playerInputManager.Current.CrouchInput)
			{
				CurrentState = PlayerStates.Running;
				return;
			}
			else if (playerInputManager.Current.CrouchInput)
			{
				CurrentState = PlayerStates.CrouchRunning;
				return;
			}
		}

		moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, RunDeceleration * Time.deltaTime);
	}

	void Standing_ExitState()
	{

	}
	#endregion

	#region Crouching
	void Crouching_EnterState()
	{
		controller.EnableSlopeLimit();
		controller.EnableClamping();
	}

	void Crouching_SuperUpdate()
	{
		if (playerInputManager.Current.JumpInput)
		{
			CurrentState = PlayerStates.Jumping;
			return;
		}

		if (!playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerStates.Standing;
			return;
		}

		if (!MaintainingGround())
		{
			CurrentState = PlayerStates.Falling;
			return;
		}

		if (playerInputManager.Current.MoveInput != Vector3.zero)
		{
			if (!playerInputManager.Current.CrouchInput)
			{
				CurrentState = PlayerStates.Running;
				return;
			}
			else if (playerInputManager.Current.CrouchInput)
			{
				CurrentState = PlayerStates.CrouchRunning;
				return;
			}
		}

		moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, CrouchDeceleration * Time.deltaTime);
	}

	void Crouching_ExitState()
	{

	}
	#endregion

	#region Running
	void Running_SuperUpdate()
	{
		if (playerInputManager.Current.JumpInput)
		{
			CurrentState = PlayerStates.Jumping;
			return;
		}

		if (!MaintainingGround())
		{
			CurrentState = PlayerStates.Falling;
			return;
		}

		if (playerInputManager.Current.MoveInput != Vector3.zero && !playerInputManager.Current.CrouchInput)
		{
			moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * RunSpeed, RunAcceleration * Time.deltaTime);
		}
		else if (playerInputManager.Current.MoveInput != Vector3.zero && playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerStates.CrouchRunning;
			return;
		}
		else if (playerInputManager.Current.MoveInput == Vector3.zero && playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerStates.Crouching;
			return;
		}
		else if (playerInputManager.Current.MoveInput == Vector3.zero && !playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerStates.Standing;
			return;
		}
	}
	#endregion

	#region CrouchRunning
	void CrouchRunning_SuperUpdate()
	{
		if (playerInputManager.Current.JumpInput)
		{
			CurrentState = PlayerStates.Jumping;
			return;
		}

		if (!MaintainingGround())
		{
			CurrentState = PlayerStates.Falling;
			return;
		}

		if (playerInputManager.Current.MoveInput != Vector3.zero && playerInputManager.Current.CrouchInput)
		{
			moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * CrouchSpeed, CrouchAcceleration * Time.deltaTime);
		}
		else if (playerInputManager.Current.MoveInput != Vector3.zero && !playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerStates.Running;
			return;
		}
		else if (playerInputManager.Current.MoveInput == Vector3.zero && playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerStates.Crouching;
			return;
		}
		else if (playerInputManager.Current.MoveInput == Vector3.zero && !playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerStates.Standing;
			return;
		}
	}
	#endregion

	#region Jumping
	void Jumping_EnterState()
	{
		controller.DisableClamping();
		controller.DisableSlopeLimit();

		moveDirection += controller.up * CalculateJumpSpeed(JumpHeight, Gravity);
	}

	void Jumping_SuperUpdate()
	{
		Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
		Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

		if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
		{
			moveDirection = planarMoveDirection;
			CurrentState = PlayerStates.Standing;
			return;
		}

		planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * RunSpeed, AirControl * RunAcceleration * Time.deltaTime);
		verticalMoveDirection -= controller.up * Gravity * Time.deltaTime;

		moveDirection = planarMoveDirection + verticalMoveDirection;
	}
	#endregion

	#region Falling
	void Falling_EnterState()
	{
		controller.DisableClamping();
		controller.DisableSlopeLimit();
	}

	void Falling_SuperUpdate()
	{
		if (AcquiringGround())
		{
			moveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
			CurrentState = PlayerStates.Standing;
			return;
		}

		Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
		planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * RunSpeed, AirControl * RunAcceleration * Time.deltaTime);
		moveDirection -= controller.up * Gravity * Time.deltaTime;
	}
	#endregion

	#endregion

	private void GoToCrouching()
	{
		PlayerCamera.currentViewYOffset = Mathf.Lerp(PlayerCamera.currentViewYOffset, PlayerCamera.PLAYER_CROUCHING_VIEW_Y_OFFSET,
			(Time.time - timeEnteredState) / ChangeStanceSpeed);

		controller.heightScale = PlayerCamera.currentViewYOffset / controller.height;
	}

	private void GoToStanding()
	{
		PlayerCamera.currentViewYOffset = Mathf.Lerp(PlayerCamera.currentViewYOffset, PlayerCamera.PLAYER_STANDING_VIEW_Y_OFFSET,
			(Time.time - timeEnteredState) / ChangeStanceSpeed);

		controller.heightScale = PlayerCamera.currentViewYOffset / controller.height;
	}
}

public enum PlayerStates { Standing, Crouching, Running, CrouchRunning, Jumping, Falling }