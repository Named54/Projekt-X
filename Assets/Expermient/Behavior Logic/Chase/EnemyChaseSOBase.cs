using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base ScriptableObject for chase behavior
public class EnemyChaseSOBase : ScriptableObject
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

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    public virtual void DoEnterLogic() { }
    public virtual void DoExitLogic() { ResetValues(); }
    public virtual void DoFrameUpdateLogic()
    {
        if (enemy.IsWithinStrikingDistance)
        {
            enemy.stateMachine.ChangeState(enemy.attackState);
        }
    }
    public virtual void DoPhysicsLogic() { }
    public virtual void DoAnimationTriggerEventLogic(Enemy.AnimationTriggerType triggerType) { }
    public virtual void ResetValues() { }
    // Neue Methode für zusätzliche Logik
    public virtual bool ShouldTransitionToSpecialState() { return false; }
}
