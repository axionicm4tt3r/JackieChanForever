using UnityEngine;
using UnityEngine.AI;

public class BumpEnemyAI : EnemyAI
{
	public override void Awake()
	{
		base.Awake();
	}

	public override void Update() //Remove this code
	{
		base.Update();

		if (status.IsDead())
		{
			Die();
		}
	}

	internal override void Die() //Remove this code
	{
		base.Die();
	}
}
