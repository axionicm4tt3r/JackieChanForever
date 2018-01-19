using UnityEngine;

public class PlayerCameraHeadbob : MonoBehaviour
{
	public float bobbingSpeed = 0.18f;
	public float bobbingAmount = 0.2f;

	private PlayerInputManager playerInput;
	private PlayerMovementManager playerMovement;
	private PlayerController playerController;

	private float timer = 0.0f;

	private void Start()
	{
		playerInput = gameObject.GetComponentInParent<PlayerInputManager>();
		playerMovement = gameObject.GetComponentInParent<PlayerMovementManager>();
		playerController = gameObject.GetComponentInParent<PlayerController>();
	}

	void Update()
	{
		Headbob();
	}

	private void Headbob()
	{
		float midpoint = PlayerCamera.currentViewYOffset;
		float waveslice = 0.0f;

		float horizontal = 0f;
		float vertical = 0f;

		if (playerController.playerState == PlayerController.PlayerState.FreeMove)
		{
			horizontal = Input.GetAxis("Horizontal");
			vertical = Input.GetAxis("Vertical");
		}
	
		Vector3 cSharpConversion = transform.localPosition;

		if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
		{
			timer = 0.0f;
		}
		else
		{
			waveslice = Mathf.Sin(timer);
			timer = timer + bobbingSpeed;
			if (timer > Mathf.PI * 2)
			{
				timer = timer - (Mathf.PI * 2);
			}
		}
		if (waveslice != 0)
		{
			var playerMaxSpeed = playerInput.IsCrouched ? playerMovement.moveSpeed : playerMovement.moveSpeedCrouched;
			float translateChange = waveslice * bobbingAmount * (playerMovement.playerVelocity.magnitude / playerMaxSpeed);
			float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
			totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
			translateChange = totalAxes * translateChange;
			cSharpConversion.y = midpoint + translateChange;
		}
		else
		{
			cSharpConversion.y = midpoint;
		}

		transform.localPosition = cSharpConversion;
	}
}