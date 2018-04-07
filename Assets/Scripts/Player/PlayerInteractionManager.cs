
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    private Camera playerCamera;

    private Interactable grabbedObject;
    private bool objectGrabbed = false;
    private Transform grabbedLocation;

    private float playerInteractionRange = 4f;
    private float playerThrowPower = 25f;

    void Awake()
    {
        grabbedLocation = Helpers.FindObjectInChildren(gameObject, "GrabbedLocation").transform;
        playerCamera = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerCamera).GetComponent<Camera>();
    }

    public bool Grabbing { get { return objectGrabbed; } }

    public void Interact()
    {
        if (objectGrabbed)
        {
            Drop();
        }
        else
        {
            Ray screenRay = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            screenRay.direction *= playerInteractionRange;
            RaycastHit hitInfo;

            var layerMask = ~LayerMask.GetMask(Helpers.Layers.PlayerHitbox, "IgnoreRaycast");

            if (Physics.Raycast(screenRay, out hitInfo, playerInteractionRange, layerMask))
            {
                Interactable newLiftedObject = hitInfo.collider.gameObject.GetComponent<Interactable>();
                if (newLiftedObject == null)
                    return;
                else
                {
                    Grab(newLiftedObject);
                }
            }
        }
    }

    private void Grab(Interactable newLiftedObject)
    {
        objectGrabbed = true;
        grabbedObject = newLiftedObject;
        grabbedObject.Grab(grabbedLocation);
        //Debug.Log("Picked up a new object");
    }

    public void Drop()
    {
        objectGrabbed = false;
        grabbedObject.grabbed = false;
        grabbedObject.Drop();
        //Debug.Log("Threw the old object");
    }

    public void Throw()
    {
        objectGrabbed = false;
        grabbedObject.grabbed = false;
        grabbedObject.Drop();
        Ray screenRay = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        grabbedObject.GetComponent<Rigidbody>().velocity = screenRay.direction * playerThrowPower;
    }
}
