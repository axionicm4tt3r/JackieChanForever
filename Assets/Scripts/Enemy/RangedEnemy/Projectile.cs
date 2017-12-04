using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public float fireballMaxVelocity = 15f;
	public float fireballAcceleration = 12f;
	Rigidbody fireballRigidbody;

	// Use this for initialization
	void Start () {
		fireballRigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		fireballRigidbody.velocity += transform.forward * (fireballRigidbody.velocity.magnitude + (fireballAcceleration * Time.deltaTime));

		if (fireballRigidbody.velocity.magnitude >= fireballMaxVelocity)
		{
			fireballRigidbody.velocity = fireballRigidbody.velocity.normalized * fireballMaxVelocity;
		}
	}
}
