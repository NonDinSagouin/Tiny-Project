using UnityEngine;
using Pathfinding;
using NaughtyAttributes;

namespace TinyProject.Entities
{
    [RequireComponent(typeof(IAstarAI))]
    public class Unit : Entity
    {
        [BoxGroup("Components")] private IAstarAI ai;
        [BoxGroup("Etats")] [SerializeField, ReadOnly] private bool isFlipped = false;

        protected override void Start()
        {
            base.Start();

            ai = GetComponent<IAstarAI>();
        }

        void Update()
        {
            UnitMovement.HandleMovement(animator, ai);
            UnitMovement.Flip(ai, transform, ref isFlipped);
        }

        /// <summary>
        /// Déplace l'unité vers la position spécifiée.
        /// </summary>
        /// <param name="position">La position vers laquelle déplacer l'unité.</param>
        public void MoveTo(Vector3 position)
        {
            UnitMovement.MoveTo(position, ai);
        }
    }
}