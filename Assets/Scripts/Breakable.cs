using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Breakable : MonoBehaviour
{
	[ReadOnly]
	public bool levitated;
	[ReadOnly]
	public bool grabbed;

	private bool reachedHeight = false;
	private Vector3 destination;
	private Vector3 startPosition;
	private Transform grabbedLocation;

	public float ThrowPower = 25f;

	public float LevitationHeight = 10;
	public float TimeToRise = 3f;
	public float MaxFloatTime = 10;
	private float currentFloatTime;

	private Rigidbody rigidBody;


	public void Start()
	{
		levitated = false;
		grabbed = false;
		rigidBody = GetComponent<Rigidbody>();
	}

	public void Update()
	{
		if (levitated)
		{
			currentFloatTime += Time.deltaTime;

			if (grabbed)
			{
				if ((transform.position - grabbedLocation.position).sqrMagnitude > .25f)
					transform.position = Vector3.Lerp(transform.position, grabbedLocation.position, 0.1f);
			}

			else if (!reachedHeight)
			{
				transform.position = Vector3.Lerp(startPosition, destination, currentFloatTime / TimeToRise);

				if (currentFloatTime >= TimeToRise)
				{
					reachedHeight = true;
					currentFloatTime = 0f;
				}
			}

			else
			{
				if (currentFloatTime >= MaxFloatTime)
					EndLevitate();
			}
		}
	}

	public void Levitate()
	{
		if (levitated)
			return;

		levitated = true;
		rigidBody.useGravity = false;
		startPosition = transform.position;

		destination = new Vector3(transform.position.x, transform.position.y + LevitationHeight, transform.position.z);
	}

	public void EndLevitate()
	{
		levitated = false;
		reachedHeight = false;
		rigidBody.useGravity = true;
		currentFloatTime = 0f;
	}

	public void Grab(Transform grabbedLocation)
	{
		Levitate();
		grabbed = true;
		this.grabbedLocation = grabbedLocation;
	}

	public void ThrowAtTarget(Vector3 targetLocation)
	{
		ThrowInDirection(targetLocation - transform.position);
	}

	public void ThrowInDirection(Vector3 throwDirection)
	{
		EndLevitate();
		grabbed = false;
		rigidBody.velocity = throwDirection.normalized * ThrowPower;
	}
}
