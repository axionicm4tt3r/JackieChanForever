using System.Collections.Generic;
using UnityEngine;

public class PlayerHitboxScanner : MonoBehaviour
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
		var attackableComponent = collider.gameObject.GetAttackableComponent();
		if (attackableComponent != null)
		{
			switch(playerController.attackState)
			{
				case AttackState.JumpKicking:
					playerAttackManager.JumpKick(attackableComponent);
					break;
				case AttackState.SlideKicking:
					playerAttackManager.SlideKick(attackableComponent);
					break;
				default:
					break;
			}
		}
	}
}
