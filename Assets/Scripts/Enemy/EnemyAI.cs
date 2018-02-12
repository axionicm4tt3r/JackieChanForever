﻿using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyStatus))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyAI : MonoBehaviour, IAttackable
{
	private float knockbackRecoveryFraction = 3f;
	private float currentKnockbackRecoveryTime = 0f;

	private float staggerKnockbackVelocity = 2f;
	private float currentStaggerRecoveryTime = 0.1f;

	private Vector3 staggerDirection;

	internal Animator animator;
	internal EnemyStatus status;
	internal NavMeshAgent navAgent;
	internal Transform player;

	public virtual void Awake()
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
			status.BecomeFreeMoving();
		}

		if (currentStaggerRecoveryTime > 0)
		{
			status.BecomeStaggered();
			currentStaggerRecoveryTime -= Time.deltaTime;
		}
		else if (currentStaggerRecoveryTime <= 0 && status.IsStaggered())
		{
			animator.SetBool("Staggered", false);
			navAgent.velocity = staggerDirection * staggerKnockbackVelocity;
			status.BecomeFreeMoving();
		}
	}

	public void ReceiveAttack(float damage)
	{
		status.TakeDamage(damage);
	}

	public void ReceiveStaggerAttack(float damage, Vector3 staggerDirection, float staggerRecoveryTime)
	{
		status.BecomeStaggered();
		this.staggerDirection = staggerDirection;
		currentStaggerRecoveryTime = staggerRecoveryTime;
		animator.SetBool("Staggered", true);

		status.TakeDamage(damage);
	}

	public void ReceiveKnockbackAttack(float damage, Vector3 knockbackDirection, float knockbackVelocity, float knockbackTime)
	{
		status.BecomeKnockedBack();
		currentKnockbackRecoveryTime = knockbackTime;
		animator.SetBool("KnockedBack", true);
		navAgent.velocity = knockbackDirection * knockbackVelocity;

		status.TakeDamage(damage);
	}

	internal virtual void Die() //Pull this code and put it into the state logic
	{
		navAgent.isStopped = true;
	}
}
