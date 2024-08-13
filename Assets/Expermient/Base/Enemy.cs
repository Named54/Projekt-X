using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Main enemy class that implements the state machine
public class Enemy : MonoBehaviour, IDamageable, IEnemyMoveable, ITriggerCheckable
{
    protected Transform playerTransform;
    public Rigidbody2D rb { get; set; }
    public float currentHealth { get; set; }
    public bool IsFachingRight { get; set; } = true;
    [field: SerializeField] public float maxHealth { get; set; }
    public bool IsAggroed { get; set; }
    public bool IsWithinStrikingDistance { get; set; }

    // State machine and states
    public EnemyIdleState idleState { get; set; }
    public EnemyChaseState chaseState { get; set; }
    public EnemyAttackState attackState { get; set; }
    public EnemyStateMachine stateMachine { get; set; }

    #region ScriptablrObject Variables
    // ScriptableObject instances for behavior logic
    [SerializeField] private EnemyIdleSOBase enemyIdleBase;
    [SerializeField] private EnemyChaseSOBase enemyChaseBase;
    [SerializeField] private EnemyAttackSOBase enemyAttackBase;
    public EnemyIdleSOBase enemyIdleBaseInstance { get; set; }
    public EnemyChaseSOBase enemyChaseBaseInstance { get; set; }
    public EnemyAttackSOBase enemyAttackBaseInstance { get; set; }
    #endregion
    // Initialize states and ScriptableObjects
    protected virtual void Awake()
    {
        enemyIdleBaseInstance = Instantiate(enemyIdleBase);
        enemyChaseBaseInstance = Instantiate(enemyChaseBase);
        enemyAttackBaseInstance = Instantiate(enemyAttackBase);

        stateMachine = new EnemyStateMachine();
        idleState = new EnemyIdleState(this, stateMachine);
        chaseState = new EnemyChaseState(this,stateMachine);
        attackState = new EnemyAttackState(this, stateMachine);
    }
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        enemyIdleBaseInstance.Initialize(gameObject, this);
        enemyChaseBaseInstance.Initialize(gameObject, this);
        enemyAttackBaseInstance.Initialize(gameObject, this);

        stateMachine.Initialize(idleState);

        StartCoroutine(FindPlayerCoroutine());
    }
    // Update methods for the current state
    protected virtual void Update()
    {
        stateMachine.currentEnemyState.FrameUpdate();
    }
    private void FixedUpdate()
    {
        stateMachine.currentEnemyState.PhysicsUpdate();
    }
    public void TakeDamage(int damageAmount, GameObject causer)
    {
        currentHealth -= damageAmount;
        if(currentHealth <= 0f)
        {
            Die();
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }
    #region Movement Functions 
    public void MoveEnemy(Vector2 velocity)
    {
        rb.velocity = velocity;
        CheckForFacingToPlayer(velocity);
    }

    public void CheckForFacingToPlayer(Vector2 velocity)
    {
        if(IsFachingRight && velocity.x < 0f)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFachingRight = !IsFachingRight;
        }
        else if (IsFachingRight && velocity.x > 0f)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFachingRight = !IsFachingRight;
        }
    }
    #endregion
    private IEnumerator FindPlayerCoroutine()
    {
        while (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void AnimatonTriggerEvent(AnimationTriggerType triggerType)
    {
        stateMachine.currentEnemyState.AnimationTriggerEvent(triggerType);
    }

    public enum AnimationTriggerType
    {
        EnemyDamaged,
        PlayJumpSound
    }
    public void SetAggroStatus(bool isAggroed)
    {
        IsAggroed = isAggroed;
    }

    public void SetStrikingDistanceBool(bool isWithinStrikingDistance)
    {
        IsWithinStrikingDistance = isWithinStrikingDistance;
    }
}
