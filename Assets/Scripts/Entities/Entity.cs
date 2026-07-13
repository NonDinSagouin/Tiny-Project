using System;
using UnityEngine;
using Pathfinding;
using NaughtyAttributes;

using TinyProject.Selection;

namespace TinyProject.Entities
{
    public class Entity : MonoBehaviour
    {
        State State { get; set; }

        [BoxGroup("Components")] [SerializeField] protected Animator animator;
        [BoxGroup("Components")] [SerializeField] protected GameObject selectionVisual;
        [BoxGroup("Components")] [SerializeField] protected Collider2D entityColliderSelection;

        [BoxGroup("Etats")] [SerializeField, ReadOnly] protected bool isSelected;

        /// <summary>
        /// Obtient le collider utilisé pour la sélection de l'entité.
        /// </summary>
        public Collider2D EntityColliderSelection => entityColliderSelection; 

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
            if (EntitySelectionSingleton.Instance == null)
            {
                throw new InvalidOperationException("EntitySelectionSingleton instance est null. Assurez-vous qu'il y a un GameObject avec le script EntitySelectionSingleton dans la scène.");
            }

            EntitySelectionSingleton.Instance.RegisterEntity(this);
        }

        protected virtual void Update()
        {
            State?.Tick();
        }

        protected virtual void FixedUpdate()
        {
            State?.FixedTick();
        }

        protected virtual void OnDestroy()
        {
            if (EntitySelectionSingleton.Instance != null)
            {
                EntitySelectionSingleton.Instance.DeregisterEntity(this);
            }
        }
    
        /// <summary>
        /// Change l'état actuel de l'entité vers un nouvel état spécifié.
        /// </summary>
        /// <param name="newState">Le nouvel état vers lequel changer.</param>
        protected void ChangeState(State newState)
        {
            if (State == newState) return;

            State?.Exit();
            State = newState;
            State?.Enter();
        }
    }
}