using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Base ScriptableObject for idle behavior
public class EnemyIdleSOBase : ScriptableObject
{
    protected Enemy enemy;
    protected Transform Transform;
    protected GameObject gameObject;

    protected Transform playerTransform;

    public virtual void Initialize(GameObject gameObject, Enemy enemy)
    {
        this.gameObject = gameObject;
        Transform = gameObject.transform;
        this.enemy = enemy;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public virtual void DoEnterLogic() { }
    public virtual void DoExitLogic() { ResetValues(); }
    public virtual void DoFrameUpdateLogic() 
    {
        if (enemy.IsAggroed)
        {
            enemy.stateMachine.ChangeState(enemy.chaseState);
        }
    }
    public virtual void DoPhysicsLogic() { }
    public virtual void DoAnimationTriggerEventLogic(Enemy.AnimationTriggerType triggerType) { }
    public virtual void ResetValues() { }
}

