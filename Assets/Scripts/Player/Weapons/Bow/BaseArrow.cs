using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseArrow : MonoBehaviour
{
    public float arrowDamage;
    public Bow_Combat bowCombat;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Players"))
        {
            return;
        }

        BaseEnemies enemy = collision.GetComponent<BaseEnemies>();
        if (enemy != null)
        {
            enemy.TakeDamage(Mathf.RoundToInt(arrowDamage), bowCombat.gameObject);

            // Füge Knockback hinzu
            KnockbackReceiver knockbackReceiver = enemy.GetComponent<KnockbackReceiver>();
            if (knockbackReceiver != null)
            {
                knockbackReceiver.ApplyKnockback(transform.position);
            }
        }

        Destroy(gameObject);
    }
}
