using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Player_health : MonoBehaviour
{
    private KnockbackReceiver knockbackReceiver;

    [Header("UI")]
    [SerializeField] private Image healthBar;

    [Header("Health Settings")]
    public int maxHealth;
    private int currentHealth;

    [Header("Stun Settings")]
    public float stunDuration;

    private void Start()
    {
        currentHealth = maxHealth;
        knockbackReceiver = GetComponent<KnockbackReceiver>();
    }

    public void TakeDamage(int Damage, GameObject causer)
    {
        currentHealth -= Damage;
        UpdateHealthBar();
        knockbackReceiver.ApplyKnockback(causer.transform.position);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }
    private void Die()
    {
        Destroy(gameObject);
    }
    public virtual void Heal(int amount)
    {
        //Gibt Denn Player HP zurück
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHealthBar();
    }
}
