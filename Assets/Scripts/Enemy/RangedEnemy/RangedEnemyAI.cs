using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyAI : EnemyAI
{
	[SerializeField]
	private float FireRate;
	[SerializeField]
	private Transform ProjectilePrefab;

	private Transform shotSource;
	private float lastShotTime;

	public override void Start()
	{
		base.Start();
		shotSource = this.gameObject.FindObjectInChildren("ShotSource").transform;
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
			if (lastShotTime > 0)
				lastShotTime -= Time.deltaTime;

			float distanceToPlayer = (transform.position - player.position).sqrMagnitude;
			if (distanceToPlayer < AggroDistance)
				Aggro();
			else
				DeAggro();

			if (lastShotTime <= 0 && status.IsAggro())
				Shoot();

			if (status.IsAggro())
			{
				transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
			}
		}
	}

	internal override void Aggro()
	{
		base.Aggro();
	}

	internal override void DeAggro()
	{
		base.DeAggro();
	}

	internal override void Die()
	{
		base.Die();
	}

	private void Shoot()
	{
		lastShotTime = FireRate;
		var playerCentrePosition = (player.position + new Vector3(0, player.GetComponent<CharacterController>().height / 2, 0));
		GameObject.Instantiate(ProjectilePrefab, shotSource.position, Quaternion.LookRotation(playerCentrePosition - shotSource.position));
	}
}
