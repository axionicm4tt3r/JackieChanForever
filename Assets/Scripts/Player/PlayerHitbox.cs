using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
	PlayerAttackStateManager playerController;
	PlayerAttackManager playerAttackManager;

	void Start()
	{
		playerController = GetComponentInParent<PlayerAttackStateManager>();
		playerAttackManager = GetComponentInParent<PlayerAttackManager>();
	}

	public void OnTriggerStay(Collider collider)
	{
		var attackableComponent = collider.gameObject.GetAttackableComponent();
		if (attackableComponent != null)
		{
			switch(playerController.attackState)
			{
				case PlayerAttackState.JumpKicking:
					playerAttackManager.JumpKick(attackableComponent);
					break;
				case PlayerAttackState.SlideKicking:
					playerAttackManager.SlideKick(attackableComponent);
					break;
				default:
					break;
			}
		}
	}
}
