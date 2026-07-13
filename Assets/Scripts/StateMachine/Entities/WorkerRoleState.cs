using UnityEngine;
using NaughtyAttributes;

using TinyProject.Entities;

namespace TinyProject.StateMachine.Entities
{
    /// <summary>
    /// Représente l'état d'un travailleur dans la state machine, en fonction de son rôle.
    /// </summary>
    public class WorkerRoleState : State
    {
        public WorkerRoleState(MonoBehaviour owner) : base(owner) { }

        public void Enter(int workerRole)
        {
            base.Enter();
            Animator.SetInteger("WorkerRole", workerRole);
        }

        public override void Tick()
        {
            base.Tick();
        }

        public override void Exit()
        {
            base.Exit();
        }

    }
}