using UnityEngine;
using NaughtyAttributes;

using TinyProject.Enums;

namespace TinyProject.Singleton
{
    public class PlayerRessourceSingleton : MonoBehaviour
    {
        public static PlayerRessourceSingleton instance;

        [BoxGroup("Resource Info")] [SerializeField] private int wood;
        [BoxGroup("Resource Info")] [SerializeField] private int gold;
        [BoxGroup("Resource Info")] [SerializeField] private int food;

        public int Wood => wood;
        public int Gold => gold;
        public int Food => food;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;
        }

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