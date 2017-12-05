
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
	private Breakable liftedObject;
    private bool objectGrabbed = false;
    private bool objectLifted = false;
    private bool buttonHeld = false;
    private float timeButtonHeld = 0f;
    private Transform grabbedLocation;

    void Start()
    {
        grabbedLocation = Helpers.FindObjectInChildren(transform.parent.gameObject, "GrabbedLocation").transform;
	}

    public override void PickupObject(Vector3 spawnPosition, Vector3 spawnDirection, Vector3? targetPosition = null)
    {
        buttonHeld = true;

        if (objectGrabbed)
        {
            if(targetPosition.HasValue)
                liftedObject.ThrowAtTarget(targetPosition.Value);
            else
                liftedObject.ThrowInDirection(spawnDirection);
            liftedObject = null;
            objectGrabbed = false;
        }

        else
        {
            
            RaycastHit hitInfo;
            if (Physics.Raycast(spawnPosition, spawnDirection, out hitInfo))
            {
                Breakable newLiftedObject = hitInfo.collider.GetComponent<Breakable>();
                if (newLiftedObject == null)
                    return;
                    
                if (objectLifted && newLiftedObject == liftedObject)
                {
                    liftedObject.EndLevitate();
                    objectLifted = false;
                }
                else if(playerStatus.UseMana(castManaCost))
                {
                    if (objectLifted)
                        liftedObject.EndLevitate();
                    objectLifted = true;
                    liftedObject = newLiftedObject;
                    liftedObject.Levitate();
                }
            }
            
        }
    }

    public void ThrowObject(Vector3 spawnPosition, Vector3 spawnDirection, Vector3? targetPosition = null)
    {
        buttonHeld = false;
        timeButtonHeld = 0;

        if (!objectLifted && !objectGrabbed)
            liftedObject = null;
    }

	private void GrabObject()
    {
        if (objectGrabbed)
            return;
        if (liftedObject == null)
            return;
        
        objectGrabbed = true;
        objectLifted = false;
        liftedObject.Grab(grabbedLocation);
	}
}
