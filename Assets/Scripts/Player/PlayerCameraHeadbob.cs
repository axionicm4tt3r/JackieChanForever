using UnityEngine;

public class PlayerCameraHeadbob : MonoBehaviour
{
	public float bobbingSpeed = 0.18f;
	public float bobbingAmount = 0.2f;

	private PlayerInputManager playerInputManager;
	private PlayerStateMachine playerStateMachine;
	private PlayerController playerController;

	private float timer = 0.0f;

	private void Start()
	{
		playerInputManager = gameObject.GetComponentInParent<PlayerInputManager>();
		playerStateMachine = gameObject.GetComponentInParent<PlayerStateMachine>();
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

		float horizontal = playerInputManager.Current.MoveInput.x;
		float vertical = playerInputManager.Current.MoveInput.y;
	
		Vector3 cSharpConversion = transform.localPosition;

		if (!playerStateMachine.MaintainingGround())
		{
			timer = Mathf.Lerp(timer, 0.0f, 1 - Mathf.Abs(timer));
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
			var playerMaxSpeed = playerInputManager.Current.CrouchInput ? PlayerStateMachine.RunSpeed : PlayerStateMachine.CrouchSpeed;
			var planarMovementVector = new Vector2(playerStateMachine.moveDirection.x, playerStateMachine.moveDirection.z);
			float translateChange = waveslice * bobbingAmount * (planarMovementVector.magnitude / playerMaxSpeed);
			float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
			totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
			translateChange = totalAxes * translateChange;
			cSharpConversion.y = midpoint + translateChange;
		}
		else
		{
			cSharpConversion.y = midpoint;
		}

		transform.localPosition = cSharpConversion; //This moves the camera
	}
}