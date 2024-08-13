using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry_Block_System : MonoBehaviour
{
    private Player_Movement movement;
    private Player_health playerHealth;

    [Header("Parry Settings")]
    public int parryDamage;
    public bool isParrying = false;
    public float parryTimer = 0f;
    public float parryWindowDuration = 0.2f;
    public float parryWindowRange = 2f;

    [Header("Block Settings")]
    public bool isBlocking = false;
    public float blockWindowRange = 2f;
    public float blockPushForce = 3f;

    private CircleCollider2D blockCollider;

    [Header("Layer Settings")]
    public LayerMask enemyLayer;

    [SerializeField] private Transform parryWindow;
    [SerializeField] private Transform blockWindow;

    private void Start()
    {
        movement = GetComponent<Player_Movement>();
        playerHealth = GetComponent<Player_health>();

        blockCollider = gameObject.AddComponent<CircleCollider2D>();
        blockCollider.radius = blockWindowRange;
        blockCollider.isTrigger = true;
        blockCollider.enabled = false;
    }

    void Update()
    {
        HandleParryInput();
        UpdateParryTimer();
        HandleBlockInput();
    }

    void HandleParryInput()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isParrying && !isBlocking)
        {
            StartParry();
        }
    }

    void StartParry()
    {
        isParrying = true;
        movement.canMove = false;
        parryTimer = parryWindowDuration;
        GetPariert(gameObject);
        Debug.Log("Parry Window Open");
    }

    void EndParry()
    {
        isParrying = false;
        movement.canMove = true;
        Debug.Log("Parry Window Closed");
    }

    void UpdateParryTimer()
    {
        if (isParrying)
        {
            parryTimer -= Time.deltaTime;
            if (parryTimer <= 0)
            {
                EndParry();
                if (Input.GetKey(KeyCode.Q))
                {
                    StartBlock();
                }
            }
        }
    }

    void StartBlock()
    {
        if (movement.currentStamina <= 0)
            return;
        isBlocking = true;
        movement.canMove = false;
        blockCollider.enabled = true;
        Debug.Log("Block Window Opened!");
    }

    void HandleBlockInput()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            EndBlock();
        }
    }

    void EndBlock()
    {
        if (isBlocking)
        {
            isBlocking = false;
            movement.canMove = true;
            blockCollider.enabled = false;
            Debug.Log("Block Window Closed!");
        }
    }

    bool IsEnemyInParryWindow(Collider2D enemyCollider)
    {
        if (parryWindow != null && enemyCollider != null)
        {
            float distanceToEnemy = Vector2.Distance(parryWindow.position, enemyCollider.transform.position);
            return distanceToEnemy <= parryWindowRange;
        }
        return false;
    }

    public void GetPariert(GameObject causer)
    {
        Collider2D[] parryEnemies = Physics2D.OverlapCircleAll(parryWindow.position, parryWindowRange, enemyLayer);
        Debug.Log(parryEnemies.Length);
        if (parryEnemies.Length > 0)
        {
            foreach (Collider2D enemyCollider in parryEnemies)
            {
                if (IsEnemyInParryWindow(enemyCollider))
                {
                    Debug.Log("Player parry Enemy Attack!");
                    enemyCollider.GetComponent<BaseEnemies>()?.TakeDamage(parryDamage, causer);
                    continue;
                }
            }
        }
    }

    bool IsEnemyInBlockWindow(Collider2D enemyCollider)
    {
        if (blockWindow != null && enemyCollider != null)
        {
            float distanceToEnemy = Vector2.Distance(blockWindow.position, enemyCollider.transform.position);
            return distanceToEnemy <= blockWindowRange;
        }
        return false;
    }

    public bool TryBlock(int damage, GameObject attacker)
    {
        if (isBlocking && movement.currentStamina > 0)
        {
            ConsumeStaminaForBlocking(movement.staminaCostPerHit);
            Vector2 pushDirection = ((Vector2)attacker.transform.position - (Vector2)transform.position).normalized;
            Rigidbody2D attackerRb = attacker.GetComponent<Rigidbody2D>();
            if (attackerRb != null)
            {
                attackerRb.AddForce(pushDirection * blockPushForce, ForceMode2D.Impulse);
            }
            return true;
        }
        return false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isBlocking && collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
            Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                enemyRb.AddForce(pushDirection * blockPushForce, ForceMode2D.Impulse);
            }
            ConsumeStaminaForBlocking(movement.staminaCostPerHit);
        }
    }

    private void ConsumeStaminaForBlocking(float amount)
    {
        movement.currentStamina -= amount;
        movement.currentStamina = Mathf.Max(0, movement.currentStamina);
    }

    private void OnDrawGizmosSelected()
    {
        if (parryWindow == null || blockWindow == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(parryWindow.position, parryWindowRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(blockWindow.position, blockWindowRange);
    }
}
