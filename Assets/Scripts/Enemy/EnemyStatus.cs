using System;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class EnemyStatus : MonoBehaviour
{

	[SerializeField]
	protected float Health;

	private EnemyAI enemyAI;

	public EnemyAIState state { get; private set; }

	void Start () {
		enemyAI = GetComponent<EnemyAI>();
		state = EnemyAIState.FreeMoving;
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
		state = EnemyAIState.Dead;
		Destroy(gameObject);
	}
	#endregion

	#region AIState
	public bool IsDead()
	{
		return state == EnemyAIState.Dead;
	}

	public bool IsKnockedBack()
	{
		return state == EnemyAIState.KnockedBack;
	}

	internal bool IsStaggered()
	{
		return state == EnemyAIState.Staggered;
	}

    internal bool IsFreeMoving()
    {
        return state == EnemyAIState.FreeMoving;
    }

	public void BecomeFreeMoving()
	{
		state = EnemyAIState.FreeMoving;
	}

	public void BecomeKnockedBack()
	{
		state = EnemyAIState.KnockedBack;
	}

	internal void BecomeStaggered()
	{
		state = EnemyAIState.Staggered;
	}
	#endregion
}
