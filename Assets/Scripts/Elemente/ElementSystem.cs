using System;
using UnityEngine;

public class ElementSystem : MonoBehaviour
{
    public ElementBase currentElement;
    public PlayerMovement movement;

    [Header("Element Settings")]
    public bool canUseElement = true;
    public float elementRange = 1.5f;

    public LayerMask enemyLayer;

    private ElementWheelController elementWheel;

    private void Start()
    {
        elementWheel = FindFirstObjectByType<ElementWheelController>();
    }

    private void Update()
    {
        if (elementWheel != null && elementWheel.IsElementWheelOpen())
        {
            return;
        }
        HandleElementInput();
        UpdateCooldowns();
    }

    private void HandleElementInput()
    {
        if (!canUseElement)
        {
            return;
        }

        if (InputManager.GetMouseButtonDown(0) && currentElement.CanElementAttack()) // Linke Maustaste für Elementangriff
        {
            PerformElementAttack();
        }
        else if (InputManager.GetMouseButtonDown(1) && currentElement.CanElementAttack())  // Rechte Maustaste für Elementangriff
        {
            PerformElementAttack();
        }
    }

    private void PerformElementAttack()
    {
        Vector3 attackDirection = GetAttackDirection();
        CheckEnemyCollisions(attackDirection);
    }

    private Vector3 GetAttackDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;
        return (mousePosition - transform.position).normalized;
    }

    private void CheckEnemyCollisions(Vector3 attackDirection)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, elementRange, enemyLayer);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.TryGetComponent(out BaseEnemies enemy))
            {
                currentElement.ApplyElementalEffect(enemy.gameObject);
                Debug.Log($"Element effect applied to enemy: {enemy.name}");
            }
        }
    }

    private void UpdateCooldowns()
    {
        currentElement.UpdateCooldowns();
    }

    public void SwitchElement(ElementBase newElement)
    {
        currentElement = newElement;
        Debug.Log($"Switched element to {newElement.name}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, elementRange);
    }
}
