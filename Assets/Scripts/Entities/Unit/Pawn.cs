using UnityEngine;
using Pathfinding;
using NaughtyAttributes;

using TinyProject.StateMachine.Entities;
using TinyProject.Enums;
using TinyProject.Resources;
using TinyProject.Singleton;
using TinyProject.Entities.Buildings;

namespace TinyProject.Entities
{
    [RequireComponent(typeof(IAstarAI))]
    public class Pawn : Unit
    {
        public HarvestState HarvestState { get; private set; }

        [BoxGroup("Pawn")] [SerializeField] private WorkerRole workerRole = WorkerRole.none;
        [BoxGroup("Pawn")] [SerializeField, Min(0.1f)] private float tickInterval = 1f;
        
        [BoxGroup("Build")] [SerializeField] private int buildingForce = 1;

        [BoxGroup("Inventory")] [SerializeField] private int extractionAmount = 1;
        [BoxGroup("Inventory")] [SerializeField] private ResourceType inventoryType = ResourceType.None;
        [BoxGroup("Inventory")] [SerializeField] private bool haveInventoryFull = false;
        [BoxGroup("Inventory")] [SerializeField] private int maxInventory = 10;
        [BoxGroup("Inventory")] [SerializeField] private int currentInventory = 0;

        [BoxGroup("Boucle")] [SerializeField] private GameObject assignedResource;
        [BoxGroup("Boucle")] [SerializeField] private WorkerRole assignedWorkerRole;

        private float nextTickTime;

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

                if (TickTimer())
                {
                    ExtractResource();
                }
            }
            else if (IsNearResourceStorage() && ai.velocity.magnitude > 0f && workerRole is WorkerRole.woodPorter or WorkerRole.goldPorter or WorkerRole.foodPorter)
            {
                isInAction = false;
                DepositResources();
                SetWorkerRole(assignedWorkerRole);
                targetGameObject = assignedResource;
                MoveToTargetGameObject();
            }
            else if (IsNearConstructionSite() && ai.velocity.magnitude == 0f && workerRole is WorkerRole.builder)
            {
                isInAction = true;
                ChangeEntityState(HarvestState);
                SetWorkerRole(WorkerRole.builder);

                if (TickTimer())
                {
                    Debug.Log($"Building {targetGameObject.name} with force {buildingForce}");
                    targetGameObject.GetComponent<Building>().Construct(buildingForce);
                }
            }
            else if (haveInventoryFull)
            {
                isInAction = false;
                targetGameObject = GetNearestResourceStorageGameObject();
                assignWorkerRoleBasedOnInventory();
                MoveToTargetGameObject();
            }
            else
            {
                isInAction = false;
            }
        }

        private bool TickTimer()
        {
            if (Time.time >= nextTickTime)
            {
                nextTickTime = Time.time + tickInterval;
                return true;
            }
            return false;
        }

        public override void MoveTo(Vector3 destination)
        {
            base.MoveTo(destination);

            if (haveInventoryFull)
            {
                assignWorkerRoleBasedOnInventory();
                return;
            }

            if (targetGameObject != null && targetGameObject.TryGetComponent(out ResourceCollector resource))
            {
                SetWorkerRole(resource.WorkerRole);
            }
            else if (targetGameObject != null && targetGameObject.TryGetComponent(out ResourceStorage storage))
            {
                SetWorkerRole(WorkerRole.none);
            }
            else if (targetGameObject != null && targetGameObject.TryGetComponent(out Building building) && !building.IsConstructed)
            {
                SetWorkerRole(WorkerRole.builder);
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

        private bool IsNearConstructionSite()
        {
            return targetGameObject != null
                && targetGameObject.TryGetComponent(out Building building)
                && !building.IsConstructed
                && IsNearTargetGameObject(2f);
        }

        private GameObject GetNearestResourceStorageGameObject()
        {
            ResourceStorage[] storages = FindObjectsByType<ResourceStorage>();
            if (storages == null || storages.Length == 0)
            {
                return null;
            }

            ResourceStorage nearestStorage = null;
            float minSqrDistance = float.MaxValue;

            foreach (ResourceStorage storage in storages)
            {
                float sqrDistance = (storage.transform.position - transform.position).sqrMagnitude;
                if (sqrDistance < minSqrDistance)
                {
                    minSqrDistance = sqrDistance;
                    nearestStorage = storage;
                }
            }

            return nearestStorage != null ? nearestStorage.gameObject : null;
        }

        private void ExtractResource()
        {
            if (currentInventory >= maxInventory)
            {
                haveInventoryFull = true;
                targetGameObject = GetNearestResourceStorageGameObject();
                assignWorkerRoleBasedOnInventory();
                MoveToTargetGameObject();
                return;
            }

            if (targetGameObject == null)
            {
                return;
            }   

            assignedResource = targetGameObject;
            assignedWorkerRole = workerRole;

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