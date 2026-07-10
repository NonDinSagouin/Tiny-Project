using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;

using static UnityEngine.InputSystem.InputAction;

using TinyProject.Entities;

namespace TinyProject.Selection
{
    public class EntitySelectionSingleton : MonoBehaviour
    {
        public static EntitySelectionSingleton Instance { get; private set; }

        [BoxGroup("Components")] [SerializeField] private Camera cam;

        [BoxGroup("Selection")] [SerializeField] private EntitySelectionBox entitySelectionBox;
        [BoxGroup("Selection")] [SerializeField] private UnitSelectionFormation entityFormation;

        [BoxGroup("Units")] [SerializeField] private List<Entity> allUnitsList = new();
        [BoxGroup("Units")] [SerializeField] private List<Entity> unitesSelected = new();
        [BoxGroup("Units")] [SerializeField] private FormationType currentFormationType = FormationType.Square;

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
            foreach (Entity unit in unitesSelected)
            {
                unit.IsSelected = false;
            }

            unitesSelected.Clear();
        }

        /// <summary>
        /// Enregistre une entité dans la liste de toutes les entités.
        /// Cela permet de suivre toutes les entités disponibles pour la sélection.
        /// </summary>
        /// <param name="unit">L'unité à enregistrer.</param>
        public void RegisterEntity(Entity unit)
        {
            if (unit != null && !allUnitsList.Contains(unit))
            {
                allUnitsList.Add(unit);
            }
        }

        /// <summary>
        /// Désenregistre une entité de la liste de toutes les entités.
        /// Cela permet de retirer une entité de la sélection lorsqu'elle n'est plus disponible.
        /// </summary>
        /// <param name="unit">L'entité à désenregistrer.</param>
        public void DeregisterEntity(Entity unit)
        {
            if (unit != null && allUnitsList.Contains(unit))
            {
                allUnitsList.Remove(unit);
            }
        }

        /// <summary>
        /// Compte le nombre d'entité actuellement sélectionnées.
        /// </summary>
        /// <returns>Le nombre d'entité actuellement sélectionnées.</returns>
        public int CountSelectedEntity()
        {
            return unitesSelected.Count;
        }

        /// <summary>
        /// Obtient la liste de toutes les entité actuellement enregistrées.
        /// </summary>
        /// <returns>La liste de toutes les entité actuellement enregistrées.</returns>
        public List<Entity> GetAllEntity()
        {
            return new List<Entity>(allUnitsList);
        }

        /// <summary>
        /// Obtient la liste des entités actuellement sélectionnées.
        /// </summary>
        /// <returns>La liste des entités actuellement sélectionnées.</returns>
        public List<Entity> GetSelectedEntity()
        {
            return new List<Entity>(unitesSelected);
        }

        /// <summary>
        /// Sélectionne une entité spécifique et la marque comme sélectionnée.
        /// </summary>
        /// <param name="unit">L'entité à sélectionner.</param>
        public void SelectEntity(Entity unit)
        {
            if (unit == null)
            {
                return;
            }

            DeselectAllEntity();
            unitesSelected.Add(unit);
            unit.IsSelected = true;
        }

        /// <summary>
        /// Sélectionne une liste d'entités et les marque comme sélectionnées.
        /// </summary>
        /// <param name="units">La liste des entités à sélectionner.</param>
        public void SetSelectedEntity(List<Entity> units)
        {
            DeselectAllEntity();

            if (units == null)
            {
                return;
            }

            foreach (Entity unit in units)
            {
                if (unit == null || unitesSelected.Contains(unit))
                {
                    continue;
                }

                unitesSelected.Add(unit);
                unit.IsSelected = true;
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
            if (hit.collider != null && hit.collider.GetComponentInParent<Entity>() is Entity unit)
            {
                DeselectAllEntity();
                SelectEntity(unit);
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
        /// [Event] Gère le mouvement des entité sélectionnées en fonction de l'entrée utilisateur.
        /// </summary>
        /// <param name="context"></param>
        public void HandleInputMovement(CallbackContext context)
        {
            if (!context.performed || unitesSelected.Count == 0)
            {
                return;
            }

            List<Unit> units = unitesSelected.ConvertAll(u => u as Unit);

            if (unitesSelected.Count == 1)
            {
                entityFormation.NoFormation(mouseWorldPosition, units[0]);
                return;
            }

            switch (currentFormationType)
            {
                case FormationType.VerticalLine:
                    entityFormation.LineFormation(mouseWorldPosition, units, false);
                    break;
                case FormationType.HorizontalLine:
                    entityFormation.LineFormation(mouseWorldPosition, units, true);
                    break;

                case FormationType.Square:
                    entityFormation.SquareFormation(mouseWorldPosition, units);
                    break;

                case FormationType.Circle:
                    entityFormation.CircleFormation(mouseWorldPosition, units);
                    break;
            }
        }
    }
}