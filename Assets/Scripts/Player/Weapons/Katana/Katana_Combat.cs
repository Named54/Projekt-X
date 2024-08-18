using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana_Combat : WeaponBase
{
    [System.Serializable]
    public class KatanaCombatStats
    {
        public int maxLightComboStep;
        public int maxHeavyComboStep;
        public float lightComboTimer;
        public float heavyComboTimer;
    }

    public KatanaCombatStats katanaStats;

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

        // Hier k�nnen Sie zus�tzliche Faust-spezifische Angriffslogik hinzuf�gen
    }

    private void PerformLightCombo()
    {
        lightComboStep++;
        if (lightComboStep > katanaStats.maxLightComboStep)
        {
            lightComboStep = 1;
        }
        lightComboTimerCurrent = katanaStats.lightComboTimer;
    }

    private void PerformHeavyCombo()
    {
        heavyComboStep++;
        if (heavyComboStep > katanaStats.maxHeavyComboStep)
        {
            heavyComboStep = 1;
        }
        heavyComboTimerCurrent = katanaStats.heavyComboTimer;
    }

    public override void UpdateCooldowns()
    {
        base.UpdateCooldowns();
        if (lightComboTimerCurrent > 0) lightComboTimerCurrent -= Time.deltaTime;
        if (heavyComboTimerCurrent > 0) heavyComboTimerCurrent -= Time.deltaTime;
        if (lightComboTimerCurrent <= 0) lightComboStep = 0;
        if (heavyComboTimerCurrent <= 0) heavyComboStep = 0;
    }
}
