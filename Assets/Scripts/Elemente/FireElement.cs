using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireElement : ElementBase
{
    public override void ApplyElementalEffect(GameObject target)
    {
        StartCoroutine(FireAttackRoutine(target));
    }

    private IEnumerator FireAttackRoutine(GameObject target)
    {
        yield return StartCoroutine(ElementAttackRoutine());

        // Fügen Sie hier die spezifische Feuer-Elementlogik hinzu
        Debug.Log($"Fire effect applied to {target.name}");
        // Beispiel: Fügen Sie dem Ziel einen "Burning"-Status hinzu
    }

    protected override void PerformElementAttack()
    {
        base.PerformElementAttack();
        Debug.Log("Fire Attack performed!");
        // Hier können Sie zusätzliche Feuer-spezifische Angriffslogik hinzufügen
    }

}
