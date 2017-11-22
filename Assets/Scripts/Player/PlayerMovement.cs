using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	Rigidbody playerRigidbody;

	float maxVelocity = 10;
	float acceleration = 150;
	float drag = 60;
	float jumpForce = 250;

	// Use this for initialization
	void Start () {
		playerRigidbody = GetComponent<Rigidbody>();
	}

	internal void Move(Vector3 inputVector)
	{
		//Debug.Log("Input is X:" + inputVector.x + " Y:" + inputVector.y);
		playerRigidbody.velocity += inputVector * acceleration * Time.deltaTime;
		playerRigidbody.velocity -= playerRigidbody.velocity / drag;

		if (playerRigidbody.velocity.magnitude > maxVelocity)
			playerRigidbody.velocity = playerRigidbody.velocity.normalized * maxVelocity;
		//Debug.Log("Player velocity = " + playerRigidbody.velocity);
	}

	internal void Jump()
	{
		playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
	}
}
