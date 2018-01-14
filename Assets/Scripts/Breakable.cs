using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Breakable : MonoBehaviour
{
	[ReadOnly]
	public bool grabbed;

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
        if (collision.gameObject.tag == Helpers.Tags.Enemy)
        {
            Debug.Log("Damaging Enemy with a Throwable!");
            var currentVelocity = rigidbody.velocity.magnitude;

            if (currentVelocity < 5f)
            {
                return;
            }
            else
            {
                var enemyAI = collision.gameObject.GetComponent<EnemyAI>();
                var direction = (collision.gameObject.transform.position - transform.position).normalized;
                enemyAI.ApplyKnockbackEffect(direction, 10f);
                enemyAI.status.TakeDamage(PlayerController.JumpKickHitDamage);
                return;
            }
        }

        if (collision.gameObject.tag == Helpers.Tags.Player)
        {
            Debug.Log("Damaging Player with a Throwable!");
        }
    }
}
