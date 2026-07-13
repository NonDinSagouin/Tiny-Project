using UnityEngine;
using Pathfinding;
using NaughtyAttributes;

namespace TinyProject.Entities
{
    [RequireComponent(typeof(AIPath))]
    public class Unit : Entity
    {
        public IdleState IdleState { get; private set; }
        public WalkingState WalkingState { get; private set; }

        [BoxGroup("Components")] protected IAstarAI ai;
        [BoxGroup("Etats")] [SerializeField, ReadOnly] protected bool isFlipped = false;
        // Seuil de vitesse pour déterminer si l'unité est en mouvement
        [BoxGroup("Parameters")][SerializeField] private const float VelocityThreshold = 0.1f;

        protected override void Start()
        {
            base.Start();
            ai = GetComponent<IAstarAI>();

            IdleState = new IdleState(this);
            WalkingState = new WalkingState(this);

            IdleState.Init(animator);
            WalkingState.Init(animator);

            ChangeState(IdleState);
        }

        protected override void Update()
        {
            base.Update();
            
            if (ai.velocity.magnitude > VelocityThreshold)
            {
                ChangeState(WalkingState);
            }
            else
            {
                ChangeState(IdleState);
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