using UnityEngine;

public class PlayerCameraHeadbob : MonoBehaviour
{
	public float bobbingSpeed = 0.18f;
	public float bobbingAmount = 0.2f;

    private PlayerInput playerInput;
    private PlayerMovement playerMovement;

    private float timer = 0.0f;

	private void Start()
	{
        playerInput = gameObject.GetComponentInParent<PlayerInput>();
        playerMovement = gameObject.GetComponentInParent<PlayerMovement>();
	}

	void Update()
	{
		Headbob();
	}

	private void Headbob()
	{
		float midpoint = PlayerCamera.currentViewYOffset;
		float waveslice = 0.0f;

		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

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