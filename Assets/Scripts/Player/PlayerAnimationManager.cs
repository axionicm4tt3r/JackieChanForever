using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour {

	private Animator playerUIAnimator;
	private Animator playerMotionAnimator;

	private Animator PlayerUIAnimator
	{
		get
		{
			if (playerUIAnimator == null)
				playerUIAnimator = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerHUD)?.GetComponentInChildren<Animator>();
			return playerUIAnimator;
		}
	}

	private Animator PlayerMotionAnimator
	{
		get
		{
			if (playerMotionAnimator == null)
				playerMotionAnimator = GameObject.FindGameObjectWithTag(Helpers.Tags.Player)?.GetComponent<Animator>();
			return playerMotionAnimator;
		}
	}

	internal void ResetAnimatorParameters()
	{
		SetAnimationBool("JumpKicking", false);
		SetAnimationBool("SlideKicking", false);
		SetAnimationBool("Blocking", false);
	}

	internal void BasicAttack()
	{
		SetAnimationInteger("BasicAttackIndex", UnityEngine.Random.Range(0, 2));
		SetAnimationTrigger("BasicAttacking");
	}

	internal void ChargeAttack()
	{
		Debug.Log("TODO: Charge Animation");
	}

	internal void Block()
	{
		SetAnimationBool("Blocking", true);
	}

	internal void JumpKick()
	{
		SetAnimationBool("JumpKicking", true);
	}

	internal void SlideKick()
	{
		SetAnimationBool("SlideKicking", true);
	}

	private void SetAnimationBool(string name, bool value)
	{
		PlayerUIAnimator?.SetBool(name, value);
		PlayerMotionAnimator?.SetBool(name, value);
	}

	private void SetAnimationInteger(string name, int value)
	{
		PlayerUIAnimator?.SetInteger(name, value);
		PlayerMotionAnimator?.SetInteger(name, value);
	}

	private void SetAnimationTrigger(string name)
	{
		PlayerUIAnimator?.SetTrigger(name);
		PlayerMotionAnimator?.SetTrigger(name);
	}
}