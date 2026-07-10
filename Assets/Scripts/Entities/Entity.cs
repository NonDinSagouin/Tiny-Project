using UnityEngine;
using Pathfinding;
using NaughtyAttributes;

using TinyProject.Selection;

namespace TinyProject.Entities
{
    public class Entity : MonoBehaviour
    {
        [BoxGroup("Components")] [SerializeField] protected Animator animator;
        [BoxGroup("Components")] [SerializeField] protected GameObject selectionVisual;

        [BoxGroup("Etats")] [SerializeField, ReadOnly] protected bool isSelected;

        /// <summary>
        /// Indique si l'unité est actuellement sélectionnée.
        /// </summary>
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                
                if (selectionVisual != null)
                {
                    selectionVisual.SetActive(isSelected);
                }
            }
        }

        protected virtual void Start()
        {
            EntitySelectionSingleton.Instance.RegisterEntity(this);
        }

        protected virtual void OnDestroy()
        {
            if (EntitySelectionSingleton.Instance != null)
            {
                EntitySelectionSingleton.Instance.DeregisterEntity(this);
            }
        }
    }
}