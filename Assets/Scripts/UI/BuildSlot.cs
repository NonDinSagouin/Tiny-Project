using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using NaughtyAttributes;

using TinyProject.Singleton;
using TinyProject.Entities.Buildings;

namespace TinyProject.UI
{
    [RequireComponent(typeof(Image))]
    public class BuildSlot : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private PlayerRessourceSingleton playerRessourceSingleton;

        [BoxGroup("UI Settings")] [SerializeField] private bool canBuild = false;
        [BoxGroup("UI Settings")] [SerializeField] private Color normalColor = Color.white;
        [BoxGroup("UI Settings")] [SerializeField] private Color hoverColor = new Color32(0x5D, 0xD1, 0xFF, 0xFF);
        [BoxGroup("UI Settings")] [SerializeField] private Color disabledImageColor = new Color32(0x87, 0x87, 0x87, 0xFF);

        [BoxGroup("UI Settings")] [SerializeField] private GameObject buildingPrefab;
        [BoxGroup("UI Settings")] [SerializeField] private Image buildingImage;
        
        [BoxGroup("Building Costs")] [SerializeField] private int woodCost;
        [BoxGroup("Building Costs")] [SerializeField] private int goldCost;
        [BoxGroup("Building Costs")] [SerializeField] private int foodCost;

        [BoxGroup("Building Ghost Settings")] [SerializeField] private Transform constructionGhostParent;
        [BoxGroup("Building Ghost Settings")] [SerializeField] private Transform notConstructedParent;
        [BoxGroup("Building Ghost Settings")] [SerializeField] private LayerMask blockedLayers;
        [BoxGroup("Building Ghost Settings")] [SerializeField] private Color validColor = new Color(1f, 1f, 1f, 0.5f);
        [BoxGroup("Building Ghost Settings")] [SerializeField] private Color blockedColor = new Color(1f, 0f, 0f, 0.5f);

        private Image image;

        private static GameObject currentGhost;

        void Awake()
        {
            image = GetComponent<Image>();
            image.color = normalColor;
            UpdateBuildingImageColor();
        }

        void Start()
        {
            playerRessourceSingleton = PlayerRessourceSingleton.Instance;
        }

        void Update()
        {
            if (buildingPrefab == null) return;
            var buildingCost = buildingPrefab.GetComponent<Building>();
            if (buildingCost == null) return;

            woodCost = buildingCost.WoodCost;
            goldCost = buildingCost.GoldCost;
            foodCost = buildingCost.FoodCost;

            int currentWood = playerRessourceSingleton.Wood;
            int currentGold = playerRessourceSingleton.Gold;
            int currentFood = playerRessourceSingleton.Food;

            bool hasEnoughResources = currentWood >= woodCost && currentGold >= goldCost && currentFood >= foodCost;

            SetCanBuild(hasEnoughResources);
        }

        private void UpdateBuildingImageColor()
        {
            if (buildingImage == null) return;
            buildingImage.color = canBuild ? normalColor : disabledImageColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!canBuild) return;

            image.color = hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!canBuild) return;

            image.color = normalColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!canBuild) return;
            if (buildingPrefab == null) return;

            ClearGhost();

            currentGhost = Instantiate(buildingPrefab, constructionGhostParent.position, constructionGhostParent.rotation, constructionGhostParent);
            var buildingGhost = currentGhost.AddComponent<BuildingGhost>();

            buildingGhost.blockedLayers = blockedLayers;
            buildingGhost.validColor = validColor;
            buildingGhost.blockedColor = blockedColor;
            buildingGhost.buildingPrefab = buildingPrefab;
            buildingGhost.notConstructedParent = notConstructedParent;

            buildingGhost.woodCost = woodCost;
            buildingGhost.goldCost = goldCost;
            buildingGhost.foodCost = foodCost;
        }

        public static void ClearGhost()
        {
            if (currentGhost != null)
            {
                Destroy(currentGhost);
                currentGhost = null;
            }
        }

        public static void ClearGhostReference()
        {
            currentGhost = null;
        }

        public void SetCanBuild(bool value)
        {
            canBuild = value;
            UpdateBuildingImageColor();
        }
    }
}