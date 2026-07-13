using UnityEngine;

public class WalkingState : State
{
    private static readonly int AnimationHash = Animator.StringToHash("isWalking");

    public WalkingState(MonoBehaviour owner) : base(owner) { }

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