using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Look")]
public class LookDecision : Decision
{

    public override bool Decide(StateController controller)
    {
        bool targetVisible = Look(controller);
        return targetVisible;
    }

    private bool Look(StateController controller)
    {
        Transform playerTransform;

        Debug.DrawRay(controller.eyes.position, controller.eyes.forward.normalized * controller.enemyStats.fieldOfVisionDistance, Color.green);

        if (CanSeePlayer(controller, out playerTransform))
        {
            Debug.Log("Going to the player now");
            controller.chaseTarget = playerTransform;
            controller.targetLastKnownPosition = playerTransform.position;
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CanSeePlayer(StateController controller, out Transform playerTransform)
    {
        RaycastHit hit;
        Vector3 rayDirection = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerCamera).transform.position - controller.eyes.transform.position;

        if ((Vector3.Angle(rayDirection, controller.eyes.transform.forward)) <= controller.enemyStats.fieldOfVisionAngle * 0.5f)
        {
            var layerMask = ~LayerMask.GetMask(Helpers.Layers.PlayerHitbox);
            if (Physics.Raycast(controller.eyes.transform.position, rayDirection, out hit, controller.enemyStats.fieldOfVisionDistance, layerMask))
            {
                if (hit.transform.tag == Helpers.Tags.Player || hit.transform.tag == Helpers.Tags.PlayerCamera)
                {
                    playerTransform = hit.transform;
                    return true;
                }
            }
        }

        playerTransform = null;
        return false;
    }
}