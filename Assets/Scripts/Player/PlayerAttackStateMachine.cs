using System;
using UnityEngine;

public class PlayerAttackStateMachine : SuperStateMachine
{
	public const float JUMP_KICK_ALLOWANCE_TIME = 0.2f;
	public const float SLIDE_KICK_ALLOWANCE_TIME = 0.2f;

	private PlayerStatus playerStatus;
	private PlayerAnimationManager playerAnimationManager;
	private PlayerAttackManager playerAttackManager;
	private PlayerInputManager playerInputManager;
	private PlayerInteractionManager playerInteractionManager;
	private PlayerMovementStateMachine playerMovementStateMachine;

	public float maximumAttackChargeTime = 3f;
	public float chargeAttackDamageModifier = 2.5f;

	public float blockMotionTime = 0.5f;
	public float basicAttackMotionTime = 0.3f;
	public float jumpKickAttackCooldown = 0.5f;
	public float jumpKickAttackMotionTime = 0.4f;

	public float slideKickAttackCooldown = 0.5f;
	public float slideKickAttackMotionTime = 0.6f;

	private float attackCooldown = 0f;
	private float attackMotionTime = 0f;
	private float blockTime = 0f;
	private bool blockInputHandled = false;

	public Enum CurrentState { get { return currentState; } private set { ChangeState(); currentState = value; } }

	public float TimeSinceEnteringCurrentState { get { return Time.time - timeEnteredState; } }

	public bool IsInState(PlayerAttackState state) { return (PlayerAttackState)CurrentState == state; }

	private void ChangeState()
	{
		lastState = state.currentState;
		timeEnteredState = Time.time;
	}

	void Awake()
	{
		playerStatus = GetComponent<PlayerStatus>();
		playerAnimationManager = GetComponent<PlayerAnimationManager>();
		playerAttackManager = GetComponent<PlayerAttackManager>();
		playerInputManager = GetComponent<PlayerInputManager>();
		playerInteractionManager = GetComponent<PlayerInteractionManager>();
		playerMovementStateMachine = GetComponent<PlayerMovementStateMachine>();
		CurrentState = PlayerAttackState.Idle;
	}

	protected override void EarlyGlobalSuperUpdate()
	{

	}

	protected override void LateGlobalSuperUpdate()
	{
		if (Input.GetButtonUp(InputCodes.SecondaryFire))
			blockInputHandled = false;

		//Debug.Log($"Time in state: {TimeSinceEnteringCurrentState}");
	}

	#region AttackStates

	#region Idle
	void Idle_EnterState()
	{
		playerAttackManager.ClearEnemiesHit();
		playerAnimationManager.ResetAnimatorParameters();
	}

	void Idle_SuperUpdate()
	{
		if (playerInputManager.Current.PrimaryFireInput)
		{
			Attack();
			return;
		}

		if (playerInputManager.Current.SecondaryFireInput && !blockInputHandled)
		{
			CurrentState = PlayerAttackState.Blocking;
			return;
		}

		if (playerInputManager.Current.InteractInput)
		{
			playerInteractionManager.Interact();
			return;
		}
	}
	#endregion

	#region Blocking
	void Blocking_EnterState()
	{
		blockInputHandled = true;
		playerAnimationManager.Block();
	}

	void Blocking_SuperUpdate()
	{
		if (TimeSinceEnteringCurrentState >= blockMotionTime)
		{
			CurrentState = PlayerAttackState.Idle;
			return;
		}
	}

	#endregion

	#region Charging
	void Charging_EnterState()
	{
		playerAnimationManager.ChargeAttack();
	}

	void Charging_SuperUpdate()
	{
		if (!playerInputManager.Current.PrimaryFireInput || TimeSinceEnteringCurrentState >= maximumAttackChargeTime)
		{
			CurrentState = PlayerAttackState.BasicAttacking;
			return;
		}

		//Charge up the attack
	}
	#endregion

	#region BasicAttacking
	void BasicAttacking_EnterState()
	{
		playerAttackManager.BasicAttack();
		playerAnimationManager.BasicAttack();
	}

	void BasicAttacking_SuperUpdate()
	{
		if (TimeSinceEnteringCurrentState >= basicAttackMotionTime)
		{
			CurrentState = PlayerAttackState.Idle;
			return;
		}

	}
	#endregion

	#region JumpKicking
	void JumpKicking_EnterState()
	{
		playerAnimationManager.JumpKick();
	}

	void JumpKicking_SuperUpdate()
	{
		if (TimeSinceEnteringCurrentState >= jumpKickAttackMotionTime)
		{
			CurrentState = PlayerAttackState.Idle;
			return;
		}
	}
	#endregion

	#region SlideKicking
	void SlideKicking_EnterState()
	{
		playerAnimationManager.SlideKick();
	}

	void SlideKicking_SuperUpdate()
	{
		if (TimeSinceEnteringCurrentState >= slideKickAttackMotionTime || (PlayerMovementState)playerMovementStateMachine.CurrentState != PlayerMovementState.Sliding)
		{
			CurrentState = PlayerAttackState.Idle;
			return;
		}
	}
	#endregion

	#endregion

	internal void Attack()
	{
		if (playerInteractionManager.HoldingObject)
		{
			playerInteractionManager.Throw();
			//Animate player throw?
		}
		else
		{
			if ((PlayerMovementState)playerMovementStateMachine.CurrentState == PlayerMovementState.Sliding)
			{
				CurrentState = PlayerAttackState.SlideKicking;
				return;
			}
			else if ((PlayerMovementState)playerMovementStateMachine.CurrentState == PlayerMovementState.Jumping
				&& playerMovementStateMachine.TimeSinceEnteringCurrentState < JUMP_KICK_ALLOWANCE_TIME
				&& playerMovementStateMachine.LocalMovementIsForwardFacing)
			{
				CurrentState = PlayerAttackState.JumpKicking;
				return;
			}
			else
			{
				CurrentState = PlayerAttackState.BasicAttacking;
				return;
			}
		}
	}
}

public enum PlayerAttackState
{
	Idle = 1,
	Blocking = 2,
	Charging = 3,
	BasicAttacking = 4,
	JumpKicking = 5,
	SlideKicking = 6
}