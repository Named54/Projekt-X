using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Player_health : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image healthBar;

    [Header("Health Settings")]
    public int maxHealth;
    private int currentHealth;

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;

    private bool isKnockedBack = false;

    [Header("Stun Settings")]
    public float stunDuration;
    
    private Rigidbody2D rb;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int Damage, GameObject causer)
    {
        currentHealth -= Damage;
        ApplyKnockback(causer.transform.position);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void ApplyKnockback(Vector3 sourcePosition)
    {
        if (rb != null && !isKnockedBack)
        {
            StartCoroutine(KnockbackCoroutine(sourcePosition));
        }
    }

    private IEnumerator KnockbackCoroutine(Vector3 sourcePosition)
    {
        isKnockedBack = true;
        Vector2 knockbackDirection = (transform.position - sourcePosition).normalized;
        rb.velocity = knockbackDirection * knockbackForce;

        yield return new WaitForSeconds(knockbackDuration);

        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
    public virtual void Heal(int amount)
    {
        //Gibt Denn Player HP zurück
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }
}
