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

	//void Update ()
	//{
	//	var horizontalInput = Input.GetAxis("Horizontal");
	//	var verticalInput = Input.GetAxis("Vertical");
	//	var movementVector = new Vector3(horizontalInput, 0f, verticalInput);
	//	playerMovement.Move(movementVector);

	//	var mouseXInput = Input.GetAxis("Mouse X");
	//	var mouseYInput = Input.GetAxis("Mouse Y");
	//	var mouseLookVector = new Vector2(mouseXInput, mouseYInput);
	//	playerCamera.Look(mouseLookVector);

	//	if (Input.GetButtonDown("Fire1"))
	//	{
	//		playerController.Attack();
	//	}

	//	if (Input.GetButtonDown("Jump"))
	//	{
	//		playerMovement.Jump();
	//	}
	//}
}
