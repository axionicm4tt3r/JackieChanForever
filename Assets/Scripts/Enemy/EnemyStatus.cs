using System;
using UnityEngine;

public class EnemyStatus : MonoBehaviour {

    [SerializeField]
    protected float Health;
	public EnemyAIState state { get; private set; }

    void Start () {
        state = EnemyAIState.Idle;
    }

    void Update () {
		
	}

    #region HealthState
    protected void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
            Die();
    }

    protected virtual void Die()
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

    public bool IsAggro()
    {
        return state == EnemyAIState.Aggro;
    }

    public bool IsIdle()
    {
        return state == EnemyAIState.Idle;
    }

	public bool IsKnockedBack()
	{
		return state == EnemyAIState.KnockedBack;
	}

	public void AggroOnPlayer()
    {
        state = EnemyAIState.Aggro;
    }

    public void BecomeIdle()
    {
        state = EnemyAIState.Idle;
    }

	public void BecomeKnockedBack()
	{
		state = EnemyAIState.KnockedBack;
	}
	#endregion
}
