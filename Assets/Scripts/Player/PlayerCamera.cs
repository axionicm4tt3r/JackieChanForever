using System;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	//Move to PlayerConstants class or something
	public const float PLAYER_STANDING_VIEW_Y_OFFSET = 2.0f;
	public const float PLAYER_CROUCHING_VIEW_Y_OFFSET = 1.0f;
	public static float currentViewYOffset = PLAYER_STANDING_VIEW_Y_OFFSET;

	public float xMouseSensitivity = 45.0f;
	public float yMouseSensitivity = 45.0f;

	public float bobbingSpeed = 0.18f;
	public float bobbingAmount = 0.2f;

	private float rotX = 0.0f;
	private float rotY = 0.0f;

	private float timer = 0.0f;

	private PlayerInputManager playerInputManager;
	private PlayerMovementStateMachine playerStateMachine;
	private PlayerAttackStateManager playerController;

	void Awake ()
	{
		playerInputManager = gameObject.GetComponentInParent<PlayerInputManager>();
		playerStateMachine = gameObject.GetComponentInParent<PlayerMovementStateMachine>();
		playerController = gameObject.GetComponentInParent<PlayerAttackStateManager>();

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		// Put the camera inside the capsule collider
		transform.position = new Vector3(
			transform.root.position.x,
			transform.root.position.y + currentViewYOffset,
			transform.root.position.z);
	}

	private void LateUpdate()
	{
		MouseLook();

		Headbob();
	}

	internal void MouseLook()
	{
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			if (Input.GetButtonDown("PrimaryFire"))
				Cursor.lockState = CursorLockMode.Locked;
		}

		rotX -= playerInputManager.Current.MouseInput.y * xMouseSensitivity * 0.02f;
		rotY += playerInputManager.Current.MouseInput.x * yMouseSensitivity * 0.02f;

		if (rotX < -90)
			rotX = -90;
		else if (rotX > 90)
			rotX = 90;

		transform.root.rotation = Quaternion.Euler(0, rotY, 0);
		this.transform.rotation = Quaternion.Euler(rotX, rotY, 0);
	}

	private void Headbob()
	{
		float midpoint = PlayerCamera.currentViewYOffset;
		float waveslice = 0.0f;

		float horizontal = playerInputManager.Current.MoveInput.x;
		float vertical = playerInputManager.Current.MoveInput.z;

		Vector3 cSharpConversion = transform.localPosition;

		if (PlayerShouldHeadbob())
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
			var playerMaxSpeed = playerInputManager.Current.CrouchInput ? PlayerMovementStateMachine.RunSpeed : PlayerMovementStateMachine.CrouchSpeed;
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

	private bool PlayerShouldHeadbob()
	{
		return !playerStateMachine.MaintainingGround() ||
			!(playerController.attackState == PlayerAttackState.Idle ||
			playerController.attackState == PlayerAttackState.Blocking ||
			playerController.attackState == PlayerAttackState.Charging);
	}
}
