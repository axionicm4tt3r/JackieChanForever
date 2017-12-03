using UnityEngine;
using UnityEngine.AI;

public class BumpEnemyAI : EnemyAI
{
	public override void Start()
	{
		base.Start();
	}

	public override void Update()
	{
		base.Update();

		if (status.IsDead())
		{
			Die();
		}
		else if (!status.IsKnockedBack())
		{
			float distanceToPlayer = (new Vector2(transform.position.x, transform.position.z) - new Vector2(player.position.x, player.position.z)).magnitude;
			if (distanceToPlayer < AggroDistance)
				Aggro();
			else
				DeAggro();
		}
	}

	internal override void Aggro()
	{
		base.Aggro();
		navAgent.isStopped = false;
	}

	internal override void DeAggro()
	{
		base.DeAggro();
		navAgent.isStopped = true;
	}

	internal override void Die()
	{
		base.Die();
	}
}
