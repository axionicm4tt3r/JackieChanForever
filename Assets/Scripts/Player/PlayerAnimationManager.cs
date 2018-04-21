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
		SetAnimationBool(AnimationCodes.Blocking, false);
		SetAnimationBool(AnimationCodes.ChargingAttack, false);
		SetAnimationBool(AnimationCodes.JumpKicking, false);
		SetAnimationBool(AnimationCodes.SlideKicking, false);
	}

	internal void BasicAttack()
	{
		SetAnimationInteger(AnimationCodes.BasicAttackIndex, UnityEngine.Random.Range(0, 2));
	}

	internal void ChargeAttack()
	{
		SetAnimationBool(AnimationCodes.ChargingAttack, true);
	}

	internal void Block()
	{
		SetAnimationBool(AnimationCodes.Blocking, true);
	}

	internal void JumpKick()
	{
		SetAnimationBool(AnimationCodes.JumpKicking, true);
	}

	internal void SlideKick()
	{
		SetAnimationBool(AnimationCodes.SlideKicking, true);
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

	private static class AnimationCodes
	{
		public const string Blocking = "Blocking";
		public const string ChargingAttack = "ChargingAttack";
		public const string BasicAttackIndex = "BasicAttackIndex";
		public const string JumpKicking = "JumpKicking";
		public const string SlideKicking = "SlideKicking";
	}
}
