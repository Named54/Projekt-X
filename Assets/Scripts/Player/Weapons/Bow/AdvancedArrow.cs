using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AdvancedArrow : MonoBehaviour
{
    public float speed;
    public float damage;
    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void Initialize(Vector2 direction, float damage, float speed, float rotation)
    {
        this.damage = damage;
        this.speed = speed;
        rb.velocity = direction * speed;
        transform.rotation.SetFromToRotation(transform.position, transform.position + new Vector3(direction.x, direction.y, 0));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Players")) // Überprüfe, ob das Ziel der Spieler ist
        {
            return; // Verlasse die Methode, wenn der Spieler getroffen wurde, ohne etwas zu tun
        }
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
            // Pfeil hat etwas anderes getroffen, zerstöre ihn
            Destroy(gameObject);
        }
    }
}
