using UnityEngine;

namespace TinyProject.StateMachine.Entities
{
    /// <summary>
    /// État représentant l'état de récolte d'un personnage ou d'un objet.
    /// </summary>
    public class HarvestState : State
    {
        private static readonly int AnimationHash = Animator.StringToHash("isHarvesting");

        public HarvestState(MonoBehaviour owner) : base(owner) { }

        public override void Enter()
        {
            base.Enter();
            Animator.SetBool(AnimationHash, true);
            Animator.Play("Interact", -1, 0f);
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