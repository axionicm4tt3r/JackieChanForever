﻿using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	CharacterController characterController;
	PlayerController playerController;
	PlayerMovement playerMovement;
	PlayerCamera playerCamera;

	public float crouchTime = 0f;
	public float jumpTime = 0f;

    public bool IsCrouched { get { return crouchTime > 0; } }

	void Start()
	{
		characterController = GetComponent<CharacterController>();
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

		if (Input.GetButtonDown("PrimaryFire"))
		{
			playerController.Attack();
		}

        if (Input.GetButtonDown("Interact"))
        {
            playerController.Interact();
        }

		if (Input.GetButton("Crouch") || playerController.playerState == PlayerController.PlayerState.SlideKicking)
		{
			playerMovement.GoToCrouching();
			crouchTime += Time.deltaTime;
		}
		else if (!Input.GetButton("Crouch"))
		{
			playerMovement.GoToStanding();
			crouchTime = 0;
		}

		HandlePlayerIsGroundedTime();
	}

	private void HandlePlayerIsGroundedTime()
	{
		if (characterController.isGrounded)
		{
			jumpTime = 0;
		}
		else if (jumpTime < PlayerController.JUMP_KICK_ALLOWANCE_TIME)
		{
			jumpTime += Time.deltaTime;
		}
	}
}
