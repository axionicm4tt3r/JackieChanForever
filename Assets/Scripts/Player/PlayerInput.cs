using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	PlayerController playerController;
	PlayerMovement playerMovement;
	PlayerCamera playerCamera;

	void Start()
	{
		playerController = GetComponent<PlayerController>();
		playerMovement = GetComponent<PlayerMovement>();
		playerCamera = Camera.main.GetComponent<PlayerCamera>();
	}

	void Update()
	{
		var mouseXInput = Input.GetAxisRaw("Mouse X");
		var mouseYInput = Input.GetAxisRaw("Mouse Y");
		var mouseLookVector = new Vector2(mouseXInput, mouseYInput);
		playerCamera.Look(mouseLookVector);

		if (Input.GetButtonDown("Fire1"))
		{
			playerController.Attack();
		}

		if (Input.GetButton("Crouch"))
		{
			Debug.Log("Crouching");
			playerMovement.GoToCrouching();
		}
		else if (!Input.GetButton("Crouch"))
		{
			Debug.Log("No longer crouching");
			playerMovement.GoToStanding();
		}
	}
}
