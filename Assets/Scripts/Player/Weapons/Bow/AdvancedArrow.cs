using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AdvancedArrow : MonoBehaviour
{
    public float damage;
    public float speed;
    public float gravityScale = 5.5f;
    public float lifetimeAfterCollision = 5f;

    private Rigidbody2D rb;
    private bool hasCollided = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction, float damage, float speed, float rotation)
    {
        this.damage = damage;
        this.speed = speed;
        rb.velocity = direction * speed;
        rb.gravityScale = 0;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotation);
    }

    private void Update()
    {
        if (!hasCollided)
        {
            // Apply gravity effect
            rb.velocity += Vector2.down * gravityScale * Time.deltaTime;

            // Update arrow rotation to match its velocity
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasCollided) return;
        if (collision.CompareTag("Players") && hasCollided)
        {
            // Player can pick up the arrow
            Destroy(gameObject);
            // Here you can add logic to increase the player's arrow count
        }
        hasCollided = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        BaseEnemies enemy = collision.gameObject.GetComponent<BaseEnemies>();
        if (enemy != null)
        {
            enemy.TakeDamage(Mathf.RoundToInt(damage), gameObject);
            // Füge Knockback hinzu
            KnockbackReceiver knockbackReceiver = enemy.GetComponent<KnockbackReceiver>();
            if (knockbackReceiver != null)
            {
                knockbackReceiver.ApplyKnockback(transform.position);
            }
            Destroy(gameObject);
        }
        else
        {
            // Arrow has hit something that's not an enemy
            StartCoroutine(DestroyAfterDelay(lifetimeAfterCollision));
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
