using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;

using static UnityEngine.InputSystem.InputAction;

using TinyProject.Entities;
using TinyProject.Resources;
using TinyProject.Selection;

namespace TinyProject.Singleton
{
    public class EntitySelectionSingleton : MonoBehaviour
    {
        public static EntitySelectionSingleton Instance { get; private set; }

        [BoxGroup("Components")] [SerializeField] private Camera cam;

        [BoxGroup("Selection")] [SerializeField] private EntitySelectionBox entitySelectionBox;
        [BoxGroup("Selection")] [SerializeField] private UnitSelectionFormation entityFormation;

        [BoxGroup("Entities")] [SerializeField] private List<Entity> allEntitiesList = new();
        [BoxGroup("Entities")] [SerializeField] private List<Entity> entitiesSelected = new();
        [BoxGroup("Entities")] [SerializeField] private FormationType currentFormationType = FormationType.Square;

        [BoxGroup("Mouse")] [SerializeField] private Vector3 mouseWorldPosition;

        public Vector3 MouseWorldPosition => mouseWorldPosition;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            cam = Camera.main;
        }

        void Update()
        {
            Camera activeCamera = cam != null ? cam : Camera.main;
            if (activeCamera == null)
            {
                return;
            }

            Vector2 screenPos = Mouse.current.position.ReadValue();
            mouseWorldPosition = activeCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        }

        private void DeselectAllEntity()
        {
            foreach (Entity entity in entitiesSelected)
            {
                entity.IsSelected = false;
            }

            entitiesSelected.Clear();
        }

        /// <summary>
        /// Enregistre une entité dans la liste de toutes les entités.
        /// Cela permet de suivre toutes les entités disponibles pour la sélection.
        /// </summary>
        /// <param name="entity">L'entité à enregistrer.</param>
        public void RegisterEntity(Entity entity)
        {
            if (entity != null && !allEntitiesList.Contains(entity))
            {
                allEntitiesList.Add(entity);
            }
        }

        /// <summary>
        /// Désenregistre une entité de la liste de toutes les entités.
        /// Cela permet de retirer une entité de la sélection lorsqu'elle n'est plus disponible.
        /// </summary>
        /// <param name="entity">L'entité à désenregistrer.</param>
        public void DeregisterEntity(Entity entity)
        {
            if (entity != null && allEntitiesList.Contains(entity))
            {
                allEntitiesList.Remove(entity);
            }
        }

        /// <summary>
        /// Compte le nombre d'entité actuellement sélectionnées.
        /// </summary>
        /// <returns>Le nombre d'entité actuellement sélectionnées.</returns>
        public int CountSelectedEntity()
        {
            return entitiesSelected.Count;
        }

        /// <summary>
        /// Obtient la liste de toutes les entité actuellement enregistrées.
        /// </summary>
        /// <returns>La liste de toutes les entité actuellement enregistrées.</returns>
        public List<Entity> GetAllEntity()
        {
            return new List<Entity>(allEntitiesList);
        }

        /// <summary>
        /// Obtient la liste des entités actuellement sélectionnées.
        /// </summary>
        /// <returns>La liste des entités actuellement sélectionnées.</returns>
        public List<Entity> GetSelectedEntity()
        {
            return new List<Entity>(entitiesSelected);
        }

        /// <summary>
        /// Sélectionne une entité spécifique et la marque comme sélectionnée.
        /// </summary>
        /// <param name="entity">L'entité à sélectionner.</param>
        public void SelectEntity(Entity entity)
        {
            if (entity == null)
            {
                return;
            }

            DeselectAllEntity();
            entitiesSelected.Add(entity);
            entity.IsSelected = true;
        }

        /// <summary>
        /// Sélectionne une liste d'entités et les marque comme sélectionnées.
        /// </summary>
        /// <param name="entities">La liste des entités à sélectionner.</param>
        public void SetSelectedEntity(List<Entity> entities)
        {
            DeselectAllEntity();

            if (entities == null)
            {
                return;
            }

            foreach (Entity entity in entities)
            {
                if (entity == null || entitiesSelected.Contains(entity))
                {
                    continue;
                }

                entitiesSelected.Add(entity);
                entity.IsSelected = true;
            }
        }

