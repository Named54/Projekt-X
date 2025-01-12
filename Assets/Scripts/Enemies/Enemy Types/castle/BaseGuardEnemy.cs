using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGuardEnemy : BaseEnemies
{
    [Header("Attack Settings")]
    public int attackDamage = 2;
    public float attackCooldown = 2f;
    private bool isOnCooldown = false;
    public int comboHits = 3;
    public float comboCooldown = 0.5f;
    private int currentComboHit = 0;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Player Reward")]
    public int healthReward = 10;
    public int staminaReward = 15;

    protected override void Update()
    {
        base.Update();
        if (isPlayerDetected && !isAttacking && !isOnCooldown)
        {
            MoveTowardsPlayer();
        }
    }

    protected override void CheckPlayerPosition()
    {
        base.CheckPlayerPosition();
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (!isPlayerDetected && distanceToPlayer <= detectionRange)
            {
                isPlayerDetected = true;
            }
            else if (isPlayerDetected && distanceToPlayer > exitRange)
            {
                isPlayerDetected = false;
            }
        }
    }

    protected override void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange)
            {
                rb.linearVelocity = Vector2.zero;
                StartCoroutine(PerformAttackCombo());
            }
        }
    }

    private IEnumerator PerformAttackCombo()
    {
        isAttacking = true;
        isOnCooldown = true;

        for (int i = 0; i < comboHits; i++)
        {
            currentComboHit = i + 1;
            CheckPlayerCollision();
            yield return new WaitForSeconds(comboCooldown);
        }

        yield return new WaitForSeconds(attackCooldown - (comboHits * comboCooldown));

        isOnCooldown = false;
        isAttacking = false;
    }

    public override void CheckPlayerCollision()
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
                    int damage = CalculateAttackDamage();
                    playerHealth.TakeDamage(damage, gameObject);
                    Debug.Log($"Enemy hits player with combo hit {currentComboHit} and deals {damage} damage!");
                }
                else
                {
                    Debug.Log("Attack blocked!");
                }
            }
        }
    }

    private int CalculateAttackDamage()
    {
        return attackDamage + (currentComboHit - 1);
    }

    public override void TakeDamage(int damage, GameObject causer)
    {
        base.TakeDamage(damage, causer);
        ResetEnemyState();
    }

    protected override void ResetEnemyState()
    {
        base.ResetEnemyState();
        isOnCooldown = false;
        isAttacking = false;
        rb.linearVelocity = Vector2.zero;
    }

    protected override void Die()
    {
        if (player != null)
        {
            Player_health playerHealth = player.GetComponent<Player_health>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healthReward);
            }

            PlayerMovement playerStamina = player.GetComponent<PlayerMovement>();
            if (playerStamina != null)
            {
                playerStamina.RestoreStamina(staminaReward);
            }
        }

        base.Die();
    }
}