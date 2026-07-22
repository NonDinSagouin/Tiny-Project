using Pathfinding;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

using TinyProject.Singleton;
using TinyProject.Enums;

namespace TinyProject.UI
{
    public class BuildingGhost : MonoBehaviour
    {
        public LayerMask blockedLayers;
        public Color validColor;
        public Color blockedColor;
        public GameObject buildingPrefab;

        public Transform notConstructedParent;

        public int woodCost;
        public int goldCost;
        public int foodCost;

        private Camera mainCamera;
        private SpriteRenderer[] spriteRenderers;
        private Collider2D[] ownColliders;
        private Vector2 overlapSize;

        private void Awake()
        {
            mainCamera = Camera.main;
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            ownColliders = GetComponentsInChildren<Collider2D>();

            Bounds bounds = new Bounds(transform.position, Vector3.zero);
            foreach (var sr in spriteRenderers)
                bounds.Encapsulate(sr.bounds);
            overlapSize = bounds.size;

            SetColor(validColor);
        }

        private void Update()
        {
            Vector2 mouseScreen = Mouse.current.position.ReadValue();
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, 0f));
            worldPos.z = 0f;
            transform.position = worldPos;

            foreach (var col in ownColliders) col.enabled = false;
            bool isBlocked = Physics2D.OverlapBox(transform.position, overlapSize, 0f, blockedLayers);
            foreach (var col in ownColliders) col.enabled = true;
            SetColor(isBlocked ? blockedColor : validColor);

            if (!isBlocked
                && Mouse.current.leftButton.wasPressedThisFrame
                && !EventSystem.current.IsPointerOverGameObject())
            {
                GameObject placed = Instantiate(buildingPrefab, transform.position, transform.rotation, notConstructedParent);

                if (woodCost > 0) PlayerRessourceSingleton.Instance.Spend(woodCost, ResourceType.Wood);
                if (goldCost > 0) PlayerRessourceSingleton.Instance.Spend(goldCost, ResourceType.Gold);
                if (foodCost > 0) PlayerRessourceSingleton.Instance.Spend(foodCost, ResourceType.Food);

                Bounds placedBounds = new Bounds(placed.transform.position, Vector3.zero);
                foreach (var sr in placed.GetComponentsInChildren<SpriteRenderer>())
                    placedBounds.Encapsulate(sr.bounds);
                AstarPath.active.UpdateGraphs(placedBounds);

                BuildSlot.ClearGhost();
            }
            else if (Keyboard.current.escapeKey.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
            {
                BuildSlot.ClearGhost();
            }
        }

        private void SetColor(Color color)
        {
            foreach (var sr in spriteRenderers)
                sr.color = color;
        }

        private void OnDestroy()
        {
            BuildSlot.ClearGhostReference();
        }
    }
}
