﻿using UnityEngine;

public class PlayerSlideKickHitbox : MonoBehaviour
{
	public float slideKickHitEnemyKnockbackVelocity = 18f;

	BoxCollider slideKickHitbox;
	PlayerController playerController;
	PlayerSoundManager playerSoundManager;

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
			enemyAI.ApplyKnockbackEffect(transform.forward, slideKickHitEnemyKnockbackVelocity);
			playerSoundManager.PlaySlideAttackHitSound();
		}
	}
}
