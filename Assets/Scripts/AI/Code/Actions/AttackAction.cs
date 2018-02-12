using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Attack")]
public class AttackAction : Action
{
    public override void Act(StateController controller)
    {
        Attack(controller);
    }

    private void Attack(StateController controller)
    {
        Transform playerTransform;

        Debug.DrawRay(controller.eyes.position, controller.eyes.forward.normalized * controller.enemyStats.attackRange, Color.red);

        if (CanSeePlayer(controller, out playerTransform))
        {
            if (controller.CheckIfCountDownElapsed(controller.enemyStats.attackRate))
            {
                Debug.Log("Attack!");
                //controller.enemyAI.Attack(controller.enemyStats.attackForce, controller.enemyStats.attackRate); //Play attack animation, do damage
            }
            //else 
            //1) dodge...
            //2) block...
            //3) idle...
            //4) strafe around player
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