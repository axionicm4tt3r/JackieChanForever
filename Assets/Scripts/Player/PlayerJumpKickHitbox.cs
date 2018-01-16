using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpKickHitbox : MonoBehaviour
{
	BoxCollider jumpKickHitbox;
	PlayerController playerController;
	PlayerSoundManager playerSoundManager;

    List<EnemyAI> enemiesHit = new List<EnemyAI>();

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
            if (!enemiesHit.Contains(enemyAI))
            {
                enemiesHit.Add(enemyAI);
                enemyAI.status.TakeDamage(PlayerController.JumpKickHitDamage);
                playerSoundManager.PlayJumpAttackHitSound();
            }

            enemyAI.ApplyKnockbackEffect(transform.forward, PlayerController.JumpKickHitKnockbackVelocity);
        }
    }

    public void ClearEnemiesHit()
    {
        enemiesHit.Clear();
    }
}
