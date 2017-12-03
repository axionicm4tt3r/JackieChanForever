using UnityEngine;

public class PlayerJumpKickHitbox : MonoBehaviour
{
	BoxCollider jumpKickHitbox;
	PlayerController playerController;
	PlayerSoundManager playerSoundManager;

	// Use this for initialization
	void Start()
	{
		jumpKickHitbox = GetComponent<BoxCollider>();
		playerController = GetComponentInParent<PlayerController>();
		playerSoundManager = GetComponentInParent<PlayerSoundManager>();
	}

	public void OnTriggerEnter(Collider collider)
	{
		if (!(playerController.playerState == PlayerController.PlayerState.JumpKicking))
			return;

		if (collider.gameObject.tag == Helpers.Tags.Enemy || collider.gameObject.tag == Helpers.Tags.Breakable)
		{
			var enemyAI = collider.gameObject.GetComponent<EnemyAI>();
			enemyAI.ApplyKnockbackEffect(transform.forward, PlayerController.JumpKickHitKnockbackVelocity);
			enemyAI.status.TakeDamage(PlayerController.JumpKickHitDamage);
			playerSoundManager.PlayJumpAttackHitSound();
		}
	}
}
