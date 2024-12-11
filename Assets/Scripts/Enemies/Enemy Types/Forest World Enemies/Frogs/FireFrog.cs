using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFrog : FrogEnemy
{
    [Header("Elemental Settings")]
    public ElementBase enemyElement;
    public float elementalEffectStrength = 10f;
    public float maxElementalEffect = 100f;

    private float currentElementalEffect = 0f;
    protected override void Start()
    {
        base.Start();
        if (enemyElement == null)
        {
            Debug.LogError("Enemy element not set for ElementalFrogEnemy!");
        }
    }
    protected override void CheckPlayerCollision()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, attackRange, LayerMask.GetMask("Player"));
        if (playerCollider != null)
        {
            Player_health playerHealth = playerCollider.GetComponent<Player_health>();
            ElementSystem playerElementSystem = playerCollider.GetComponent<ElementSystem>();
            Parry_Block_System playerBlockSystem = playerCollider.GetComponent<Parry_Block_System>();

            if (playerHealth != null && playerBlockSystem != null && playerElementSystem != null)
            {
                if (playerBlockSystem.isParrying)
                {
                    TakeDamage(playerBlockSystem.parryDamage, playerCollider.gameObject);
                    Debug.Log("Enemy parried!");
                }
                else if (!playerBlockSystem.TryBlock(attackDamage, gameObject))
                {
                    playerHealth.TakeDamage(attackDamage, gameObject);
                    ApplyElementalEffect(playerElementSystem);
                    Debug.Log($"ElementalFrogEnemy hits player, deals {attackDamage} damage, and applies elemental effect!");
                }
                else
                {
                    Debug.Log("Attack blocked!");
                }
            }
        }
    }

    private void ApplyElementalEffect(ElementSystem playerElementSystem)
    {
        if (enemyElement != null)
        {
            currentElementalEffect += elementalEffectStrength;
            currentElementalEffect = Mathf.Min(currentElementalEffect, maxElementalEffect);

            // Hier können Sie die Logik für die Anwendung des Elementareffekts implementieren
            // Zum Beispiel:
            enemyElement.ApplyElementalEffect(playerElementSystem.gameObject);

            if (currentElementalEffect >= maxElementalEffect)
            {
                TriggerMaxElementalEffect(playerElementSystem);
            }

            Debug.Log($"Applied {enemyElement.GetType().Name} effect. Current effect: {currentElementalEffect}/{maxElementalEffect}");
        }
    }

    private void TriggerMaxElementalEffect(ElementSystem playerElementSystem)
    {
        // Hier implementieren Sie die Logik für den maximalen Elementareffekt
        // Zum Beispiel für ein Feuer-Element:
        if (enemyElement is FireElement)
        {
            Debug.Log("Player is now on fire!");
            // Fügen Sie hier die Logik hinzu, um den Spieler "anzuzünden"
            // z.B. einen "Burning"-Status hinzufügen oder eine spezielle Methode im PlayerElementSystem aufrufen
        }

        // Setzen Sie den Effekt zurück
        currentElementalEffect = 0f;
    }
}
