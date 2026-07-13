using UnityEngine;
using Pathfinding;
using NaughtyAttributes;

using TinyProject.StateMachine.Entities;
using TinyProject.StateMachine;

namespace TinyProject.Entities
{
    /// <summary>
    /// Représente un travailleur dans le jeu, capable d'effectuer différentes tâches en fonction de son rôle.
    /// </summary>
    public enum WorkerRole 
    {
        none = 0,
        builder = 1,
        chopper = 2,
        miner = 3,
        hunter = 4,
        woodPorter = 5,
        goldPorter = 6,
        foodPorter = 7,
    }
    
    [RequireComponent(typeof(IAstarAI))]
    public class Pawn : Unit
    {
        public WorkerRoleState WorkerRoleState { get; private set; }
        public State EntityWorkerRoleState { get; private set; }

        [BoxGroup("Pawn")] [SerializeField] private WorkerRole workerRole = WorkerRole.none;

        public WorkerRole WorkerRole => workerRole;

        protected override void Start()
        {
            base.Start();

            WorkerRoleState = new WorkerRoleState(this);
            WorkerRoleState.Init(animator);

            WorkerRoleState.Enter((int)workerRole);
        }
        
        protected override void Update()
        {
            base.Update();
            EntityWorkerRoleState?.Tick();
        }

        protected override void FixedUpdate()
        {
            EntityWorkerRoleState?.FixedTick();
        }
        
        /// <summary>
        /// Définit le rôle du travailleur et met à jour l'animation en conséquence.
        /// </summary>
        /// <param name="newRole">Le nouveau rôle du travailleur.</param>
        public void SetWorkerRole(WorkerRole newRole)
        {
            workerRole = newRole;
            WorkerRoleState.Enter((int)newRole);
        }

        /// <summary>
        /// Change l'état actuel du rôle du travailleur vers un nouvel état spécifié.
        /// </summary>
        /// <param name="newState">Le nouvel état vers lequel changer.</param>
        protected void ChangeWorkerRoleState(State newState)
        {
            if (EntityWorkerRoleState == newState) return;

            EntityWorkerRoleState?.Exit();
            EntityWorkerRoleState = newState;
            EntityWorkerRoleState?.Enter();
        }
    }
}