using UnityEngine;
using Pathfinding;
using NaughtyAttributes;

using TinyProject.StateMachine.Entities;
using TinyProject.Enums;
using TinyProject.Resources;
using TinyProject.Singleton;

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

            if (IsNearResourceCollector() && ai.velocity.magnitude == 0f && workerRole is WorkerRole.builder or WorkerRole.chopper or WorkerRole.miner or WorkerRole.hunter)
            {
                isInAction = true;
                ChangeEntityState(HarvestState);

                if (Time.time >= nextExtractionTime)
                {
                    ExtractResource();
                    nextExtractionTime = Time.time + extractionIntervalSeconds;
                }
            }
            else if (IsNearResourceStorage() && ai.velocity.magnitude > 0f && workerRole is WorkerRole.woodPorter or WorkerRole.goldPorter or WorkerRole.foodPorter)
            {
                DepositResources();
                SetWorkerRole(WorkerRole.none);
            }
            else
            {
                isInAction = false;
                nextExtractionTime = Time.time;
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
        
        private void SetWorkerRole(WorkerRole newRole)
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

        private bool IsNearResourceCollector()
        {
            return targetGameObject != null
                && targetGameObject.TryGetComponent<ResourceCollector>(out _)
                && IsNearTargetGameObject();
        }

        private bool IsNearResourceStorage()
        {
            return targetGameObject != null
                && targetGameObject.TryGetComponent<ResourceStorage>(out _)
                && IsNearTargetGameObject();
        }

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
    
        private void DepositResources()
        {
            PlayerRessourceSingleton.Instance.Deposit(currentInventory, inventoryType);
            currentInventory = 0;
            inventoryType = ResourceType.None;
            haveInventoryFull = false;
        }
    }
}