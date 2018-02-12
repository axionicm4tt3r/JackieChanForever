using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
    #region AttackProperties
    public static float BasicAttackDamage = 5f;
    public static float BasicAttackStaggerTime = 0.2f;

    public static float JumpKickDamage = 18f;
    public static float JumpKickKnockbackVelocity = 25f;
    public static float JumpKickKnockbackTime = 0.2f;

    public static float SlideKickDamage = 12f;
    public static float SlideKickKnockbackVelocity = 18f;
    public static float SlideKickKnockbackTime = 0.2f;
    #endregion

    PlayerController playerController;
    PlayerSoundManager playerSoundManager;

    BoxCollider MidPunchHitbox;
    BoxCollider JumpKickHitbox;
    BoxCollider SlideKickHitbox;

    List<IAttackable> enemiesHit = new List<IAttackable>();

    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        playerSoundManager = GetComponentInParent<PlayerSoundManager>();

        MidPunchHitbox = GameObject.FindGameObjectWithTag(Helpers.Tags.MidPunchHitbox).GetComponent<BoxCollider>();
        JumpKickHitbox = GameObject.FindGameObjectWithTag(Helpers.Tags.JumpKickHitbox).GetComponent<BoxCollider>();
        SlideKickHitbox = GameObject.FindGameObjectWithTag(Helpers.Tags.SlideKickHitbox).GetComponent<BoxCollider>();
    }

    public void BasicAttack()
    {
        bool hitEnemy = false;
        List<IAttackable> results = CheckInstantFrameHitboxForEnemies(MidPunchHitbox, out hitEnemy);

        foreach (IAttackable attackableComponent in results)
        {
            attackableComponent.ReceiveStaggerAttack(BasicAttackDamage, transform.forward, BasicAttackStaggerTime);
        }

        if (hitEnemy)
            playerSoundManager.PlayBasicAttackHitSound();
        else
            playerSoundManager.PlayBasicAttackMissSound();
    }

    public void JumpKick(IAttackable attackableComponent)
    {
        if (!enemiesHit.Contains(attackableComponent))
        {
            enemiesHit.Add(attackableComponent);
            attackableComponent.ReceiveKnockbackAttack(JumpKickDamage, transform.forward, JumpKickKnockbackVelocity, JumpKickKnockbackTime);
            playerSoundManager.PlayJumpAttackHitSound();
        }
        else
            attackableComponent.ReceiveKnockbackAttack(0f, transform.forward, JumpKickKnockbackVelocity, JumpKickKnockbackTime);
    }

    public void SlideKick(IAttackable attackableComponent)
    {
        if (!enemiesHit.Contains(attackableComponent))
        {
            enemiesHit.Add(attackableComponent);
            attackableComponent.ReceiveKnockbackAttack(SlideKickDamage, transform.forward, SlideKickKnockbackVelocity, SlideKickKnockbackTime);
            playerSoundManager.PlayJumpAttackHitSound();
        }
        else
            attackableComponent.ReceiveKnockbackAttack(0f, transform.forward, SlideKickKnockbackVelocity, SlideKickKnockbackTime);
    }

    public List<IAttackable> CheckInstantFrameHitboxForEnemies(BoxCollider hitbox, out bool hasEnemy)
    {
        Vector3 size = hitbox.size / 2;
        size.x = Mathf.Abs(size.x);
        size.y = Mathf.Abs(size.y);
        size.z = Mathf.Abs(size.z);
        ExtDebug.DrawBox(hitbox.transform.position + hitbox.transform.forward * 0.5f, size, hitbox.transform.rotation, Color.blue);
        Collider[] colliders = Physics.OverlapBox(hitbox.transform.position + hitbox.transform.forward * 0.5f, size, hitbox.transform.rotation);
        var results = new List<IAttackable>();

        foreach (Collider collider in colliders)
        {
            var attackableComponent = collider.gameObject.GetAttackableComponent();
            if (attackableComponent != null)
                results.Add(attackableComponent);
        }

        hasEnemy = results.Count > 0;
        return results;
    }

    public void ClearEnemiesHit()
    {
        enemiesHit.Clear();
    }
}

public static class PlayerAttackManagerExtensions
{
    public static IAttackable GetAttackableComponent(this GameObject gameObject)
    {
        return gameObject.GetComponentInChildren<IAttackable>();
    }
}
