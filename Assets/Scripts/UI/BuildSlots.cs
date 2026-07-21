using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TinyProject.UI
{
    [RequireComponent(typeof(Image))]
    public class BuildSlots : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = new Color32(0x5D, 0xD1, 0xFF, 0xFF);
        [SerializeField] private GameObject buildingPrefab;

        private Image image;

        private static GameObject currentGhost;

        private void Awake()
        {
            image = GetComponent<Image>();
            image.color = normalColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            image.color = hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            image.color = normalColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (buildingPrefab == null) return;

            ClearGhost();

            currentGhost = Instantiate(buildingPrefab);
            currentGhost.AddComponent<BuildingGhost>();
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
    }
}