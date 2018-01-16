using System.Collections.Generic;
using UnityEngine;

public class PlayerSlideKickHitbox : MonoBehaviour
{
	BoxCollider slideKickHitbox;
	PlayerController playerController;
	PlayerSoundManager playerSoundManager;

    List<EnemyAI> enemiesHit = new List<EnemyAI>();

    // Use this for initialization
    void Start()
	{
		slideKickHitbox = GetComponent<BoxCollider>();
		playerController = GetComponentInParent<PlayerController>();
		playerSoundManager = GetComponentInParent<PlayerSoundManager>();
	}

	public void OnTriggerEnter(Collider collider)
	{
		if (!(playerController.playerState == PlayerController.PlayerState.SlideKicking))
			return;

		if (collider.gameObject.tag == Helpers.Tags.Enemy || collider.gameObject.tag == Helpers.Tags.Breakable)
		{
			var enemyAI = collider.gameObject.GetComponent<EnemyAI>();
            if (!enemiesHit.Contains(enemyAI))
            {
                enemiesHit.Add(enemyAI);
                enemyAI.status.TakeDamage(PlayerController.SlideKickHitDamage);
                playerSoundManager.PlaySlideAttackHitSound();
            }

            enemyAI.ApplyKnockbackEffect(transform.forward, PlayerController.SlideKickHitKnockbackVelocity);
        }
    }

    public void ClearEnemiesHit()
    {
        enemiesHit.Clear();
    }
}
