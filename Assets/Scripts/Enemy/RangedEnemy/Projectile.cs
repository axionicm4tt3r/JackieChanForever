using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public float fireballMaxVelocity = 15f;
	public float fireballAcceleration = 12f;
	Rigidbody fireballRigidbody;
	GameObject player;

	Vector3 targetVector;

	// Use this for initialization
	void Start () {
		fireballRigidbody = GetComponent<Rigidbody>();
		player = GameObject.FindGameObjectWithTag(Helpers.Tags.Player);

		targetVector = (this.transform.position - player.transform.position);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		fireballRigidbody.velocity += targetVector * (fireballRigidbody.velocity.magnitude + (fireballAcceleration * Time.deltaTime));

		if (fireballRigidbody.velocity.magnitude >= fireballMaxVelocity)
		{
			fireballRigidbody.velocity = fireballRigidbody.velocity.normalized * fireballMaxVelocity;
		}
	}
}
