using System.Collections;
using UnityEngine;

public class FrogEnemy : BaseEnemies
{
    [Header("Jump Settings")]
    public float jumpSpeed = 10f; // Geschwindigkeit des Sprungangriffs
    public float jumpPrepareTime = 2f;

    private bool isJumping = false; // Gibt an, ob der Frosch gerade springt
    private bool isOnCooldown = false; // Gibt an, ob der Sprung abkühlt

    [Header("Attack Settings")]
    public int attackDamage = 2; // Schaden, den der Feind dem Spieler zufügt
    public float attackCooldown = 2f; // Abklingzeit zwischen Angriffen


    private Vector2 targetJumpPosition;
    private bool isPreparing = false;

    // Update wird einmal pro Frame aufgerufen
    protected override void Update()
    {
        base.Update(); // Ruft die Update-Methode der Basisklasse auf
        CheckPlayerPosition();
    }

    protected override void CheckPlayerPosition()
    {
        base.CheckPlayerPosition();
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Wenn der Spieler erkannt wurde und innerhalb des Exitradius ist
        if (isPlayerDetected && distanceToPlayer <= exitRange)
        {
            // Wenn der Spieler innerhalb des Detektionsradius ist, springt der FrogEnemy normal zum Spieler
            if (distanceToPlayer <= detectionRange)
            {
                if (!isJumping && !isOnCooldown && !isPreparing)
                {
                    StartCoroutine(PrepareAndJump());
                }
            }
            // Wenn der Spieler außerhalb des Detektionsradius, aber innerhalb des Exitradius ist, springt der FrogEnemy zum Rand des Detektionsradius in Richtung des Spielers
            else
            {
                if (!isJumping && !isOnCooldown && !isPreparing)
                {
                    Vector2 targetPosition = transform.position + (player.position - transform.position).normalized * detectionRange;
                    StartCoroutine(PrepareAndJumpToPosition(targetPosition));
                }
            }
        }
        // Wenn der Spieler außerhalb des Exitradius ist, verliert der FrogEnemy ihn aus den Augen
        else if (distanceToPlayer > exitRange)
        {
            isPlayerDetected = false;
        }
    }
    private IEnumerator PrepareAndJumpToPosition(Vector2 targetPosition)
    {
        isPreparing = true;

        // Zeige Vorbereitung an (z.B. durch Animation oder visuellen Effekt)
        Debug.Log("FrogEnemy is preparing to jump to position!");

        yield return new WaitForSeconds(jumpPrepareTime);

        targetJumpPosition = targetPosition;
        StartCoroutine(PerformJumpAttack());
        isPreparing = false;
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

        // Berechne die Sprunggeschwindigkeit basierend auf der Entfernung
        float jumpDistance = Vector2.Distance(jumpStartPosition, targetJumpPosition);
        float jumpDuration = jumpDistance / jumpSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            float t = elapsedTime / jumpDuration;

            // Verwende eine Parabel für die Sprunghöhe
            float height = Mathf.Sin(t * Mathf.PI) * 2f;

            Vector2 newPosition = Vector2.Lerp(jumpStartPosition, targetJumpPosition, t);
            newPosition.y += height;

            rb.position = newPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rb.position = targetJumpPosition;
        isJumping = false;

        // Überprüfe Kollision mit dem Spieler
        CheckPlayerCollision();

        // Starte Abklingzeit
        StartCoroutine(AttackCooldown());
    }
    private void CheckPlayerCollision()
    {
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
    }
}