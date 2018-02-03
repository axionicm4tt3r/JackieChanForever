using UnityEngine;
using UnityEngine.AI;

public class BumpEnemyAI : EnemyAI
{
	public override void Start()
	{
		base.Start();
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
