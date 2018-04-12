using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour {

	private Animator playerUIAnimator;
	private Animator playerMotionAnimator;

	void Start ()
	{
		playerUIAnimator = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerHUD).GetComponentInChildren<Animator>();
		playerMotionAnimator = GameObject.FindGameObjectWithTag(Helpers.Tags.Player).GetComponent<Animator>();
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
		playerUIAnimator.SetBool(name, value);
		playerMotionAnimator.SetBool(name, value);
	}

	private void SetAnimationInteger(string name, int value)
	{
		playerUIAnimator.SetInteger(name, value);
		playerMotionAnimator.SetInteger(name, value);
	}

	private void SetAnimationTrigger(string name)
	{
		playerUIAnimator.SetTrigger(name);
		playerMotionAnimator.SetTrigger(name);
	}
}