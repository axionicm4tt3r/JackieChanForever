using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Breakable : MonoBehaviour, IAttackable
{
	[ReadOnly]
	public bool grabbed;

	#region AttackProperties
	public static float ThrownBreakableDamageVelocityThreshold = 5f;
	public static float ThrownBreakableDamage = 18f;
	public static float ThrownBreakableKnockbackVelocity = 25f;
	public static float ThrownBreakableKnockbackTime = 0.2f;
	#endregion

	private Transform grabbedLocation;
	private new Collider collider;
	private new Rigidbody rigidbody;

	public void Start()
	{
		grabbed = false;
		collider = GetComponent<Collider>();
		rigidbody = GetComponent<Rigidbody>();
	}

	public void Update()
	{
		if (grabbed)
		{
			transform.position = grabbedLocation.position;
		}
	}

	public void ReceiveAttack(float damage)
	{
		//Needs Breakable status to take damage
	}

	public void ReceiveStaggerAttack(float damage, Vector3 staggerDirection, float staggerTime)
	{
		//Needs Breakable status to take damage
	}

	public void ReceiveKnockbackAttack(float damage, Vector3 knockbackDirection, float knockbackVelocity, float knockbackTime)
	{
		//Needs Breakable status to take damage
		this.rigidbody.velocity = knockbackDirection * knockbackVelocity / rigidbody.mass; //Something like this
	}

	public void Grab(Transform grabbedLocation)
	{
		grabbed = true;
		rigidbody.isKinematic = true;
		collider.enabled = false;
		this.grabbedLocation = grabbedLocation;
	}

	public void Drop()
	{
		grabbed = false;
		rigidbody.isKinematic = false;
		collider.enabled = true;
		this.grabbedLocation = null;
	}

	public void OnCollisionEnter(Collision collision)
	{
		Debug.Log("Damaging Enemy with a Throwable!");
		var attackableComponent = collision.gameObject.GetAttackableComponent();
		if (attackableComponent != null)
		{
			var direction = (collision.gameObject.transform.position - transform.position).normalized;
			var currentVelocity = rigidbody.velocity.magnitude;

			if (currentVelocity >= ThrownBreakableDamageVelocityThreshold)
				attackableComponent.ReceiveKnockbackAttack(ThrownBreakableDamage, direction, ThrownBreakableKnockbackVelocity, ThrownBreakableKnockbackTime);
		}
	}
}
