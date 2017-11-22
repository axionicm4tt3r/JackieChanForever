using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	PlayerController playerController;
	PlayerMovement playerMovement;
	Camera playerCamera;
	
	void Start()
	{
		playerController = GetComponent<PlayerController>();
		playerMovement = GetComponent<PlayerMovement>();
		playerCamera = GameObject.FindWithTag("Camera").GetComponent<Camera>();
	}

	void Update ()
	{
		
	}
}
