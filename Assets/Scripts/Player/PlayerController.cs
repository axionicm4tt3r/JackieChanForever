using UnityEngine;

public class PlayerController : MonoBehaviour, IAttackable
{
	public const float JUMP_KICK_ALLOWANCE_TIME = 0.2f;
	public const float SLIDE_KICK_ALLOWANCE_TIME = 0.2f;

	public GameObject PlayerHUD;

	private Animator playerUIAnimator;
	private PlayerStatus playerStatus;
	private PlayerInputManager playerInputManager;
	private PlayerStateMachine playerStateMachine;
	private PlayerAttackManager playerAttackManager;
	private PlayerInteractionManager playerInteractionManager;

	[ReadOnly]
	public AttackState attackState;

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

	void Awake()
	{
		if (!GameObject.FindGameObjectWithTag("PlayerHUD"))
		{
			PlayerHUD = Instantiate(PlayerHUD) as GameObject;
		}

		playerUIAnimator = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerHUD).GetComponentInChildren<Animator>();
		playerStatus = GetComponent<PlayerStatus>();
		playerInputManager = GetComponent<PlayerInputManager>();
		playerStateMachine = GetComponent<PlayerStateMachine>();
		playerAttackManager = GetComponent<PlayerAttackManager>();
		playerInteractionManager = GetComponent<PlayerInteractionManager>();
	}

	void Update()
	{
		if (attackCooldown > 0)
			attackCooldown -= Time.deltaTime;

		if (attackMotionTime <= 0 && attackState != AttackState.Idle)
		{
			attackState = AttackState.Idle;
			attackMotionTime = 0;
			playerAttackManager.ClearEnemiesHit();
			ResetAnimatorParameters();
		}
		else
			attackMotionTime -= Time.deltaTime;
	}

	void LateUpdate()
	{
		if (playerInputManager.Current.PrimaryFireInput)
			Attack();

		if (playerInputManager.Current.InteractInput)
			Interact();
	}

	private void ResetAnimatorParameters()
	{
		playerUIAnimator.SetBool("JumpKicking", false);
		playerUIAnimator.SetBool("SlideKicking", false);
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

	internal void Attack()
	{
		if (playerInteractionManager.Grabbing)
		{
			playerInteractionManager.Throw();
		}
		else if (attackCooldown <= 0)
		{
			if ((PlayerStates)playerStateMachine.CurrentState == PlayerStates.CrouchRunning && playerStateMachine.TimeSinceEnteringCurrentState < SLIDE_KICK_ALLOWANCE_TIME)
				PerformSlideKickAttack();
			else if ((PlayerStates)playerStateMachine.CurrentState == PlayerStates.Jumping && playerStateMachine.TimeSinceEnteringCurrentState < JUMP_KICK_ALLOWANCE_TIME)
				PerformJumpKickAttack();
			else
				PerformBasicAttack();
		}
	}

	private void PerformSlideKickAttack()
	{
		Debug.Log("SlideKick!");
		attackState = AttackState.SlideKicking; //State allows the triggers in the hitbox
		attackMotionTime = slideKickAttackMotionTime;

		attackCooldown = slideKickAttackCooldown;
		playerUIAnimator.SetBool("SlideKicking", true);
	}

	private void PerformJumpKickAttack()
	{
		Debug.Log("JumpKick!");
		attackState = AttackState.JumpKicking; //State allows the triggers in the hitbox
		attackMotionTime = jumpKickAttackMotionTime;

		attackCooldown = jumpKickAttackCooldown;
		playerUIAnimator.SetBool("JumpKicking", true);
	}

	private void PerformBasicAttack()
	{
		Debug.Log("BasicAttack!");
		playerAttackManager.BasicAttack(); //Instant frame attack, does calculations in 1 frame

		attackCooldown = basicAttackCooldown;
		playerUIAnimator.SetInteger("BasicAttackIndex", UnityEngine.Random.Range(0, 2));
		playerUIAnimator.SetTrigger("BasicAttacking");
	}
}

public enum AttackState
{
	Idle,
	JumpKicking,
	SlideKicking
}
