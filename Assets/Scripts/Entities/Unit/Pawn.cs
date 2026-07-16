using UnityEngine;
using Pathfinding;
using NaughtyAttributes;

using TinyProject.StateMachine.Entities;
using TinyProject.Enums;
using TinyProject.Resources;

namespace TinyProject.Entities
{
    [RequireComponent(typeof(IAstarAI))]
    public class Pawn : Unit
    {
        public HarvestState HarvestState { get; private set; }

        [BoxGroup("Pawn")] [SerializeField] private WorkerRole workerRole = WorkerRole.none;

        [BoxGroup("Inventory")] [SerializeField] private int extractionAmount = 1;
        [BoxGroup("Inventory")] [SerializeField, Min(0.1f)] private float extractionIntervalSeconds = 1f;
        [BoxGroup("Inventory")] [SerializeField] private ResourceType inventoryType = ResourceType.None;
        [BoxGroup("Inventory")] [SerializeField] private bool haveInventoryFull = false;
        [BoxGroup("Inventory")] [SerializeField] private int maxInventory = 10;
        [BoxGroup("Inventory")] [SerializeField] private int currentInventory = 0;

        private float nextExtractionTime;

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

            if (ai.velocity.magnitude == 0f && workerRole is WorkerRole.builder or WorkerRole.chopper or WorkerRole.miner or WorkerRole.hunter)
            {
                isInAction = true;
                ChangeEntityState(HarvestState);

                if (Time.time >= nextExtractionTime)
                {
                    ExtractResource();
                    nextExtractionTime = Time.time + extractionIntervalSeconds;
                }
            }
            else
            {
                isInAction = false;
                nextExtractionTime = Time.time;
            }
        }
        
        /// <summary>
        /// Définit le rôle du travailleur et met à jour l'animation en conséquence.
        /// </summary>
        /// <param name="newRole">Le nouveau rôle du travailleur.</param>
        public void SetWorkerRole(WorkerRole newRole)
        {
            workerRole = newRole;
            animator.SetInteger("WorkerRole", (int)workerRole);
        }

        private void assignWorkerRoleBasedOnInventory()
        {
            switch (inventoryType)
            {
                case ResourceType.Wood:
                    SetWorkerRole(WorkerRole.woodPorter);
                    break;
                case ResourceType.Gold:
                    SetWorkerRole(WorkerRole.goldPorter);
                    break;
                case ResourceType.Food:
                    SetWorkerRole(WorkerRole.foodPorter);
                    break;
                default:
                    SetWorkerRole(WorkerRole.none);
                    break;
            }
        }

        public override void MoveTo(Vector3 destination)
        {
            base.MoveTo(destination);

            if (haveInventoryFull)
            {
                assignWorkerRoleBasedOnInventory();
                return;
            }

            if (targetGameObject != null)
            {
                ResourceCollector resource = targetGameObject.GetComponent<ResourceCollector>();
                SetWorkerRole(resource.WorkerRole);
            }
            else 
            {
                SetWorkerRole(WorkerRole.none);
            }
        }
    
        /// <summary>
        /// Extrait une quantité de ressource de la ressource cible et l'ajoute à
        /// la charge actuelle du travailleur, si la charge maximale n'est pas atteinte.
        /// </summary>
        private void ExtractResource()
        {
            if (targetGameObject == null)
            {
                return;
            }

            if (currentInventory >= maxInventory)
            {
                haveInventoryFull = true;
                assignWorkerRoleBasedOnInventory();
                return;
            }

            ResourceCollector resource = targetGameObject.GetComponent<ResourceCollector>();
            
            if (currentInventory > 0 && resource.ResourceType != inventoryType) 
            {
                inventoryType = ResourceType.None;
                currentInventory = 0;
            }

            if (resource != null)
            {
                inventoryType = resource.ResourceType;
                resource.Take(extractionAmount);
                currentInventory += extractionAmount;
            }
            
            haveInventoryFull = currentInventory >= maxInventory;
        }
    }
}