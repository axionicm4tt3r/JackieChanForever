using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public GameObject PlayerHUD;
	public PlayerSoundManager playerSoundManager;
	public BoxCollider MidPunchHitbox;
	public BoxCollider JumpKickHitbox;
	public BoxCollider SlideKickHitbox;

	public float basicAttackCooldown = 0.3f;

	private float cooldown = 0f;
	private bool hitboxesReset = true;
	// Use this for initialization
	void Start()
	{
		if (!GameObject.FindGameObjectWithTag("PlayerHUD"))
		{
			Instantiate(PlayerHUD);
		}

		playerSoundManager = GetComponent<PlayerSoundManager>();

		MidPunchHitbox = GameObject.FindGameObjectWithTag("MidPunchHitbox").GetComponent<BoxCollider>();
		JumpKickHitbox = GameObject.FindGameObjectWithTag("JumpKickHitbox").GetComponent<BoxCollider>();
		SlideKickHitbox = GameObject.FindGameObjectWithTag("SlideKickHitbox").GetComponent<BoxCollider>();
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

		Vector3 size = MidPunchHitbox.transform.TransformVector(MidPunchHitbox.size / 2);
		size.x = Mathf.Abs(size.x);
		size.y = Mathf.Abs(size.y);
		size.z = Mathf.Abs(size.z);
		Collider[] results = Physics.OverlapBox(MidPunchHitbox.transform.position, size, MidPunchHitbox.transform.rotation);

		foreach (Collider collider in results)
		{
			if (collider.gameObject.tag == "Enemy" || collider.gameObject.tag == "Breakable")
			{
				//DealDamage
				//ApplyProperty
				playerSoundManager.PlayBasicAttackHitSound();
			}
			else
			{
				playerSoundManager.PlayBasicAttackMissSound();
			}
		}

		cooldown = basicAttackCooldown;
	}
}
