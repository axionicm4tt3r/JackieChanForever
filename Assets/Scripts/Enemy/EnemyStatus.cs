﻿using System;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class EnemyStatus : MonoBehaviour
{

	[SerializeField]
	protected float Health;

	private EnemyAI enemyAI;

	public EntityHealthState state { get; private set; }

	public string stateName { get { return state.ToString(); } }

	void Start () {
		enemyAI = GetComponent<EnemyAI>();
		state = EntityHealthState.FreeMoving;
	}

	#region HealthState
	internal void TakeDamage(float damage)
	{
		Health -= damage;
		enemyAI.wasAttacked = true;

		if (Health <= 0)
			Die();
	}

	internal virtual void Die()
	{
		state = EntityHealthState.Dead;
		Destroy(gameObject);
	}
	#endregion

	#region AIState
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
