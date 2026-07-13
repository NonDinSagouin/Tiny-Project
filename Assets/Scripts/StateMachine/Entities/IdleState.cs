using UnityEngine;

using TinyProject.StateMachine;

namespace TinyProject.StateMachine.Entities
{
    /// <summary>
    /// État représentant l'état d'inactivité d'un personnage ou d'un objet.
    /// </summary>
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
}