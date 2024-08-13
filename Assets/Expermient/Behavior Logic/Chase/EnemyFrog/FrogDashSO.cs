using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Frog-Dash", menuName = "Enemy Logic/Frog Logic/Dash")]
public class FrogDashSO : ScriptableObject
{
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float dashCooldown = 2f;

    private Enemy enemy;
    private Transform playerTransform;

    public void Initialize(Enemy enemy)
    {
        this.enemy = enemy;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public IEnumerator DashCoroutine()
    {
        Vector2 dashDirection = (playerTransform.position - enemy.transform.position).normalized;
        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            enemy.rb.velocity = dashDirection * dashSpeed;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        enemy.rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(dashCooldown);
    }
}