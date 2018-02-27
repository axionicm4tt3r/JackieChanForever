using System;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
	[SerializeField]
	public float MaxHealth;
	[ReadOnly]
	public float Health;

	public EntityHealthState state { get; private set; }

	public string stateName { get { return state.ToString(); } }

	void Start () {
		state = EntityHealthState.FreeMoving;
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
		state = EntityHealthState.Dead;
	}
	#endregion

	#region PlayerState
	public bool IsDead()
	{
		return state == EntityHealthState.Dead;
	}

	public bool IsKnockedBack()
	{
		return state == EntityHealthState.KnockedBack;
	}

	internal bool IsStaggered()
	{
		return state == EntityHealthState.Staggered;
	}

	internal bool IsFreeMoving()
	{
		return state == EntityHealthState.FreeMoving;
	}

	public void BecomeFreeMoving()
	{
		state = EntityHealthState.FreeMoving;
	}

	public void BecomeKnockedBack()
	{
		state = EntityHealthState.KnockedBack;
	}

	internal void BecomeStaggered()
	{
		state = EntityHealthState.Staggered;
	}
	#endregion
}
