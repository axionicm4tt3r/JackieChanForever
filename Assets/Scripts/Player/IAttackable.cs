public interface IAttackable
{
    void ReceiveAttack(float damage);
    void ReceiveStaggerAttack(float damage, float staggerTime);
    void ReceiveKnockbackAttack(float damage, UnityEngine.Vector3 knockbackDirection, float knockbackVelocity, float knockbackTime);
}