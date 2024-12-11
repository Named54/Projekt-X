using UnityEngine;
using System.Collections;

public abstract class ElementBase : MonoBehaviour
{
    [System.Serializable]
    public class ElementStats
    {
        public float elementDamage = 10f;
        public float elementEffectStrength = 5f;
        public float criticalChance = 1.5f;
        public float cooldownElementAttack = 0.5f;
        public float maxElementEffect = 100f;
    }

    public ElementStats elementStats;

    protected bool canElementAttack = true;
    protected float elementAttackCooldownTimer;

    public abstract void ApplyElementalEffect(GameObject target);

    protected IEnumerator ElementAttackRoutine()
    {
        canElementAttack = false;
        elementAttackCooldownTimer = elementStats.cooldownElementAttack;

        yield return new WaitForSeconds(0.1f); // Kurze Verzögerung vor dem Angriff

        PerformElementAttack();

        yield return new WaitForSeconds(0.2f); // Angriffsdauer
    }

    protected virtual void PerformElementAttack()
    {
        float damage = elementStats.elementDamage;
        bool isCritical = Random.Range(0, 100) < elementStats.criticalChance;
        if (isCritical) damage *= 2;

        Debug.Log($"Element attack performed. Damage: {damage}, Is Critical: {isCritical}");
    }

    public virtual void UpdateCooldowns()
    {
        if (elementAttackCooldownTimer > 0) elementAttackCooldownTimer -= Time.deltaTime;

        if (elementAttackCooldownTimer <= 0) canElementAttack = true;
    }

    public bool CanElementAttack() => canElementAttack && elementAttackCooldownTimer <= 0;

}
