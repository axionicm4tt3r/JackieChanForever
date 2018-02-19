﻿using System.Collections;
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

        if (controller.CanSeePlayer(out playerTransform))
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
}