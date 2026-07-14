using UnityEngine;
using Pathfinding;
using NaughtyAttributes;

using TinyProject.StateMachine.Entities;

namespace TinyProject.Entities
{
    [RequireComponent(typeof(AIPath))]
    public class Unit : Entity
    {
        public IdleState IdleState { get; private set; }
        public WalkingState WalkingState { get; private set; }

        [BoxGroup("Components")] protected IAstarAI ai;
        [BoxGroup("Etats")] [SerializeField, ReadOnly] protected bool isFlipped = false;
        [BoxGroup("Target")] [SerializeField] private GameObject TargetGameObject;
        
        // Seuil de vitesse pour déterminer si l'unité est en mouvement
        private const float VelocityThreshold = 0.1f;

        protected override void Start()
        {
            base.Start();
            ai = GetComponent<IAstarAI>();

            IdleState = new IdleState(this);
            WalkingState = new WalkingState(this);

            IdleState.Init(animator);
            WalkingState.Init(animator);

            ChangeEntityState(IdleState);
        }

        protected override void Update()
        {
            base.Update();
            
            if (ai.velocity.magnitude > VelocityThreshold)
            {
                ChangeEntityState(WalkingState);
            }
            else
            {
                ChangeEntityState(IdleState);
            }

            Flip(ai, transform, ref isFlipped);
        }

        protected static void Flip(IAstarAI ai, Transform transform, ref bool isFlipped)
        {
            if (ai != null && ai.velocity.magnitude > VelocityThreshold)
            {
                bool movingRight = ai.velocity.x > 0f;
                bool shouldBeFlipped = !movingRight; // par défaut face à droite, on flip seulement vers la gauche
                if (shouldBeFlipped != isFlipped)
                {
                    isFlipped = shouldBeFlipped;
                    Vector3 scale = transform.localScale;
                    scale.x *= -1;
                    transform.localScale = scale;
                }
            }
        }

        /// <summary>
        /// Définit l'objet cible pour l'unité. Cela peut être utilisé pour interagir avec des ressources, des bâtiments ou d'autres entités dans le jeu.
        /// </summary>
        /// <param name="target">L'objet cible à définir pour l'unité.</param>
        public void SetTargetGameObject(GameObject target)
        {
            TargetGameObject = target;
        }

        /// <summary>
        /// Déplace l'unité vers la position spécifiée.
        /// </summary>
        /// <param name="position">La position vers laquelle déplacer l'unité.</param>
        public void MoveTo(Vector3 position)
        {
            ai.destination = position;
            ai.SearchPath();
        }
    }
}