using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 moveDirection;
    private PlayerAnimationController animationController;

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


    [Header("Jump Settings")]
    public float jumpHeight = 1f;
    private int currentHeight = 0;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = walkSpeed;
        currentStamina = maxStamina;
        rb = GetComponent<Rigidbody2D>();
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
        // Automatisches Hochsteigen
        if (Physics2D.Raycast(transform.position, movement, 0.5f, obstacleLayer))
        {
            if (CanClimbAutomatically())
            {
                currentHeight++;
                transform.position += Vector3.up * jumpHeight;
            }
        }

        // Springen
        if (InputManager.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }
    bool CanClimbAutomatically()
    {
        // Hier Logik für automatisches Klettern implementieren
        return true; // Vereinfacht für dieses Beispiel
    }

    void Jump()
    {
        currentHeight++;
        transform.position += Vector3.up * jumpHeight;
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
