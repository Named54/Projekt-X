using UnityEngine;
using System.Collections;

public abstract class WeaponBase : MonoBehaviour
{
    [System.Serializable]
    public class WeaponStats
    {
        public int lightAttackDamage = 10;
        public int heavyAttackDamage = 20;
        public float criticalChance = 10f;
        public float cooldownLightAttack = 0.5f;
        public float cooldownHeavyAttack = 1f;
    }

    public WeaponStats weaponStats;

    protected bool canLightAttack = true;
    protected bool canHeavyAttack = true;
    protected float lightAttackCooldownTimer;
    protected float heavyAttackCooldownTimer;

    public abstract void Attack(bool isLightAttack);

    protected IEnumerator AttackRoutine(bool isLightAttack)
    {
        if (isLightAttack)
        {
            canLightAttack = false;
            lightAttackCooldownTimer = weaponStats.cooldownLightAttack;
        }
        else
        {
            canHeavyAttack = false;
            heavyAttackCooldownTimer = weaponStats.cooldownHeavyAttack;
        }

        yield return new WaitForSeconds(0.1f); // Kurze Verzögerung vor dem Angriff

        PerformAttack(isLightAttack);

        yield return new WaitForSeconds(0.2f); // Angriffsdauer
    }

    protected virtual void PerformAttack(bool isLightAttack)
    {
        // Basis-Implementierung des Angriffs
        int damage = isLightAttack ? weaponStats.lightAttackDamage : weaponStats.heavyAttackDamage;
        bool isCritical = Random.Range(0, 100) < weaponStats.criticalChance;
        if (isCritical) damage *= 2;

        Debug.Log($"Weapon attack performed. Damage: {damage}, Is Critical: {isCritical}");
    }

    public virtual void UpdateCooldowns()
    {
        if (lightAttackCooldownTimer > 0) lightAttackCooldownTimer -= Time.deltaTime;
        if (heavyAttackCooldownTimer > 0) heavyAttackCooldownTimer -= Time.deltaTime;

        if (lightAttackCooldownTimer <= 0) canLightAttack = true;
        if (heavyAttackCooldownTimer <= 0) canHeavyAttack = true;
    }

    public bool CanLightAttack() => canLightAttack && lightAttackCooldownTimer <= 0;
    public bool CanHeavyAttack() => canHeavyAttack && heavyAttackCooldownTimer <= 0;
}