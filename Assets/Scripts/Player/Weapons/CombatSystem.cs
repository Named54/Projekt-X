using System.Collections;
using UnityEngine;
public enum PlayerState
{
    Normal,
    Attacking,
    Dashing
}
public class CombatSystem : MonoBehaviour
{
    public WeaponBase currentWeapon; // Aktuelle Waffe des Spielers
    public Player_Movement movement; // Referenz auf die Bewegungskomponente des Spielers

    public float pushDistance = 2f;
    public float pushCooldown = 1f;
    public float pushDuration = 0.2f;
    private bool isPushCooldown = false; // Gibt an, ob der Schub-Cooldown aktiv ist
    [SerializeField] private float pushCooldownTimer; // Timer für den Schub-Cooldown

    public bool canAttack = true; // Gibt an, ob der Spieler angreift kann
    public float attackRange = 1.5f; // Reichweite des Angriffs
    
    public LayerMask enemyLayer; // Layer, auf dem sich Feinde befinden
    private PlayerState playerState = PlayerState.Normal; // Aktueller Zustand des Spielers

    private void Update()
    {
        HandleAttackInput();
        UpdateCooldowns();
    }

    private void HandleAttackInput()
    {
        if (playerState == PlayerState.Normal && !isPushCooldown)
        {
            if (Input.GetMouseButtonDown(0) && currentWeapon.CanLightAttack())
            {
                PerformAttack(true);
            }
            else if (Input.GetMouseButtonDown(1) && currentWeapon.CanHeavyAttack())
            {
                PerformAttack(false);
            }
        }
    }

    private void PerformAttack(bool isLightAttack)
    {
        StartPushCooldown();
        StartCoroutine(PushCooldown());
        DashTowardsMouse();
        currentWeapon.Attack(isLightAttack);

        // Überprüfen Sie Kollisionen mit Feinden
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            BaseEnemies enemy = enemyCollider.GetComponent<BaseEnemies>();
            if (enemy != null)
            {
                int damage = isLightAttack ? currentWeapon.weaponStats.lightAttackDamage : currentWeapon.weaponStats.heavyAttackDamage;
                enemy.TakeDamage(damage, gameObject);
                Debug.Log($"Spieler greift Feind an und fügt {damage} Schaden zu!");
            }
        }
    }

    private void DashTowardsMouse()
    {
        playerState = PlayerState.Attacking;
        Vector3 targetPosition = CalculatePushDestination();
        StartCoroutine(MovePlayer(targetPosition));
    }

    private Vector3 CalculatePushDestination()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;

        Vector3 dashDirection = (mousePosition - transform.position).normalized;
        Vector3 dashDestination = transform.position + dashDirection * pushDistance;

        return dashDestination;
    }

    private IEnumerator MovePlayer(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < pushDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / pushDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        playerState = PlayerState.Normal;
    }

    private void StartPushCooldown()
    {
        isPushCooldown = true;
        pushCooldownTimer = pushCooldown;
    }

    private IEnumerator PushCooldown()
    {
        while (pushCooldownTimer > 0f)
        {
            pushCooldownTimer -= Time.deltaTime;
            yield return null;
        }
        isPushCooldown = false;
    }

    private void UpdateCooldowns()
    {
        currentWeapon.UpdateCooldowns();
    }

    public void SwitchWeapon(WeaponBase newWeapon)
    {
        Debug.Log($"switched weapon to {newWeapon.name}");
        currentWeapon = newWeapon;
    }
    private void OnDrawGizmosSelected()
    {
        // Visualisiert die Angriffsreichweite im Unity-Editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
