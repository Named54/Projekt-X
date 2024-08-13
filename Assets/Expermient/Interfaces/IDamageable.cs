using UnityEngine;
public interface IDamageable 
{
    void Die();
    //bool IsStunned();
    //bool IsAlive {get; }
    //void ApplyKnockback(GameObject causer);
    void TakeDamage(int damageAmount, GameObject causer);
    //void StunPlayer(float duration, bool _override = false);

    float maxHealth { get; set;}
    float currentHealth {  get; set; }

}
