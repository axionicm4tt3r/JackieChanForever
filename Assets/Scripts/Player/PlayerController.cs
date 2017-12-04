using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public const float JUMP_KICK_ALLOWANCE_TIME = 0.2f;
	public const float SLIDE_KICK_ALLOWANCE_TIME = 0.2f;

	public static float BasicAttackHitDamage = 5f;
	public static float JumpKickHitDamage = 18f;
	public static float JumpKickHitKnockbackVelocity = 25f;
	public static float SlideKickHitDamage = 12f;
	public static float SlideKickHitKnockbackVelocity = 18f;

	public GameObject PlayerHUD;
	public BoxCollider MidPunchHitbox;
	public BoxCollider JumpKickHitbox;
	public BoxCollider SlideKickHitbox;

	private Animator playerUIAnimator;
	private PlayerSoundManager playerSoundManager;
	private PlayerInput playerInput;
	private PlayerMovement playerMovement;

	public PlayerState playerState;

	public float basicAttackCooldown = 0.3f;
	public float jumpKickAttackCooldown = 0.5f;
	public float jumpKickAttackMotionTime = 0.4f;
	public float slideKickAttackCooldown = 0.5f;
	public float slideKickAttackMotionTime = 0.6f;

	private float attackCooldown = 0f;
	private float attackMotionTime = 0f;

	void Start()
	{
		if (!GameObject.FindGameObjectWithTag("PlayerHUD"))
		{
			Instantiate(PlayerHUD);
		}

		playerUIAnimator = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerHUD).GetComponentInChildren<Animator>();
		playerSoundManager = GetComponent<PlayerSoundManager>();
		playerInput = GetComponent<PlayerInput>();
		playerMovement = GetComponent<PlayerMovement>();

		MidPunchHitbox = GameObject.FindGameObjectWithTag(Helpers.Tags.MidPunchHitbox).GetComponent<BoxCollider>();
		JumpKickHitbox = GameObject.FindGameObjectWithTag(Helpers.Tags.JumpKickHitbox).GetComponent<BoxCollider>();
		SlideKickHitbox = GameObject.FindGameObjectWithTag(Helpers.Tags.SlideKickHitbox).GetComponent<BoxCollider>();
	}

	void LateUpdate()
	{
		if (attackCooldown > 0)
			attackCooldown -= Time.deltaTime;

		if (attackMotionTime <= 0)
		{
			playerState = PlayerState.FreeMove;
			attackMotionTime = 0;
			ResetAnimatorParameters();
		}
		else
			attackMotionTime -= Time.deltaTime;
	}

	private void ResetAnimatorParameters()
	{
		playerUIAnimator.SetBool("BasicAttacking", false);
		playerUIAnimator.SetBool("JumpKicking", false);
		playerUIAnimator.SetBool("SlideKicking", false);
	}

	internal void Attack()
	{
		if (attackCooldown <= 0)
		{
			if (playerInput.crouchTime > 0.03 && playerInput.crouchTime < SLIDE_KICK_ALLOWANCE_TIME)
				PerformSlideKickAttack();

			else if (playerInput.jumpTime > 0.03 && playerInput.jumpTime < JUMP_KICK_ALLOWANCE_TIME)
				PerformJumpKickAttack();

			else
				PerformBasicAttack();
		}
	}

	private void PerformSlideKickAttack()
	{
		Debug.Log("SlideKick!");
		playerState = PlayerState.SlideKicking;
		attackCooldown = slideKickAttackCooldown;
		attackMotionTime = slideKickAttackMotionTime;
		playerUIAnimator.SetBool("SlideKicking", true);
	}

	private void PerformJumpKickAttack()
	{
		Debug.Log("JumpKick!");
		playerState = PlayerState.JumpKicking;
		attackCooldown = jumpKickAttackCooldown;
		attackMotionTime = jumpKickAttackMotionTime;
		playerUIAnimator.SetBool("JumpKicking", true);
	}

	private void PerformBasicAttack()
	{
		Debug.Log("BasicAttack!");
		bool hitEnemy = false;

		Collider[] results = CheckHitboxForEnemies(MidPunchHitbox);

		foreach (Collider collider in results)
		{
			if (collider.gameObject.tag == Helpers.Tags.Enemy || collider.gameObject.tag == Helpers.Tags.Breakable)
			{
				hitEnemy = true;

				var enemyAI = collider.gameObject.GetComponent<EnemyAI>();
				enemyAI.ApplyStagger();
				enemyAI.status.TakeDamage(BasicAttackHitDamage);
			}
		}

		if (hitEnemy)
			playerSoundManager.PlayBasicAttackHitSound();
		else
			playerSoundManager.PlayBasicAttackMissSound();

		attackCooldown = basicAttackCooldown;
		playerUIAnimator.SetBool("BasicAttacking", true);
		playerUIAnimator.SetInteger("BasicAttackIndex", Random.Range(0, 2));
	}

	private Collider[] CheckHitboxForEnemies(BoxCollider collider)
	{
		Vector3 size = collider.size / 2;
		size.x = Mathf.Abs(size.x);
		size.y = Mathf.Abs(size.y);
		size.z = Mathf.Abs(size.z);
		ExtDebug.DrawBox(collider.transform.position + collider.transform.forward * 0.5f, size, collider.transform.rotation, Color.blue);
		Collider[] results = Physics.OverlapBox(collider.transform.position + collider.transform.forward * 0.5f, size, collider.transform.rotation);
		return results;
	}

	public enum PlayerState
	{
		FreeMove,
		JumpKicking,
		SlideKicking
	}
}
