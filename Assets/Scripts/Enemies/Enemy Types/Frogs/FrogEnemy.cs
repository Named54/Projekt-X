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

    [Header("Jump Attack Settings")]
    public float jumpPrepareTime = 2f;
    public float minJumpSpeed = 5f;
    public float maxJumpSpeed = 8f;
    public float maxJumpDistance = 10f;

    private Vector2 targetJumpPosition;
    private bool isPreparing = false;

    // Update wird einmal pro Frame aufgerufen
    protected override void Update()
    {
        base.Update(); // Ruft die Update-Methode der Basisklasse auf

        // Wenn der Spieler erkannt wurde, der FrogEnemy nicht springt, nicht in einer Abklingzeit ist und nicht zurückgeschlagen wird, startet der FrogEnemy einen Sprungangriff
        if (isPlayerDetected && !isJumping && !isOnCooldown && !isPreparing)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= detectionRange)
            {
                StartCoroutine(PrepareAndJump());
            }
        }
    }

    // Bewegt den FrogEnemy langsam zum Spieler
    protected override void MoveTowardsPlayer()
    {
        if (player != null && !isJumping && !isOnCooldown)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * Time.deltaTime * jumpSpeed * 0.5f);
        }
    }
    private IEnumerator PrepareAndJump()
    {
        isPreparing = true;

        // Zeige Vorbereitung an (z.B. durch Animation oder visuellen Effekt)
        // TODO: Fügen Sie hier Code für die Vorbereitungsanzeige hinzu
        Debug.Log("FrogEnemy is preparing to jump!");

        yield return new WaitForSeconds(jumpPrepareTime);

        // Speichere die letzte bekannte Position des Spielers
        targetJumpPosition = player.position;

        StartCoroutine(PerformJumpAttack());
        isPreparing = false;
    }

    // Sprungangriff des FrogEnemy
    private IEnumerator PerformJumpAttack()
    {
        
        isJumping = true;
        Vector2 jumpStartPosition = transform.position;
        Vector2 jumpDirection = (targetJumpPosition - jumpStartPosition).normalized;
        float jumpDistance = Vector2.Distance(jumpStartPosition, targetJumpPosition);

        // Berechne die Sprunggeschwindigkeit basierend auf der Entfernung
        float jumpSpeed = Mathf.Lerp(minJumpSpeed, maxJumpSpeed, jumpDistance / maxJumpDistance);

        float jumpDuration = jumpDistance / jumpSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            float t = elapsedTime / jumpDuration;

            // Verwende eine Parabel für die Sprunghöhe
            float height = Mathf.Sin(t * Mathf.PI) * 2f;

            Vector2 newPosition = Vector2.Lerp(jumpStartPosition, targetJumpPosition, t);
            newPosition.y += height;

            rb.MovePosition(newPosition);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(targetJumpPosition);
        isJumping = false;

        // Überprüfe Kollision mit dem Spieler
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
            }
        }

        // Starte Abklingzeit
        StartCoroutine(AttackCooldown());
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
    protected override void ResetEnemyState()
      {
          base.ResetEnemyState();
          isJumping = false;
          isOnCooldown = false;
          isPreparing = false;
          StopAllCoroutines();
          StartCoroutine(SearchForPlayerAfterDelay());
      }

      // Override SearchForPlayerAfterDelay to include jump attack
      protected new IEnumerator SearchForPlayerAfterDelay()
      {
          yield return new WaitForSeconds(1f);
          FindPlayer();
          if (player != null)
          {
              StartCoroutine(PrepareAndJump());
          }
      }
    // Überschreibe die OnDrawGizmosSelected-Methode, um den Angriffsradius und die Zielposition anzuzeigen
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (isPreparing)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetJumpPosition);
            Gizmos.DrawWireSphere(targetJumpPosition, 0.5f);
        }
    }
}