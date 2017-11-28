using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public const float JUMP_KICK_ALLOWANCE_TIME = 0.2f;
	public const float SLIDE_KICK_ALLOWANCE_TIME = 0.2f;

	public GameObject PlayerHUD;
	public BoxCollider MidPunchHitbox;
	public BoxCollider JumpKickHitbox;
	public BoxCollider SlideKickHitbox;

	private PlayerSoundManager playerSoundManager;
	private PlayerInput playerInput;
	private PlayerMovement playerMovement;

	public float basicAttackCooldown = 0.3f;
	public float jumpKickAttackCooldown = 0.5f;
	public float slideKickAttackCooldown = 0.5f;

	private float cooldown = 0f;

	// Use this for initialization
	void Start()
	{
		if (!GameObject.FindGameObjectWithTag("PlayerHUD"))
		{
			Instantiate(PlayerHUD);
		}

		playerSoundManager = GetComponent<PlayerSoundManager>();
		playerInput = GetComponent<PlayerInput>();
		playerMovement = GetComponent<PlayerMovement>();

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
		if (playerInput.crouchTime > 0.02 && playerInput.crouchTime < SLIDE_KICK_ALLOWANCE_TIME && cooldown <= 0)
		{
			PerformAttack(AttackType.SlideKick);
		}
		else if (playerInput.jumpTime > 0.02 && playerInput.jumpTime < JUMP_KICK_ALLOWANCE_TIME && cooldown <= 0)
		{
			PerformAttack(AttackType.JumpKick);
		}
		else if (cooldown <= 0)
		{
			PerformAttack(AttackType.BasicAttack);
		}
	}

	private void PerformAttack(AttackType attackType)
	{
		BoxCollider attackCollider = null;
		switch (attackType)
		{
			case AttackType.JumpKick:
				attackCollider = JumpKickHitbox;
				break;
			case AttackType.SlideKick:
				attackCollider = SlideKickHitbox;
				break;
			case AttackType.BasicAttack:
				attackCollider = MidPunchHitbox;
				break;
		}

		if (attackCollider == null)
		{
			Debug.Log("There was an error. The attack collider was null");
		}

		Vector3 size = attackCollider.transform.TransformVector(attackCollider.size / 2);
		size.x = Mathf.Abs(size.x);
		size.y = Mathf.Abs(size.y);
		size.z = Mathf.Abs(size.z);
		Collider[] results = Physics.OverlapBox(attackCollider.transform.position, size, attackCollider.transform.rotation);

		foreach (Collider collider in results)
		{
			switch (attackType)
			{
				case AttackType.JumpKick:
					HandleJumpKick(collider);
					break;
				case AttackType.SlideKick:
					HandleSlideKick(collider);
					break;
				case AttackType.BasicAttack:
					HandleBasicAttack(collider);
					break;
			}
		}
	}

	private void HandleJumpKick(Collider collider)
	{
		Debug.Log("JumpKick!");

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

		cooldown = jumpKickAttackCooldown;
	}

	private void HandleSlideKick(Collider collider)
	{
		Debug.Log("SlideKick!");

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

		cooldown = slideKickAttackCooldown;
	}

	private void HandleBasicAttack(Collider collider)
	{
		Debug.Log("Punch!");

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

		cooldown = basicAttackCooldown;
	}

	enum AttackType
	{
		BasicAttack,
		JumpKick,
		SlideKick
	}
}
