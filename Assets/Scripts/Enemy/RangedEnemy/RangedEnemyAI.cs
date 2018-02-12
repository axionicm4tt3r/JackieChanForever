using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyAI : EnemyAI
{
	public float FireRate;
	public Transform ProjectilePrefab;

	private Transform shotSource;
	private float lastShotTime;

	public override void Awake()
	{
		base.Awake();
		shotSource = this.gameObject.FindObjectInChildren("ShotSource").transform;
	}

	public override void Update()
	{
		base.Update();

		if (status.IsDead()) //Remove this code
		{
			Die();
		}
	}

	internal override void Die() //Remove this code
	{
		base.Die();
	}

	public void Shoot()
	{
		lastShotTime = FireRate;
		var playerCentrePosition = (player.position + new Vector3(0, player.GetComponent<CharacterController>().height / 2, 0));
		GameObject.Instantiate(ProjectilePrefab, shotSource.position, Quaternion.LookRotation(playerCentrePosition - shotSource.position));
	}
}
