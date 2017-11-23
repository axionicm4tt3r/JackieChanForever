using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public GameObject PlayerHUD;
	public CapsuleCollider MidPunchHitbox;
	public CapsuleCollider JumpKickHitbox;
	public CapsuleCollider SlideKickHitbox;

	public float basicAttackCooldown = 0.5f;

	private float cooldown = 0f;
	private bool hitboxesReset = true;
	// Use this for initialization
	void Start()
	{
		if (!GameObject.FindGameObjectWithTag("PlayerHUD"))
		{
			Instantiate(PlayerHUD);
		}

		MidPunchHitbox = GameObject.FindGameObjectWithTag("MidPunchHitbox").GetComponent<CapsuleCollider>();
		JumpKickHitbox = GameObject.FindGameObjectWithTag("JumpKickHitbox").GetComponent<CapsuleCollider>();
		SlideKickHitbox = GameObject.FindGameObjectWithTag("SlideKickHitbox").GetComponent<CapsuleCollider>();
	}

	// Update is called once per frame
	void Update()
	{
		if (cooldown > 0)
		{
			cooldown -= Time.deltaTime;
		}
	}

	internal void Attack()
	{
		//Determine what kind of input we have over here
		//For now just BasicAttack
		if (cooldown <= 0)
		{
			BasicAttack();
		}
	}

	private void BasicAttack()
	{
		Debug.Log("Punch!");

		Vector3 bottom = MidPunchHitbox.transform.position;
		Vector3 top = MidPunchHitbox.transform.position + new Vector3(0, MidPunchHitbox.height, 0);
		float radius = MidPunchHitbox.radius;

		Collider[] allOverlappingColliders = Physics.OverlapCapsule(bottom, top, radius);
		foreach (Collider collider in allOverlappingColliders)
		{
			if (collider.gameObject.tag == "Enemy" || collider.gameObject.tag == "Breakable")
			{
				//DealDamage
				//ApplyProperty
				return;
			}
		}

		cooldown = basicAttackCooldown;
	}
}
