using UnityEngine;
using NaughtyAttributes;

using TinyProject.Entities;
using System.Collections.Generic;

namespace TinyProject.Resources
{
    public class ResourceStorage : MonoBehaviour
    {
        [BoxGroup("Resource Info")] [SerializeField] private int wood;
        [BoxGroup("Resource Info")] [SerializeField] private int gold;
        [BoxGroup("Resource Info")] [SerializeField] private int food;

        [BoxGroup("Resource Info")] [SerializeField] private List<Transform> storagePoints;

        public int Wood => wood;
        public int Gold => gold;
        public int Food => food;
        public List<Transform> StoragePoints => storagePoints;

        void Start()
        {
        }

        /// <summary>
        /// Réduit la quantité de ressource actuelle de la ressource.
        /// </summary>
        /// <param name="amount">La quantité de ressource à retirer.</param>
        public void Deposit(int amount, ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.Wood:
                    wood += amount;
                    break;
                case ResourceType.Gold:
                    gold += amount;
                    break;
                case ResourceType.Food:
                    food += amount;
                    break;
                default:
                    Debug.LogWarning($"Resource type {resourceType} is not recognized.");
                    break;
            }
        }
    }
}
