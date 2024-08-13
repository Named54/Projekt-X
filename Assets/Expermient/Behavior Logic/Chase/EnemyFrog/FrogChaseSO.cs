using UnityEngine;

[CreateAssetMenu(fileName = "Frog-Chase", menuName = "Enemy Logic/Frog Logic/Chase")]
public class FrogChaseSO : EnemyChaseSOBase
{
    [SerializeField] private float dashProbability = 0.1f;
    [SerializeField] private float minDashInterval = 2f;
    private float lastDashTime;

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
        lastDashTime = -minDashInterval; // Erlaubt einen sofortigen Dash beim Start
    }

    public override bool ShouldTransitionToSpecialState()
    {
        if (Time.time - lastDashTime >= minDashInterval && Random.value < dashProbability)
        {
            lastDashTime = Time.time;
            return true;
        }
        return false;
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();
        if (ShouldTransitionToSpecialState())
        {
            (enemy as Frog)?.TransitionToDashState();
        }
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoPhysicsLogic()
    {
        base.DoPhysicsLogic();
    }

    public override void DoAnimationTriggerEventLogic(Enemy.AnimationTriggerType triggerType)
    {
        base.DoAnimationTriggerEventLogic(triggerType);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
}