        /// <summary>
        /// [Event] Gère la sélection d'entité en fonction de l'entrée utilisateur.
        /// </summary>
        /// <param name="context">Le contexte de l'action d'entrée utilisateur.</param>
        public void HandleInputSelection(CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);
            if (hit.collider != null && hit.collider.GetComponentInParent<Entity>() is Entity entity)
            {
                DeselectAllEntity();
                SelectEntity(entity);
            }
            else
            {
                DeselectAllEntity();
            }
        }

        /// <summary>
        /// [Event] Gère la sélection d'entité par glisser-déposer en fonction de l'entrée utilisateur.
        /// </summary>
        /// <param name="context">Le contexte de l'action d'entrée utilisateur.</param>
        public void HandleInputDragSelection(CallbackContext context)
        {   
            if (context.phase == InputActionPhase.Started)
            {
                entitySelectionBox.StartSelection(mouseWorldPosition);
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                entitySelectionBox.EndSelection(mouseWorldPosition);
            }
        }

        /// <summary>
        /// [Event] Gère le mouvement des entités sélectionnées en fonction de l'entrée utilisateur.
        /// Cela permet de déplacer les entités sélectionnées vers la position de la souris.
        /// </summary>
        /// <param name="context">Le contexte de l'action d'entrée utilisateur.</param>
        public void HandleInputAction(CallbackContext context)
        {
            if (!context.performed){ return; }
            if (entitiesSelected.Count == 0){ return; }

            Vector2 targetPosition = mouseWorldPosition;

            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);
            if (hit.collider != null && hit.collider.GetComponentInParent<Entity>() is Entity clickedEntity && !entitiesSelected.Contains(clickedEntity))
            {
                if (clickedEntity.TryGetComponent(out ResourceCollector resource))
                {
                    SetTargetGameObject(resource.gameObject);
                    targetPosition = resource.ExtractionPoints.FirstOrDefault()?.position ?? targetPosition;
                }
                if (clickedEntity.TryGetComponent(out ResourceStorage storage))
                {
                    SetTargetGameObject(storage.gameObject);
                    targetPosition = storage.StoragePoints.FirstOrDefault()?.position ?? targetPosition;
                }
            }
            else
            {
                SetTargetGameObject(null);
            }

            Movement(targetPosition);
        }

        /// <summary>
        /// Gère le mouvement des entité sélectionnées en fonction de l'entrée utilisateur.
        /// Uniquement pour les unités, les autres entités ne sont pas concernées par le mouvement.
        /// </summary>
        /// <param name="context"></param>
        public void Movement(Vector3 targetPosition)
        {
            List<Unit> units = entitiesSelected.OfType<Unit>().ToList();

            if (units.Count == 0)
            {
                return;
            }

            if (units.Count == 1)
            {
                entityFormation.NoFormation(targetPosition, units[0]);
                return;
            }

            switch (currentFormationType)
            {
                case FormationType.VerticalLine:
                    entityFormation.LineFormation(targetPosition, units, false);
                    break;
                case FormationType.HorizontalLine:
                    entityFormation.LineFormation(targetPosition, units, true);
                    break;

                case FormationType.Square:
                    entityFormation.SquareFormation(targetPosition, units);
                    break;

                case FormationType.Circle:
                    entityFormation.CircleFormation(targetPosition, units);
                    break;
            }
        }

        /// <summary>
        /// Définit l'objet cible pour toutes les unités actuellement sélectionnées.
        /// Cela peut être utilisé pour interagir avec des ressources, des bâtiments ou d'autres entités dans le jeu.
        /// </summary>
        /// <param name="target">L'objet cible à définir pour les unités sélectionnées.</param>
        public void SetTargetGameObject(GameObject target)
        {
            foreach (Entity entity in entitiesSelected)
            {
                if (entity is Unit unit)
                {
                    unit.SetTargetGameObject(target);
                }
            }
        }
    }
}