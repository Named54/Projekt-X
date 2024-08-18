using System.Collections;
using UnityEngine;

public class Fist_Combat : WeaponBase
{
    [System.Serializable]
    public class FistCombatStats
    {
        public int maxLightComboStep;
        public int maxHeavyComboStep;
        public float lightComboTimer;
        public float heavyComboTimer;
    }

    public FistCombatStats fistStats;

    private int lightComboStep = 1;
    private int heavyComboStep = 1;
    private float lightComboTimerCurrent;
    private float heavyComboTimerCurrent;

    public override void Attack(bool isLightAttack)
    {
        StartCoroutine(FistAttackRoutine(isLightAttack));
    }

    private IEnumerator FistAttackRoutine(bool isLightAttack)
    {
        yield return StartCoroutine(AttackRoutine(isLightAttack));

        if (isLightAttack)
        {
            PerformLightCombo();
        }
        else
        {
            PerformHeavyCombo();
        }
    }

    protected override void PerformAttack(bool isLightAttack)
    {
        base.PerformAttack(isLightAttack);

        if (isLightAttack)
        {
            Debug.Log($"Light Fist Attack! Combo Step: {lightComboStep}");
        }
        else
        {
            Debug.Log($"Heavy Fist Attack! Combo Step: {heavyComboStep}");
        }

        // Hier können Sie zusätzliche Faust-spezifische Angriffslogik hinzufügen
    }

    private void PerformLightCombo()
    {
        lightComboStep++;
        if (lightComboStep > fistStats.maxLightComboStep)
        {
            lightComboStep = 1;
        }
        lightComboTimerCurrent = fistStats.lightComboTimer;
    }

    private void PerformHeavyCombo()
    {
        heavyComboStep++;
        if (heavyComboStep > fistStats.maxHeavyComboStep)
        {
            heavyComboStep = 1;
        }
        heavyComboTimerCurrent = fistStats.heavyComboTimer;
    }

    public override void UpdateCooldowns()
    {
        base.UpdateCooldowns();
        if (lightComboTimerCurrent > 0) lightComboTimerCurrent -= Time.deltaTime;
        if (heavyComboTimerCurrent > 0) heavyComboTimerCurrent -= Time.deltaTime;
        if (lightComboTimerCurrent <= 0) lightComboStep = 0;
        if (heavyComboTimerCurrent <= 0) heavyComboStep = 0;
    }

    // Methoden für freigeschaltete Fähigkeiten
    public void UnlockComboAttack()
    {
        fistStats.maxLightComboStep = 5; // Beispiel: Erweitern Sie die maximale Kombo auf 5
    }

    public void UnlockPowerPunch()
    {
        weaponStats.heavyAttackDamage *= 2; // Beispiel: Erhöhen Sie den Schaden des schweren Angriffs
    }
}