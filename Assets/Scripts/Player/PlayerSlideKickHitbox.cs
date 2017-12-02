using UnityEngine;

public class PlayerSlideKickHitbox : MonoBehaviour
{

	BoxCollider slideKickHitbox;
	PlayerController playerController;
	PlayerSoundManager playerSoundManager;

	// Use this for initialization
	void Start()
	{
		slideKickHitbox = GetComponent<BoxCollider>();
		playerController = GetComponentInParent<PlayerController>();
		playerSoundManager = GetComponentInParent<PlayerSoundManager>();
	}

	public void OnTriggerEnter(Collider collider)
	{
		if (!(playerController.playerState == PlayerController.PlayerState.SlideKicking))
			return;

		if (collider.gameObject.tag == Helpers.Tags.Enemy || collider.gameObject.tag == Helpers.Tags.Breakable)
		{
			//DealDamage
			//ApplyProperty
			playerSoundManager.PlaySlideAttackHitSound();
		}
	}
}
