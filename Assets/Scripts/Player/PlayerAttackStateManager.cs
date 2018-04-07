using UnityEngine;

public class PlayerAttackStateManager : MonoBehaviour, IAttackable
{
	public const float JUMP_KICK_ALLOWANCE_TIME = 0.2f;
	public const float SLIDE_KICK_ALLOWANCE_TIME = 0.2f;

	public GameObject PlayerHUD;

	private PlayerStatus playerStatus;
	private PlayerAnimationManager playerAnimationManager;
	private PlayerAttackManager playerAttackManager;
	private PlayerInputManager playerInputManager;
	private PlayerInteractionManager playerInteractionManager;
	private PlayerMovementStateMachine playerStateMachine;

	[ReadOnly]
	public PlayerAttackState attackState;

	public float maximumAttackChargeTime = 3f;
	public float maximumBlockTime = 0.5f;
	public float basicAttackCooldown = 0.3f;
	public float jumpKickAttackCooldown = 0.5f;
	public float jumpKickAttackMotionTime = 0.4f;

	public float slideKickAttackCooldown = 0.5f;
	public float slideKickAttackMotionTime = 0.6f;

	private float knockbackRecoveryFraction = 3f;
	private float currentKnockbackRecoveryTime = 0f;

	private float staggerKnockbackVelocity = 2f;
	private float currentStaggerRecoveryTime = 0.1f;

	private float attackCooldown = 0f;
	private float attackMotionTime = 0f;
	private float blockTime = 0f;
	private bool blockInputHandled = false;

	void Awake()
	{
		if (!GameObject.FindGameObjectWithTag("PlayerHUD"))
			PlayerHUD = Instantiate(PlayerHUD) as GameObject;

		playerStatus = GetComponent<PlayerStatus>();
		playerAnimationManager = GetComponent<PlayerAnimationManager>();
		playerAttackManager = GetComponent<PlayerAttackManager>();
		playerInputManager = GetComponent<PlayerInputManager>();
		playerInteractionManager = GetComponent<PlayerInteractionManager>();
		playerStateMachine = GetComponent<PlayerMovementStateMachine>();
	}

	void Update()
	{
		if (attackCooldown > 0)
			attackCooldown -= Time.deltaTime;

		if (attackMotionTime > 0)
			attackMotionTime -= Time.deltaTime;

		if (blockTime > 0)
			blockTime -= Time.deltaTime;

		if (attackMotionTime <= 0 && !(attackState == PlayerAttackState.Idle || attackState == PlayerAttackState.Blocking))
			ResetAttackStateToIdle();
		
		if (blockTime <= 0 && attackState == PlayerAttackState.Blocking)
			ResetBlockStateToIdle();
	}

	void LateUpdate()
	{
		if (playerInputManager.Current.PrimaryFireInput)
			Attack();

		if (playerInputManager.Current.SecondaryFireInput)
			Block();
		else
			blockInputHandled = false;

		if (playerInputManager.Current.InteractInput)
			Interact();
	}

	public void ReceiveAttack(float damage)
	{
		playerStatus.TakeDamage(damage);
	}

	public void ReceiveStaggerAttack(float damage, Vector3 staggerDirection, float staggerRecoveryTime)
	{
		playerStatus.BecomeStaggered();

		currentStaggerRecoveryTime = staggerRecoveryTime;
		//animator.SetBool("Staggered", true);
		playerStateMachine.moveDirection += staggerDirection * staggerKnockbackVelocity;

		playerStatus.TakeDamage(damage);
	}

	public void ReceiveKnockbackAttack(float damage, Vector3 knockbackDirection, float knockbackVelocity, float knockbackTime)
	{
		playerStatus.BecomeKnockedBack();

		currentKnockbackRecoveryTime = knockbackTime;
		//animator.SetBool("KnockedBack", true);
		playerStateMachine.moveDirection += knockbackDirection * knockbackVelocity;

		playerStatus.TakeDamage(damage);
	}

	internal void Interact()
	{
		playerInteractionManager.Interact();
	}

	private void Block()
	{
		if (attackState == PlayerAttackState.Idle && !blockInputHandled)
		{
			attackState = PlayerAttackState.Blocking;
			blockInputHandled = true;
			blockTime = maximumBlockTime;
			playerAnimationManager.Block();
		}
	}

	internal void Attack()
	{
		if (playerInteractionManager.Grabbing)
		{
			playerInteractionManager.Throw();
		}
		else if (attackCooldown <= 0)
		{
			if (playerStateMachine.IsInState(PlayerStates.CrouchRunning) 
				&& playerStateMachine.TimeSinceEnteringCurrentState < SLIDE_KICK_ALLOWANCE_TIME 
				&& playerStateMachine.LocalMovementCardinalDirection == AngleDirection.Forward)
				PerformSlideKickAttack();

			else if (playerStateMachine.IsInState(PlayerStates.Jumping) 
				&& playerStateMachine.TimeSinceEnteringCurrentState < JUMP_KICK_ALLOWANCE_TIME
				&& playerStateMachine.LocalMovementCardinalDirection == AngleDirection.Forward)
				PerformJumpKickAttack();
			else
				PerformBasicAttack();
		}
	}

	private void PerformSlideKickAttack()
	{
		//Debug.Log("SlideKick!");
		attackState = PlayerAttackState.SlideKicking;
		attackMotionTime = slideKickAttackMotionTime;
		attackCooldown = slideKickAttackCooldown;

		playerAnimationManager.SlideKick();
	}

	private void PerformJumpKickAttack()
	{
		//Debug.Log("JumpKick!");
		attackState = PlayerAttackState.JumpKicking;
		attackMotionTime = jumpKickAttackMotionTime;
		attackCooldown = jumpKickAttackCooldown;

		playerAnimationManager.JumpKick();
	}

	private void PerformBasicAttack()
	{
		//Debug.Log("BasicAttack!");
		playerAttackManager.BasicAttack(); //Instant frame attack, does calculations in 1 frame
		attackCooldown = basicAttackCooldown;

		playerAnimationManager.BasicAttack();
	}

	private void ResetAttackStateToIdle()
	{
		attackState = PlayerAttackState.Idle;
		attackMotionTime = 0;
		playerAttackManager.ClearEnemiesHit();
		playerAnimationManager.ResetAnimatorParameters();
	}

	private void ResetBlockStateToIdle()
	{
		attackState = PlayerAttackState.Idle;
		blockTime = 0;
		playerAnimationManager.ResetAnimatorParameters();
	}
}