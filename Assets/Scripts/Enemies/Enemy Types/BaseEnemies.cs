using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BaseEnemies : MonoBehaviour
{
    [Header("UI")]
    public Vector3 offset;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient healthGradient;
    [SerializeField] private float updateSpeed = 3f;
    [SerializeField] private bool useSmoothing = true;
    private float targetFillAmount;

    [Header("Experience Settings")]
    public int experienceValue = 50;
    private PlayerLevelUpSystem playerLevelSystem;

    [Header("Health Settings")]
    public int maxHealth; // Maximale Gesundheit des Feindes
    protected int currentHealth; // Aktuelle Gesundheit des Feindes

    [Header("Knockback Settings")]
    protected KnockbackReceiver knockbackReceiver;

    [Header("Range Settings")]
    public float exitRange = 7f; // Reichweite, ab der der Feind den Spieler nicht mehr verfolgt
    public float attackRange = 1f; // Reichweite, in der der Feind den Spieler angreifen kann
    public float detectionRange = 5f; // Reichweite, in der der Feind den Spieler erkennt

    protected Rigidbody2D rb; // Rigidbody2D-Komponente des Feindes
    protected Transform player; // Transform-Komponente des Spielers
    protected bool isAttacking = false; // Gibt an, ob der Feind gerade angreift
    protected bool isPlayerDetected = false; // Gibt an, ob der Spieler erkannt wurde
    protected Parry_Block_System playerBlockSystem; // Referenz auf das Parade- und Block-System des Spielers

    protected virtual void Start()
    {
        currentHealth = maxHealth; // Setzt die aktuelle Gesundheit auf das Maximum
        rb = GetComponent<Rigidbody2D>(); // Holt die Rigidbody2D-Komponente
        knockbackReceiver = GetComponent<KnockbackReceiver>();
        playerLevelSystem = FindFirstObjectByType<PlayerLevelUpSystem>();
        FindPlayer(); // Sucht den Spieler in der Szene
    }

    protected virtual void Update()
    {
        healthSlider.transform.position = Camera.main.WorldToScreenPoint(transform.position) + offset;
        if (player == null)
        {
            FindPlayer(); // Sucht erneut nach dem Spieler, falls er nicht gefunden wurde
        }
        else
        {
            CheckPlayerPosition(); // Überprüft die Position des Spielers
        }
    }
    private void UpdateHealthBar()
    {
        // Berechne den Ziel-Füllwert
        targetFillAmount = (float)currentHealth / maxHealth;

        // Aktualisiere den Slider-Wert
        healthSlider.value = targetFillAmount;

        if (useSmoothing)
        {
            // Sanfte Übergang des Fill-Bildes
            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFillAmount, Time.deltaTime * updateSpeed);
        }
        else
        {
            // Sofortige Aktualisierung des Fill-Bildes
            fillImage.fillAmount = targetFillAmount;
        }

        // Aktualisiere die Farbe basierend auf dem Gesundheitszustand
        fillImage.color = healthGradient.Evaluate(targetFillAmount);

        // Optional: Füge visuelle Effekte hinzu, wenn die Gesundheit niedrig ist
        if (targetFillAmount <= 0.2f)
        {
            StartCoroutine(PulseHealthBar());
        }
    }
    private IEnumerator PulseHealthBar()
    {
        float pulseDuration = 0.5f;
        float elapsedTime = 0f;
        Color originalColor = fillImage.color;
        Color pulseColor = Color.red;

        while (elapsedTime < pulseDuration)
        {
            fillImage.color = Color.Lerp(originalColor, pulseColor, Mathf.PingPong(elapsedTime / pulseDuration, 1));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fillImage.color = originalColor;
    }

    // Rufen Sie diese Methode auf, wenn sich die Gesundheit ändert
    public void OnHealthChanged()
    {
        UpdateHealthBar();
    }
    protected void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Players"); // Sucht den Spieler anhand des Tags
        if (playerObj != null)
        {
            player = playerObj.transform; // Speichert die Transform-Komponente des Spielers
            playerBlockSystem = player.GetComponent<Parry_Block_System>(); // Holt die Parry_Block_System-Komponente des Spielers
        }
    }

    protected virtual void CheckPlayerPosition()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position); // Berechnet die Distanz zum Spieler

            if (!isPlayerDetected && distanceToPlayer <= detectionRange)
            {
                isPlayerDetected = true; // Erkennt den Spieler, wenn er in Reichweite kommt
            }
            else if (isPlayerDetected && distanceToPlayer > exitRange)
            {
                isPlayerDetected = false; // Verliert den Spieler aus den Augen, wenn er zu weit weg ist
            }
        }
    }

    protected virtual void MoveTowardsPlayer()
    {
        // Bewegt den FrogEnemy langsam zum Spieler
        /*
        if (player != null && !isJumping && !isOnCooldown)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.position += direction * (Time.deltaTime * jumpSpeed * 0.5f);
        }
        */
    }
    public virtual void TakeDamage(int damage, GameObject causer)
    {
        currentHealth -= damage;
        UpdateHealthBar();
        knockbackReceiver.ApplyKnockback(causer.transform.position);
        ResetEnemyState();
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    protected virtual void ResetEnemyState()
    {
        isPlayerDetected = false;
        isAttacking = false;
        // Reset any other state variables as needed
        StartCoroutine(SearchForPlayerAfterDelay());
    }
    protected IEnumerator SearchForPlayerAfterDelay()
    {
        yield return new WaitForSeconds(1f); // Adjust delay as needed
        FindPlayer();
    }
    protected virtual void Die()
    {
        playerLevelSystem.GainExperience(experienceValue);
        Destroy(gameObject);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        // Visualisiert die verschiedenen Reichweiten des Feindes im Editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, exitRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}