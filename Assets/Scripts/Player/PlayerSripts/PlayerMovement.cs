using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 moveDirection;
    private DepthSystem depthSystem;
    private PlayerAnimationController animationController;

    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float jumpCooldown = 0.5f;
    private bool canJump = true;
    private float jumpTimer = 0f;

    [Header("Move Settings")]
    public int walkSpeed;
    public int moveSpeed;
    public bool canMove = true;
    public bool isSprinting = false;
    public int sprintSpeedMultiplier;
    public bool lockMovementDirection = false;

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

    [Header("Layer Settings")]
    public LayerMask obstacleLayer;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = walkSpeed;
        currentStamina = maxStamina;
        rb = GetComponent<Rigidbody2D>();
        depthSystem = GetComponent<DepthSystem>();
        animationController = GetComponent<PlayerAnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!EcsMenu.isPaused)
        {
            HandleDash();
            HandleSprint();
            UpdateStamina();
            HandleJumpCooldown();
            HandleMovementInput();
            UpdateAnimationController();
        }
    }
    void FixedUpdate()
    {
        MovePlayer();
    }
    public void HandleMovementInput()
    {
        if (!lockMovementDirection)
        {
            // Eingabe für die horizontale und vertikale Bewegung erfassen
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            moveDirection = new Vector2(moveX, moveY).normalized; // Normalisierte Richtung
        }
        else
        {
            // Bewegung ist gesperrt, setze movement auf 0
            moveDirection = Vector2.zero;
        }
    }
    public void HandleJumpCooldown()
    {
        if (!canJump)
        {
            jumpTimer += Time.deltaTime;
            if (jumpTimer >= jumpCooldown)
            {
                canJump = true;
                jumpTimer = 0f;
            }
        }

        // Springen
        if (InputManager.GetKeyDown(KeyCode.Space) && canJump)
        {
            HandleJump();
        }
    }

    public void HandleJump()
    {
        if (!canJump || currentStamina < staminaCostPerHit) return;

        canJump = false;
        currentStamina -= staminaCostPerHit;

        Vector2 jumpDirection = moveDirection != Vector2.zero ? moveDirection : Vector2.up;

        // Prüfe, ob ein Sprung in diese Richtung möglich ist
        RaycastHit2D hit = Physics2D.Raycast(transform.position, jumpDirection, 1f, obstacleLayer);
        if (hit.collider != null)
        {
            // Hindernis gefunden, prüfe auf eine höhere Ebene
            depthSystem.ChangeDepth(depthSystem.currentDepth + 1);
        }
        else
        {
            // Kein Hindernis, normaler Sprung
            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
            Debug.Log(jumpDirection);
        }
    }
    void UpdateAnimationController()
    {
        if (moveDirection.magnitude > 0)
        {
            animationController.SetSelectedSpriteBook(animationController.WalkSpriteBook);
            animationController.lookDirection = moveDirection;
            if (isSprinting)
            {
                animationController.SetSelectedSpriteBook(animationController.RunSpriteBook);
            }
        }
        else
        {
            animationController.SetSelectedSpriteBook(animationController.IdleSpriteBook);
        }
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

    if (InputManager.GetKeyDown(KeyCode.LeftControl) && dashCooldownTimer <= 0 && !isDashing && currentStamina >= dashStaminaCost)
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
        if (InputManager.GetKey(KeyCode.LeftShift) && currentStamina > 0 && !isDashing)
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

    // Neue öffentliche Methode zur Wiederherstellung von Stamina
    public void RestoreStamina(float amount)
    {
        currentStamina = Mathf.Min(maxStamina, currentStamina + amount);
        UpdateStaminaBar();
    }
    // Aktualisierte UpdateStamina-Methode
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
        UpdateStaminaBar();
    }

    // Neue Methode zur Aktualisierung der Stamina-Anzeige
    private void UpdateStaminaBar()
    {
        if (StaminaBar != null)
        {
            StaminaBar.fillAmount = currentStamina / maxStamina;
        }
    }

}
