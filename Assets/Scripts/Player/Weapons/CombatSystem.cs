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
    public PlayerMovement movement;

    [Header("Push Settings")]
    public float pushDistance = 2f;
    public float pushCooldown = 1f;
    public float pushDuration = 0.2f;
    private bool isPushCooldown = false;
    [SerializeField] private float pushCooldownTimer;

    [Header("Attack Settings")]
    public bool canAttack = true;
    public float attackRange = 1.5f;
    private bool isBowEquipped = false;

    public LayerMask enemyLayer;
    private PlayerStateFlags playerState = PlayerStateFlags.None;

    [Header("Combo Settings")]
    private int currentComboStep = 0;
    private const int maxComboStep = 2;
    public float comboResetTime = 1.5f;
    private float lastAttackTime;

    public Vector2 lockedMoveDirection;

    private WeaponWheelController weaponWheel;
    private Parry_Block_System parryBlockSystem;

    private void Start()
    {
        weaponWheel = FindFirstObjectByType<WeaponWheelController>();
        parryBlockSystem = GetComponent<Parry_Block_System>();
    }

    private void Update()
    {
        if (weaponWheel != null && weaponWheel.IsWeaponWheelOpen())
        {
            return;
        }
        HandleAttackInput();
        UpdateCooldowns();
        CheckComboReset();
    }

    private void HandleAttackInput()
    {
        if (playerState.HasFlag(PlayerStateFlags.Attacking) || isPushCooldown || parryBlockSystem.isParrying || parryBlockSystem.isBlocking)
        {
            return;
        }

        if (InputManager.GetMouseButtonDown(0) && currentWeapon.CanLightAttack())
        {
            PerformAttack(true);
        }
        else if (InputManager.GetMouseButtonDown(1) && currentWeapon.CanHeavyAttack())
        {
            PerformAttack(false);
        }
    }

    private void PerformAttack(bool isLightAttack)
    {
        StartPushCooldown();
        lockedMoveDirection = movement.moveDirection;
        bool isMoving = movement.moveDirection != Vector2.zero;

        if (!isBowEquipped && (isMoving || currentComboStep == maxComboStep))
        {
            StartCoroutine(PerformDashAttack(isLightAttack));
        }
        else
        {
            PerformStationaryAttack(isLightAttack);
        }

        lastAttackTime = Time.time;
        currentComboStep = (currentComboStep + 1) % (maxComboStep + 1);
    }

    private IEnumerator PerformDashAttack(bool isLightAttack)
    {
        playerState |= PlayerStateFlags.Attacking | PlayerStateFlags.Pushing;
        movement.moveDirection = Vector2.zero;

        Vector3 dashDirection = GetDashDirection();
        Vector3 dashDestination = transform.position + dashDirection * pushDistance;

        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < pushDuration)
        {
            transform.position = Vector3.Lerp(startPosition, dashDestination, elapsedTime / pushDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = dashDestination;
        currentWeapon.Attack(isLightAttack);
        CheckEnemyCollisions(isLightAttack);

        yield return new WaitForSeconds(0.325f);
        playerState &= ~(PlayerStateFlags.Attacking | PlayerStateFlags.Pushing);
    }

    private void PerformStationaryAttack(bool isLightAttack)
    {
        playerState |= PlayerStateFlags.Attacking;
        currentWeapon.Attack(isLightAttack);
        CheckEnemyCollisions(isLightAttack);
        StartCoroutine(StationaryAttackDelay());
    }

    private IEnumerator StationaryAttackDelay()
    {
        yield return new WaitForSeconds(pushDuration);
        playerState &= ~PlayerStateFlags.Attacking;
    }

    private Vector3 GetDashDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;
        return (mousePosition - transform.position).normalized;
    }

    private void CheckEnemyCollisions(bool isLightAttack)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        int damage = isLightAttack ? currentWeapon.weaponStats.lightAttackDamage : currentWeapon.weaponStats.heavyAttackDamage;

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.TryGetComponent(out BaseEnemies enemy))
            {
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
            isPushCooldown = pushCooldownTimer > 0;
        }

        currentWeapon.UpdateCooldowns();
    }

    private void CheckComboReset()
    {
        if (Time.time - lastAttackTime > comboResetTime)
        {
            currentComboStep = 0;
        }
    }

    public bool CanPerformAction(PlayerStateFlags requiredState)
    {
        return !playerState.HasFlag(requiredState);
    }

    public void SwitchWeapon(WeaponBase newWeapon)
    {
        currentWeapon = newWeapon;
        isBowEquipped = newWeapon is Bow_Combat;
        Debug.Log($"Switched weapon to {newWeapon.name}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}