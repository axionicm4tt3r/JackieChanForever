using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyAI : MonoBehaviour
{
	public float knockbackRecoveryTimer = 0.2f;
	private float knockbackRecoveryFraction = 3f;
	private float currentKnockbackRecoveryTime = 0f;

	public float staggerRecoveryTimer = 0.2f;
	private float staggerKnockbackVelocity = 2f;
	private float currentStaggerRecoveryTime = 0f;

	internal Animator animator;
	internal EnemyStatus status;
	internal NavMeshAgent navAgent;
	internal Transform player;

	[SerializeField]
	internal float AggroDistance;

	public virtual void Start()
	{
		animator = gameObject.GetComponentInChildren<Animator>();
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
		else if (currentKnockbackRecoveryTime <= 0 && status.IsKnockedBack())
		{
			navAgent.velocity = navAgent.velocity / knockbackRecoveryFraction;
			animator.SetBool("KnockedBack", false);
			status.BecomeIdle();
		}

		if (currentStaggerRecoveryTime > 0)
		{
			status.BecomeStaggered();
			currentStaggerRecoveryTime -= Time.deltaTime;
		}
		else if (currentStaggerRecoveryTime <= 0 && status.IsStaggered())
		{
			animator.SetBool("Staggered", false);
			navAgent.velocity = -transform.forward * staggerKnockbackVelocity;
			status.BecomeIdle();
		}
	}

	public void ApplyStagger()
	{
		status.BecomeStaggered();
		currentStaggerRecoveryTime = staggerRecoveryTimer;
		animator.SetBool("Staggered", true);
	}

	public void ApplyKnockbackEffect(Vector3 knockbackDirection, float knockbackVelocity)
	{
		status.BecomeKnockedBack();
		currentKnockbackRecoveryTime = knockbackRecoveryTimer;
		animator.SetBool("KnockedBack", true);
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
