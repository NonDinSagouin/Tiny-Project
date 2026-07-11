using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

using TinyProject.Entities;

namespace TinyProject.Selection
{
    public class EntitySelectionBox : MonoBehaviour
    {
        private Camera cam;
    
        [BoxGroup("Selection Box")] [SerializeField] private RectTransform boxVisual;
        [BoxGroup("Selection Box")] [SerializeField] private float minSelectionSize = 20f;
    
        private bool isSelecting = false;
        private Rect selectionBox;
    
        private Vector2 startPosition = Vector2.zero;
        private Vector2 endPosition = Vector2.zero;
        
        void Start()
        {
            cam = Camera.main;
            startPosition = Vector2.zero;
            endPosition = Vector2.zero;
            DrawVisual();
        }

        void Update()
        {
            if (isSelecting)
            {
                endPosition = cam.WorldToScreenPoint(EntitySelectionSingleton.Instance.MouseWorldPosition);
                DrawVisual();
                SelectEntities();
            }
        }
    
        private void DrawVisual()
        {
            // Calcule les positions de départ et de fin du rectangle de sélection.
            Vector2 boxStart = startPosition;
            Vector2 boxEnd = endPosition;
    
            // Calcule le centre du rectangle de sélection.
            Vector2 boxCenter = (boxStart + boxEnd) / 2;
    
            // Place le visuel du rectangle de sélection à partir de son centre.
            boxVisual.position = boxCenter;
    
            // Calcule la taille du rectangle de sélection en largeur et en hauteur.
            Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));
    
            // Applique la taille calculée au visuel du rectangle de sélection.
            boxVisual.sizeDelta = boxSize;
        }
    
        private void SelectEntities()
        {
            Vector2 min = Vector2.Min(startPosition, endPosition);
            Vector2 max = Vector2.Max(startPosition, endPosition);
            selectionBox = Rect.MinMaxRect(min.x, min.y, max.x, max.y);

            if (selectionBox.width < minSelectionSize && selectionBox.height < minSelectionSize)
            {
                return;
            }

            List<Entity> selectedEntities = new();
            List<Entity> selectedUnits = new();

            foreach (Entity entity in EntitySelectionSingleton.Instance.GetAllEntity())
            {
                if (entity == null)
                {
                    continue;
                }

                Vector3 entityScreenPos = cam.WorldToScreenPoint(entity.transform.position);
                if (entityScreenPos.z < 0f)
                {
                    continue;
                }

                if (selectionBox.Contains(new Vector2(entityScreenPos.x, entityScreenPos.y)))
                {
                    selectedEntities.Add(entity);

                    if (entity is Unit)
                    {
                        selectedUnits.Add(entity);
                    }
                }
            }

            EntitySelectionSingleton.Instance.SetSelectedEntity(selectedUnits.Count > 0 ? selectedUnits : selectedEntities);
        }

        /// <summary>
        /// Commence la sélection des entités en définissant la position de départ du rectangle de sélection.
        /// </summary>
        /// <param name="mouseWorldPosition">La position de la souris dans le monde.</param>
        public void StartSelection(Vector3 mouseWorldPosition)
        {
            isSelecting = true;
            startPosition = cam.WorldToScreenPoint(mouseWorldPosition);
            endPosition = startPosition;
            boxVisual.gameObject.SetActive(true);
        }

        /// <summary>
        /// Termine la sélection des entités en définissant la position de fin du rectangle de sélection et en désactivant le visuel du rectangle de sélection.
        /// </summary>
        /// <param name="mouseWorldPosition">La position de la souris dans le monde.</param>
        public void EndSelection(Vector3 mouseWorldPosition)
        {
            isSelecting = false;
            endPosition = cam.WorldToScreenPoint(mouseWorldPosition);
            boxVisual.gameObject.SetActive(false);
        }
    }
}