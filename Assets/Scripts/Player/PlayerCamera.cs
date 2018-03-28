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

	private float rotX = 0.0f;
	private float rotY = 0.0f;

	private PlayerInputManager playerInputManager;

	void Awake ()
	{
		playerInputManager = GameObject.FindGameObjectWithTag(Helpers.Tags.Player).GetComponent<PlayerInputManager>();

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
}
