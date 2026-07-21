using UnityEngine;
using NaughtyAttributes;
using TMPro;

using TinyProject.Enums;

namespace TinyProject.Singleton
{
    public class PlayerRessourceSingleton : MonoBehaviour
    {
        public static PlayerRessourceSingleton Instance { get; private set; }

        [BoxGroup("Resource Info")] [SerializeField] private int wood;
        [BoxGroup("Resource Info")] [SerializeField] private int gold;
        [BoxGroup("Resource Info")] [SerializeField] private int food;

        [BoxGroup("UI")] [Required] [SerializeField] private TextMeshProUGUI woodTextMeshPro;
        [BoxGroup("UI")] [Required] [SerializeField] private TextMeshProUGUI goldTextMeshPro;
        [BoxGroup("UI")] [Required] [SerializeField] private TextMeshProUGUI foodTextMeshPro;

        public int Wood => wood;
        public int Gold => gold;
        public int Food => food;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            RefreshUI();
        }

        private void RefreshUI()
        {
            if (woodTextMeshPro == null || goldTextMeshPro == null || foodTextMeshPro == null)
            {
                Debug.LogWarning("PlayerRessourceSingleton UI references are missing. Assign all TextMeshProUGUI fields in the inspector.", this);
                return;
            }

            woodTextMeshPro.text = wood.ToString();
            goldTextMeshPro.text = gold.ToString();
            foodTextMeshPro.text = food.ToString();
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
            
            RefreshUI();
        }
    }
}