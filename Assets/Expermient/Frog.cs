using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : Enemy
{
    public FrogDashState dashState { get; private set; }
    public FrogDashSO frogDashSOInstance { get; private set; }
    public FrogChaseSO frogChaseSOInstance { get; private set; }
    public FrogDashSO frogDashSO { get; private set; }
    public FrogChaseSO frogChaseSO { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        frogDashSOInstance = Instantiate(frogDashSO);
        frogChaseSOInstance = Instantiate(frogChaseSO);
        dashState = new FrogDashState(this, stateMachine, frogDashSOInstance);

        // Ersetzen Sie die Standard-Chase-Instanz durch die Frosch-spezifische
        enemyChaseBaseInstance = frogChaseSOInstance;
    }

    protected override void Start()
    {
        base.Start();
        frogDashSOInstance.Initialize(this);
        frogChaseSOInstance.Initialize(gameObject, this);
    }

    public void TransitionToDashState()
    {
        stateMachine.ChangeState(dashState);
    }
}
