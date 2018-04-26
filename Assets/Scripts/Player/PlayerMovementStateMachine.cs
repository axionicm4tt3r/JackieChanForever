using System;
using UnityEngine;

public class PlayerMovementStateMachine : SuperStateMachine
{
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

	public const float JumpingSafeFallHeight = 1.5f * JumpHeight;
	public const float FallingSafeFallHeight = 1.8f * JumpHeight;
	public const float SafeHeightMultiplier = 2f;

	public const float StandingRecoveryTime = 0.15f;
	public const float CrouchRecoveryTime = 0.22f;
	public const float FallingIntoRecoveryTime = 0.35f;

	public const float JumpKickVelocity = 8f;
	public const float SlideVelocity = 10f;
	public const float SlideDuration = 0.6f;
	public const float LungeVelocity = 14f;
	public const float LungeDuration = 0.3f;

	public Vector3 moveDirection;
	public Vector3 lookDirection { get; private set; }

	private SuperCharacterController controller;
	private PlayerCamera playerCamera;
	private PlayerInputManager playerInputManager;
	private PlayerStatus playerStatus;
	private PlayerAttackStateMachine playerAttackStateMachine;

	private float startingHeight = 0f;

	public Enum CurrentState { get { return currentState; } private set { ChangeState(); currentState = value; } }

	public float TimeSinceEnteringCurrentState { get { return Time.time - timeEnteredState; } }

	private void ChangeState()
	{
		lastState = CurrentState;
		timeEnteredState = Time.time;
	}

	public AngleDirection LocalMovementCardinalDirection
	{
		get
		{
			return Helpers.Direction(Vector2.SignedAngle(new Vector2(moveDirection.x, moveDirection.z), new Vector2(transform.forward.x, transform.forward.z)));
		}
	}

	#region PropertyGetters
	public bool LocalMovementIsForwardFacing
	{
		get
		{
			var movement = LocalMovementCardinalDirection;
			return (movement == AngleDirection.Forward) ||
				(movement == AngleDirection.ForwardLeft) ||
				(movement == AngleDirection.ForwardRight);
		}
	}

	public bool InCrouchingState
	{
		get
		{
			return (PlayerMovementState)CurrentState == PlayerMovementState.Crouching ||
		   (PlayerMovementState)CurrentState == PlayerMovementState.CrouchRunning ||
		   (PlayerMovementState)CurrentState == PlayerMovementState.CrouchRecovering ||
		   (PlayerMovementState)CurrentState == PlayerMovementState.Sliding;
		}
	}

	public bool IsRecovering
	{
		get
		{
			return (PlayerMovementState)CurrentState == PlayerMovementState.Recovering ||
		   (PlayerMovementState)CurrentState == PlayerMovementState.CrouchRecovering;
		}
	}
	#endregion

	void Awake()
	{
		controller = gameObject.GetComponent<SuperCharacterController>();
		playerCamera = gameObject.GetComponentInChildren<PlayerCamera>();
		playerInputManager = gameObject.GetComponent<PlayerInputManager>();
		playerStatus = gameObject.GetComponent<PlayerStatus>();
		playerAttackStateMachine = gameObject.GetComponent<PlayerAttackStateMachine>();
		lookDirection = transform.forward;
		CurrentState = PlayerMovementState.Standing;
	}

	protected override void EarlyGlobalSuperUpdate()
	{
		lookDirection = transform.forward;

		//Debug.Log($"CurrentState is {CurrentState}");
		//Debug.Log($"State Time is {TimeSinceEnteringCurrentState}");
	}

	protected override void LateGlobalSuperUpdate()
	{
		if (InCrouchingState)
			GoToCrouching();
		else
			GoToStanding();

		transform.position += moveDirection * Time.deltaTime;

		Debug.Log($"CurrentState is {CurrentState}");
		//Debug.Log($"State Time is {TimeSinceEnteringCurrentState}");

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
		if (!MaintainingGround())
		{
			CurrentState = PlayerMovementState.Falling;
			return;
		}

		if (playerInputManager.Current.JumpInput)
		{
			CurrentState = PlayerMovementState.Jumping;
			return;
		}

		if (playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerMovementState.Crouching;
			return;
		}

		if (playerInputManager.Current.MoveInput != Vector3.zero)
		{
			if (!playerInputManager.Current.CrouchInput)
			{
				CurrentState = PlayerMovementState.Running;
				return;
			}
			else if (playerInputManager.Current.CrouchInput)
			{
				CurrentState = PlayerMovementState.CrouchRunning;
				return;
			}
		}

		moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, RunDeceleration * Time.deltaTime);
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
		if (!MaintainingGround())
		{
			CurrentState = PlayerMovementState.Falling;
			return;
		}

