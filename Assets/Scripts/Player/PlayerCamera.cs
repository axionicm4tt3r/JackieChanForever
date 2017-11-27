using System;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	public const float PLAYER_STANDING_VIEW_Y_OFFSET = 1.8f; // The height at which the camera is bound to
	public const float PLAYER_CROUCHING_VIEW_Y_OFFSET = 0.9f; // The height at which the camera is bound to
	public static float currentViewYOffset = PLAYER_STANDING_VIEW_Y_OFFSET; // The height at which the camera is bound to

	public float xMouseSensitivity = 45.0f;
	public float yMouseSensitivity = 45.0f;

	private float rotX = 0.0f;
	private float rotY = 0.0f;

	// Use this for initialization
	void Start ()
	{
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
		//Need to move the camera after the player has been moved because otherwise the camera will clip the player if going fast enough and will always be 1 frame behind.
		//Set the camera's position to the transform
		//transform.position = new Vector3(
		//	transform.root.position.x,
		//	transform.root.position.y + currentViewYOffset,
		//	transform.root.position.z);
	}

	internal void Look(Vector2 input)
	{
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			if (Input.GetButtonDown("Fire1"))
				Cursor.lockState = CursorLockMode.Locked;
		}

		rotX -= input.y * xMouseSensitivity * 0.02f;
		rotY += input.x * yMouseSensitivity * 0.02f;

		if (rotX < -90)
			rotX = -90;
		else if (rotX > 90)
			rotX = 90;

		transform.root.rotation = Quaternion.Euler(0, rotY, 0);
		this.transform.rotation = Quaternion.Euler(rotX, rotY, 0);
	}
}
