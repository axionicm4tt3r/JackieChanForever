using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyAI : MonoBehaviour
{
	public float knockbackRecoveryTimer = 0.2f;

	private float currentKnockbackRecoveryTime = 0f;

	internal EnemyStatus status;
	internal NavMeshAgent navAgent;
	internal Transform player;

	[SerializeField]
	internal float AggroDistance;

	public virtual void Start()
	{
		status = gameObject.GetComponent<EnemyStatus>();
		navAgent = gameObject.GetComponent<NavMeshAgent>();
		player = GameObject.FindGameObjectWithTag(Helpers.Tags.Player).transform;
		navAgent.isStopped = true;
	}

	public virtual void Update()
	{
		if (currentKnockbackRecoveryTime > 0)
		{
			status.BecomeKnockedBack();
			currentKnockbackRecoveryTime -= Time.deltaTime;
		}

		if (currentKnockbackRecoveryTime <= 0 && status.state == EnemyAIState.KnockedBack)
		{
			navAgent.velocity = navAgent.velocity / 2;
			status.BecomeIdle();
		}
	}

	public void ApplyKnockbackEffect(Vector3 knockbackDirection, float knockbackVelocity)
	{
		status.BecomeIdle();
		currentKnockbackRecoveryTime = knockbackRecoveryTimer;
		navAgent.velocity = knockbackDirection * knockbackVelocity;
	}

	internal virtual void Aggro()
	{
		navAgent.SetDestination(player.position);
		if (status.IsDead() || status.IsAggro())
			return;

		status.AggroOnPlayer();
	}

	internal virtual void DeAggro()
	{
		if (status.IsDead() || status.IsIdle())
			return;

		status.BecomeIdle();
	}

	internal virtual void Die()
	{
		navAgent.isStopped = true;
	}
}
