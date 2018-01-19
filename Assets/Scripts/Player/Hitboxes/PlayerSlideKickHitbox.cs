using System.Collections.Generic;
using UnityEngine;

public class PlayerSlideKickHitbox : MonoBehaviour
{
	PlayerController playerController;
	PlayerAttackManager playerAttackManager;

	void Start()
	{
		playerController = GetComponentInParent<PlayerController>();
		playerAttackManager = GetComponentInParent<PlayerAttackManager>();
	}

	public void OnTriggerEnter(Collider collider)
	{
		if (!(playerController.playerState == PlayerController.PlayerState.SlideKicking))
			return;

		var attackableComponent = collider.gameObject.GetAttackableComponent();
		if (attackableComponent != null)
			playerAttackManager.SlideKick(attackableComponent);
	}
}
