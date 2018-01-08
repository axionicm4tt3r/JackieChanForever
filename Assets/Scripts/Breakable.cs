using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Breakable : MonoBehaviour
{
	[ReadOnly]
	public bool grabbed;

	private Transform grabbedLocation;
	private Rigidbody rigidBody;

	public void Start()
	{
		grabbed = false;
		rigidBody = GetComponent<Rigidbody>();
	}

	public void Update()
	{
        if (grabbed)
        {
            transform.position = grabbedLocation.position;
            //if ((transform.position - grabbedLocation.position).sqrMagnitude > .25f)
            //    transform.position = Vector3.Lerp(transform.position, grabbedLocation.position, 0.1f);
        }
    }

	public void Grab(Transform grabbedLocation)
	{
		grabbed = true;
        rigidBody.isKinematic = true;
		this.grabbedLocation = grabbedLocation;
	}

    public void Drop()
    {
        grabbed = false;
        rigidBody.isKinematic = false;
        this.grabbedLocation = null;
    }
}
