using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpKickHitbox : MonoBehaviour
{
	PlayerController playerController;
	PlayerAttackManager playerAttackManager;

	void Start()
	{
		playerController = GetComponentInParent<PlayerController>();
		playerAttackManager = GetComponentInParent<PlayerAttackManager>();
	}

	public void OnTriggerStay(Collider collider)
	{
		if (!(playerController.playerState == PlayerController.PlayerState.JumpKicking))
			return;

		var attackableComponent = collider.gameObject.GetAttackableComponent();
		if (attackableComponent != null)
			playerAttackManager.JumpKick(attackableComponent);
	}
}
