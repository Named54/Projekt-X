using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player_Movement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 moveDirection;

    [Header("Move Settings")]
    public int walkSpeed;
    public int moveSpeed;
    public bool canMove = true;
    public bool isSprinting = false;
    public int sprintSpeedMultiplier;

    [Header("Stamina Settings")]
    public Image StaminaBar;
    public float maxStamina;
    public float currentStamina;
    public float staminaCostPerHit;
    public float sprintStaminaCost;
    public float staminaRegenerationRate;

    [Header("Dash Settings")]
    public int dashSpeed;
    public float dashCooldown;
    public float dashDuration;
    public float dashStaminaCost;
    public float dashCooldownTimer;
    public bool isDashing = false;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = walkSpeed;
        currentStamina = maxStamina;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleDash();
        HandleSprint();
        UpdateStamina();
        HandleMovementInput();
    }
    void FixedUpdate()
    {
        MovePlayer();
    }
    public void HandleMovementInput()
    {
        // Eingabe für die horizontale und vertikale Bewegung erfassen
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized; // Normalisierte Richtung
    }
    public void MovePlayer()
    {
        if (!canMove || isDashing) return;
        CombatSystem combatSystem = GetComponent<CombatSystem>();
        if (!combatSystem.CanPerformAction(PlayerStateFlags.Attacking | PlayerStateFlags.Pushing)) return;


        Vector2 movement = Time.fixedDeltaTime * moveSpeed * moveDirection;
        rb.position = rb.position + movement;
    }
    public void HandleDash()
    {
        dashCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftControl) && dashCooldownTimer <= 0 && !isDashing && currentStamina >= dashStaminaCost)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        canMove = false;
        currentStamina -= dashStaminaCost;
        Vector2 dashDirection = moveDirection != Vector2.zero ? moveDirection : transform.up;
        Vector2 startPosition = rb.position;
        Vector2 endPosition = startPosition + dashDirection * dashSpeed;

        float elapsedTime = 0;
        while (elapsedTime < dashDuration)
        {
            rb.position = Vector2.Lerp(startPosition, endPosition, elapsedTime / dashDuration);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.position = endPosition;
        isDashing = false;
        canMove = true;
        dashCooldownTimer = dashCooldown;
    }

    public void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && !isDashing)
        {
            isSprinting = true;
            moveSpeed = walkSpeed * sprintSpeedMultiplier;
        }
        else
        {
            isSprinting = false;
            moveSpeed = walkSpeed;
        }
    }

    private void UpdateStamina()
    {
        if (isSprinting)
        {
            float staminaConsumed = sprintStaminaCost * Time.deltaTime;
            currentStamina -= staminaConsumed;
        }
        else if (currentStamina < maxStamina)
        {
            float staminaRegenerated = staminaRegenerationRate * Time.deltaTime;
            currentStamina += staminaRegenerated;
        }
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        StaminaBar.fillAmount = currentStamina / maxStamina;
    }
}
