using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour {

	private Animator playerUIAnimator;

	void Start () {
		playerUIAnimator = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerHUD).GetComponentInChildren<Animator>();
	}

	internal void ResetAnimatorParameters()
	{
		playerUIAnimator.SetBool("JumpKicking", false);
		playerUIAnimator.SetBool("SlideKicking", false);
		playerUIAnimator.SetBool("Blocking", false);
	}

	internal void BasicAttack()
	{
		playerUIAnimator.SetInteger("BasicAttackIndex", UnityEngine.Random.Range(0, 2));
		playerUIAnimator.SetTrigger("BasicAttacking");
	}

	internal void Block()
	{
		playerUIAnimator.SetBool("Blocking", true);
	}

	internal void JumpKick()
	{
		playerUIAnimator.SetBool("JumpKicking", true);
	}

	internal void SlideKick()
	{
		playerUIAnimator.SetBool("SlideKicking", true);
	}
}
