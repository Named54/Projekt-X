using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogDashState : EnemyState
{
    private FrogDashSO dashSO;
    public FrogDashState(Enemy enemy, EnemyStateMachine enemyStateMachine, FrogDashSO dashSO) : base(enemy, enemyStateMachine)
    {
        this.dashSO = dashSO;
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        // Start the dash coroutine
        enemy.StartCoroutine(dashSO.DashCoroutine());
    }
    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
