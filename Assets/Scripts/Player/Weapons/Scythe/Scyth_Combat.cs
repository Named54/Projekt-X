using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scyth_Combat : WeaponBase
{
    [System.Serializable]
    public class ScythCombatStats
    {
        public int maxLightComboStep;
        public int maxHeavyComboStep;
        public float lightComboTimer;
        public float heavyComboTimer;
    }

    public ScythCombatStats scythStats;

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
        if (lightComboStep > scythStats.maxLightComboStep)
        {
            lightComboStep = 1;
        }
        lightComboTimerCurrent = scythStats.lightComboTimer;
    }

    private void PerformHeavyCombo()
    {
        heavyComboStep++;
        if (heavyComboStep > scythStats.maxHeavyComboStep)
        {
            heavyComboStep = 1;
        }
        heavyComboTimerCurrent = scythStats.heavyComboTimer;
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
