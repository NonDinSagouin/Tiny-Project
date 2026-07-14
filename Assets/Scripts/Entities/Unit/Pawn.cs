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
        public HarvestState HarvestState { get; private set; }

        [BoxGroup("Pawn")] [SerializeField] private WorkerRole workerRole = WorkerRole.none;
        public WorkerRole WorkerRole => workerRole;

        protected override void Start()
        {
            base.Start();

            HarvestState = new HarvestState(this);
            HarvestState.Init(animator);
        }
        
        protected override void Update()
        {
            base.Update();

            animator.SetInteger("WorkerRole", (int)workerRole);

            if (isNearTarget && workerRole != WorkerRole.none)
            {
                ChangeEntityState(HarvestState);
            }
        }
        
        /// <summary>
        /// Définit le rôle du travailleur et met à jour l'animation en conséquence.
        /// </summary>
        /// <param name="newRole">Le nouveau rôle du travailleur.</param>
        public void SetWorkerRole(WorkerRole newRole)
        {
            workerRole = newRole;
        }
    }
}