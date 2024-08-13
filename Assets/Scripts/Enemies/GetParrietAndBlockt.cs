using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetParrietAndBlockt : MonoBehaviour
{

    public Transform getParryWindow;
    public Transform getBlockWindow;

    [SerializeField] private float EnemyParryWindowRange;
    [SerializeField] private float EnemyBlockWindowRange;

    private void OnDrawGizmosSelected()
    {
        if (getParryWindow == null || getBlockWindow == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(getParryWindow.position, EnemyParryWindowRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(getBlockWindow.position, EnemyBlockWindowRange);
    }
}
