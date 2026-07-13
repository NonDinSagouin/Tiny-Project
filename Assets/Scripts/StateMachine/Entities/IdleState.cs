using UnityEngine;

public class IdleState : State
{
    private static readonly int AnimationHash = Animator.StringToHash("isIdle");

    public IdleState(MonoBehaviour owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
        Animator.SetBool(AnimationHash, true);
    }

    public override void Tick()
    {
        base.Tick();
    }

    public override void Exit()
    {
        base.Exit();
        Animator.SetBool(AnimationHash, false);
    }
}