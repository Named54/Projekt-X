using System;
using System.Collections;
using UnityEngine;

[Flags]
public enum PlayerStateFlags
{
    None = 0,
    Attacking = 1 << 0,
    Crouching = 1 << 1,
    Pushing = 1 << 2,
    Climbing = 1 << 3,
    Jumping = 1 << 4,
}

public class CombatSystem : MonoBehaviour
{
    public WeaponBase currentWeapon;
    public Player_Movement movement;

    public float pushDistance = 2f;
    public float pushCooldown = 1f;
    public float pushDuration = 0.2f;
    private bool isPushCooldown = false;
    [SerializeField] private float pushCooldownTimer;

    public bool canAttack = true;
    public float attackRange = 1.5f;

    public LayerMask enemyLayer;
    private PlayerStateFlags playerState = PlayerStateFlags.None;

    private int currentComboStep = 0;
    private const int maxComboStep = 2; // 0, 1, 2 für drei Schritte

    Vector2 lockedMoveDirection;

    private void Update()
    {
        HandleAttackInput();
        UpdateCooldowns();
    }

    private void HandleAttackInput()
    {
        Parry_Block_System parry_Block_System = GetComponent<Parry_Block_System>(); // Referenz auf das Parry-Block-Syste
        if (!playerState.HasFlag(PlayerStateFlags.Attacking) && !isPushCooldown)
        {
            if (Input.GetMouseButtonDown(0) && currentWeapon.CanLightAttack() && !parry_Block_System.isParrying && !parry_Block_System.isBlocking)
            {
                PerformAttack(true);
            }
            else if (Input.GetMouseButtonDown(1) && currentWeapon.CanHeavyAttack() && !parry_Block_System.isParrying && !parry_Block_System.isBlocking)
            {
                PerformAttack(false);
            }
        }
    }

    private void PerformAttack(bool isLightAttack)
    {
        StartPushCooldown();

        lockedMoveDirection = movement.moveDirection;
        bool isMoving = movement.moveDirection != Vector2.zero;

        if (isMoving || currentComboStep == maxComboStep)
        {
            DashInAttackDirection();
        }
        else
        {
            PerformStationaryAttack();
        }

        currentWeapon.Attack(isLightAttack);
        CheckEnemyCollisions(isLightAttack);

        currentComboStep = (currentComboStep + 1) % (maxComboStep + 1);
    }

    private void DashInAttackDirection()
    {
        // Für den letzten Combo-Schlag, wenn der Spieler steht
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;
        Vector3 dashDirection = (mousePosition - transform.position).normalized;

        Vector3 dashDestination = transform.position + (Vector3)dashDirection * pushDistance;
        StartCoroutine(MovePlayer(dashDestination));
    }

    private void PerformStationaryAttack()
    {
        playerState |= PlayerStateFlags.Attacking;
        StartCoroutine(StationaryAttackDelay());
    }

    private IEnumerator StationaryAttackDelay()
    {
        yield return new WaitForSeconds(pushDuration);
        playerState &= ~PlayerStateFlags.Attacking;
    }

    private IEnumerator MovePlayer(Vector3 targetPosition)
    {
        playerState |= PlayerStateFlags.Attacking | PlayerStateFlags.Pushing;// Zustände hinzufügen

        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < pushDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / pushDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        movement.moveDirection = Vector2.zero;
        // yield return new WaitForSeconds(0.5f);
        playerState &= ~(PlayerStateFlags.Attacking | PlayerStateFlags.Pushing);// Zustände entfernen
    }

    private void CheckEnemyCollisions(bool isLightAttack)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            BaseEnemies enemy = enemyCollider.GetComponent<BaseEnemies>();
            if (enemy != null)
            {
                int damage = isLightAttack ? currentWeapon.weaponStats.lightAttackDamage : currentWeapon.weaponStats.heavyAttackDamage;
                enemy.TakeDamage(damage, gameObject);
                Debug.Log($"Player attacked enemy and dealt {damage} damage! Combo step: {currentComboStep}");
            }
        }
    }

    private void StartPushCooldown()
    {
        isPushCooldown = true;
        pushCooldownTimer = pushCooldown;
    }

    private void UpdateCooldowns()
    {
        if (pushCooldownTimer > 0)
        {
            pushCooldownTimer -= Time.deltaTime;
            if (pushCooldownTimer <= 0)
            {
                isPushCooldown = false;
            }
        }

        currentWeapon.UpdateCooldowns();
    }
    public bool CanPerformAction(PlayerStateFlags requiredState)
    {
        return !playerState.HasFlag(requiredState);
    }
    public void SwitchWeapon(WeaponBase newWeapon)
    {
        Debug.Log($"Switched weapon to {newWeapon.name}");
        currentWeapon = newWeapon;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}