using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyMoveable
{
    Rigidbody2D rb { get; set; }
    bool IsFachingRight { get; set; }
    void MoveEnemy(Vector2 velocity);
    void CheckForFacingToPlayer(Vector2 velocity);
}
