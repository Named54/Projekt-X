using System.Collections;
using UnityEngine;

public class FrogEnemy : BaseEnemies
{
    [Header("Dash Settings")]
    public float jumpSpeed = 10f; // Geschwindigkeit des Sprungangriffs
    public float jumpCooldown = 2f; // Abklingzeit zwischen Sprüngen
    public float jumpDuration = 0.5f; // Dauer des Sprungs
    
    private bool isJumping = false; // Gibt an, ob der Frosch gerade springt
    private bool isOnCooldown = false; // Gibt an, ob der Sprung abkühlt

    [Header("Attack Settings")]
    public int attackDamage = 2; // Schaden, den der Feind dem Spieler zufügt
    public float attackCooldown = 2f; // Abklingzeit zwischen Angriffen


    // Update wird einmal pro Frame aufgerufen
    protected override void Update()
    {
        base.Update(); // Ruft die Update-Methode der Basisklasse auf

        // Wenn der Spieler erkannt wurde, der FrogEnemy nicht springt, nicht in einer Abklingzeit ist und nicht zurückgeschlagen wird, startet der FrogEnemy einen Sprungangriff
        if (isPlayerDetected && !isJumping && !isOnCooldown && !isKnockedback)
        {
            StartCoroutine(PerformJumpAttack());
        }
    }

    // Bewegt den FrogEnemy langsam zum Spieler
    protected override void MoveTowardsPlayer()
    {
        if (player != null && !isJumping && !isOnCooldown && !isKnockedback)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * Time.deltaTime * jumpSpeed * 0.5f);
        }
    }

    // Sprungangriff des FrogEnemy
    private IEnumerator PerformJumpAttack()
    {
        isJumping = true;
        Vector2 jumpDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
        float jumpStartTime = Time.time;

        while (Time.time < jumpStartTime + jumpDuration)
        {
            rb.velocity = jumpDirection * jumpSpeed;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        isJumping = false;

        // Check if player is hit
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, attackRange, LayerMask.GetMask("Player"));
        if (playerCollider != null)
        {
            Player_health playerHealth = playerCollider.GetComponent<Player_health>();
            Parry_Block_System playerBlockSystem = playerCollider.GetComponent<Parry_Block_System>();

            if (playerHealth != null && playerBlockSystem != null)
            {
                if (playerBlockSystem.isParrying)
                {
                    TakeDamage(playerBlockSystem.parryDamage, playerCollider.gameObject);
                    Debug.Log("Enemy parried!");
                }
                else if (!playerBlockSystem.TryBlock(attackDamage, gameObject))
                {
                    playerHealth.TakeDamage(attackDamage, gameObject);
                    Debug.Log($"FrogEnemy hits player and deals {attackDamage} damage!");
                }
                else
                {
                    Debug.Log("Attack blocked!");
                }
                StartCoroutine(AttackCooldown());
            }
        }

        // Start cooldown
        isOnCooldown = true;
        yield return new WaitForSeconds(jumpCooldown);
        isOnCooldown = false;
    }

    // Überschreibt die TakeDamage-Methode der Basisklasse
    public override void TakeDamage(int damage, GameObject causer)
    {
        base.TakeDamage(damage, causer);
        Debug.Log($"FrogEnemy nimmt {damage} Schaden!");
        
        // Zusätzliche Logik für den FrogEnemy beim Schaden nehmen
        // Zum Beispiel: Sprung abbrechen, wenn er gerade springt
        if (isJumping)
        {
            isJumping = false;
            rb.velocity = Vector2.zero;
        }
    }
    private IEnumerator AttackCooldown()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    // Zeichnet eine blaue Kugel um den FrogEnemy, die die Reichweite des Sprungangriffs darstellt
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, jumpSpeed * jumpDuration);
    }
}