using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackReceiver : MonoBehaviour
{
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void ApplyKnockback(Vector2 sourcePosition)
    {
        if (!isKnockedBack)
        {
            StartCoroutine(PerformKnockback(sourcePosition));
        }
    }

    private IEnumerator PerformKnockback(Vector2 sourcePosition)
    {
        isKnockedBack = true;
        Vector2 knockbackDirection = (transform.position - (Vector3)sourcePosition).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        rb.WakeUp();
        //rb.velocity = Vector2.up * 100000f;
        Debug.Log($"Applied knockback {knockbackDirection * knockbackForce} on object {gameObject.name} for {knockbackDuration} seconds. (rb name: {rb.name})");

        yield return new WaitForSeconds(knockbackDuration);

        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }
}
