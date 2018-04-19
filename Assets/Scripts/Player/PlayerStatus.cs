using System;
using UnityEngine;

public class PlayerStatus : MonoBehaviour, IAttackable
{
	public float MaxHealth;
	[ReadOnly] public float Health;

	public GameObject PlayerHUD;

	private PlayerMovementStateMachine playerMovementStateMachine;

	private float knockbackRecoveryFraction = 3f;
	private float currentKnockbackRecoveryTime = 0f;
	private float staggerKnockbackVelocity = 2f;
	private float currentStaggerRecoveryTime = 0.1f;

	public HealthState state { get; private set; }

	public string stateName { get { return state.ToString(); } }

	void Awake () {
		state = HealthState.FreeMoving;

		if (!GameObject.FindGameObjectWithTag("PlayerHUD"))
			PlayerHUD = Instantiate(PlayerHUD) as GameObject;
	}

	public void ReceiveAttack(float damage)
	{
		TakeDamage(damage);
	}

	public void ReceiveStaggerAttack(float damage, Vector3 staggerDirection, float staggerRecoveryTime)
	{
		BecomeStaggered();

		currentStaggerRecoveryTime = staggerRecoveryTime;
		//animator.SetBool("Staggered", true);
		playerMovementStateMachine.moveDirection += staggerDirection * staggerKnockbackVelocity;

		TakeDamage(damage);
	}

	public void ReceiveKnockbackAttack(float damage, Vector3 knockbackDirection, float knockbackVelocity, float knockbackTime)
	{
		BecomeKnockedBack();

		currentKnockbackRecoveryTime = knockbackTime;
		//animator.SetBool("KnockedBack", true);
		playerMovementStateMachine.moveDirection += knockbackDirection * knockbackVelocity;

		TakeDamage(damage);
	}

	#region HealthState
	internal void TakeDamage(float damage)
	{
		Health -= damage;

		if (Health <= 0)
			Die();
	}

	internal virtual void Die()
	{
		state = HealthState.Dead;
	}
	#endregion

	#region PlayerState
	public bool IsDead()
	{
		return state == HealthState.Dead;
	}

	public bool IsKnockedBack()
	{
		return state == HealthState.KnockedBack;
	}

	internal bool IsStaggered()
	{
		return state == HealthState.Staggered;
	}

	internal bool IsFreeMoving()
	{
		return state == HealthState.FreeMoving;
	}

	public void BecomeFreeMoving()
	{
		state = HealthState.FreeMoving;
	}

	public void BecomeKnockedBack()
	{
		state = HealthState.KnockedBack;
	}

	internal void BecomeStaggered()
	{
		state = HealthState.Staggered;
	}
	#endregion
}