		if (playerInputManager.Current.JumpInput)
		{
			CurrentState = PlayerMovementState.Jumping;
			return;
		}

		if (!playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerMovementState.Standing;
			return;
		}

		if (playerInputManager.Current.MoveInput != Vector3.zero)
		{
			if (!playerInputManager.Current.CrouchInput)
			{
				CurrentState = PlayerMovementState.Running;
				return;
			}
			else if (playerInputManager.Current.CrouchInput)
			{
				CurrentState = PlayerMovementState.CrouchRunning;
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
			CurrentState = PlayerMovementState.Jumping;
			return;
		}

		if (!MaintainingGround())
		{
			CurrentState = PlayerMovementState.Falling;
			return;
		}

		if (playerInputManager.Current.MoveInput != Vector3.zero && !playerInputManager.Current.CrouchInput)
		{
			moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * RunSpeed, RunAcceleration * Time.deltaTime);
		}
		else if (playerInputManager.Current.MoveInput != Vector3.zero && playerInputManager.Current.CrouchInput)
		{
			if (LocalMovementIsForwardFacing)
			{
				CurrentState = PlayerMovementState.Sliding;
				return;
			}
			else
			{
				CurrentState = PlayerMovementState.CrouchRunning;
				return;
			}
		}
		else if (playerInputManager.Current.MoveInput == Vector3.zero && playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerMovementState.Crouching;
			return;
		}
		else if (playerInputManager.Current.MoveInput == Vector3.zero && !playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerMovementState.Standing;
			return;
		}
	}
	#endregion

	#region CrouchRunning
	void CrouchRunning_SuperUpdate()
	{
		if (playerInputManager.Current.JumpInput)
		{
			CurrentState = PlayerMovementState.Jumping;
			return;
		}

		if (!MaintainingGround())
		{
			CurrentState = PlayerMovementState.Falling;
			return;
		}

		if (playerInputManager.Current.MoveInput != Vector3.zero && playerInputManager.Current.CrouchInput)
		{
			moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * CrouchSpeed, CrouchAcceleration * Time.deltaTime);
		}
		else if (playerInputManager.Current.MoveInput != Vector3.zero && !playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerMovementState.Running;
			return;
		}
		else if (playerInputManager.Current.MoveInput == Vector3.zero && playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerMovementState.Crouching;
			return;
		}
		else if (playerInputManager.Current.MoveInput == Vector3.zero && !playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerMovementState.Standing;
			return;
		}
	}
	#endregion

	#region Sliding
	void Sliding_SuperUpdate()
	{
		if (!MaintainingGround())
		{
			CurrentState = PlayerMovementState.Falling;
			return;
		}

		if (TimeSinceEnteringCurrentState >= SlideDuration)
		{
			if ((PlayerAttackState)playerAttackStateMachine.CurrentState == PlayerAttackState.SlideKicking)
			{
				CurrentState = PlayerMovementState.CrouchRecovering;
				return;
			}
			else
			{
				CurrentState = PlayerMovementState.Crouching;
				return;
			}

		}

		moveDirection = Vector3.MoveTowards(moveDirection, transform.forward * SlideVelocity, SlideVelocity * Time.deltaTime);
	}
	#endregion

	#region Lunging
	void Lunging_EnterState()
	{
		controller.DisableClamping();
		controller.DisableSlopeLimit();

		if (MaintainingGround())
			moveDirection += (transform.forward * LungeVelocity * playerAttackStateMachine.AttackChargePercentage);
	}

	void Lunging_SuperUpdate()
	{
		if (!MaintainingGround())
		{
			CurrentState = PlayerMovementState.Falling;
			return;
		}

		if (TimeSinceEnteringCurrentState >= (LungeDuration * playerAttackStateMachine.AttackChargePercentage))
		{
			//Debug.Log($"Player shouldn't recover from attack. Charge rate is {playerAttackStateMachine.AttackChargePercentage}");
			CurrentState = PlayerMovementState.Recovering;
			return;
		}
		
		moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, RunDeceleration * Time.deltaTime);
	}
	#endregion

	#region Recovering
	void Recovering_SuperUpdate()
	{
		if (!MaintainingGround())
		{
			CurrentState = PlayerMovementState.Falling;
			return;
		}

		bool recoverFromFalling = ((PlayerMovementState)lastState == PlayerMovementState.Falling || (PlayerMovementState)lastState == PlayerMovementState.Jumping);

		if ((recoverFromFalling && TimeSinceEnteringCurrentState >= FallingIntoRecoveryTime) || 
			(!recoverFromFalling && TimeSinceEnteringCurrentState >= StandingRecoveryTime))
		{
			CurrentState = PlayerMovementState.Standing;
			return;
		}

		moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, RunDeceleration * Time.deltaTime);
	}
	#endregion

	#region CrouchRecovering
	void CrouchRecovering_SuperUpdate()
	{
		if (!MaintainingGround())
		{
			CurrentState = PlayerMovementState.Falling;
			return;
		}

		bool recoverFromFalling = ((PlayerMovementState)lastState == PlayerMovementState.Falling || (PlayerMovementState)lastState == PlayerMovementState.Jumping);

		if ((recoverFromFalling && TimeSinceEnteringCurrentState >= FallingIntoRecoveryTime) ||
			(!recoverFromFalling && TimeSinceEnteringCurrentState >= CrouchRecoveryTime)) //Maybe handle a getting up into stand recovery state
		{
			CurrentState = PlayerMovementState.Crouching;
			return;
		}

		moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, CrouchDeceleration * Time.deltaTime);
	}
	#endregion

	#region Jumping
	void Jumping_EnterState()
	{
		controller.DisableClamping();
		controller.DisableSlopeLimit();

		startingHeight = transform.position.y;
		moveDirection += controller.up * CalculateJumpSpeed(JumpHeight, Gravity);
	}

	void Jumping_SuperUpdate()
	{
		Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
		Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

		if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
		{
			moveDirection = planarMoveDirection;
			var fallenHeight = startingHeight - transform.position.y;

			if (fallenHeight >= SafeHeightMultiplier * JumpingSafeFallHeight)
			{
				CurrentState = PlayerMovementState.CrouchRecovering;
				return;
			}
			else if (fallenHeight >= JumpingSafeFallHeight)
			{
				CurrentState = PlayerMovementState.Recovering;
				return;
			}
			else
			{
				CurrentState = PlayerMovementState.Standing;
				return;
			}
		}

		if ((PlayerAttackState)playerAttackStateMachine.CurrentState == PlayerAttackState.JumpKicking)
			planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, playerCamera.AimDirectionVector * JumpKickVelocity, JumpKickVelocity * Time.deltaTime);
		else
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

		startingHeight = transform.position.y;
	}

	void Falling_SuperUpdate()
	{
		if (AcquiringGround())
		{
			moveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
			var fallenHeight = startingHeight - transform.position.y;

			if (fallenHeight >= SafeHeightMultiplier * FallingSafeFallHeight)
			{
				CurrentState = PlayerMovementState.CrouchRecovering;
				return;
			}
			else if (fallenHeight >= FallingSafeFallHeight)
			{
				CurrentState = PlayerMovementState.Recovering;
				return;
			}
			else
			{
				CurrentState = PlayerMovementState.Standing;
				return;
			}
		}

		Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
		planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * RunSpeed, AirControl * RunAcceleration * Time.deltaTime);
		moveDirection -= controller.up * Gravity * Time.deltaTime;
	}
	#endregion

	#endregion

	public void Lunge()
	{
		CurrentState = PlayerMovementState.Lunging;
	}

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

public enum PlayerMovementState
{
	Standing,
	Crouching,
	Running,
	CrouchRunning,
	Sliding,
	Lunging,
	Recovering,
	CrouchRecovering,
	Jumping,
	Falling
}