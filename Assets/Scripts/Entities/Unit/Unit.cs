using System.Linq;
using UnityEngine;
using Pathfinding;
using NaughtyAttributes;

using TinyProject.StateMachine.Entities;
using TinyProject.Resources;

namespace TinyProject.Entities
{
    [RequireComponent(typeof(AIPath))]
    public class Unit : Entity
    {
        public IdleState IdleState { get; private set; }
        public WalkingState WalkingState { get; private set; }

        [BoxGroup("Components")] protected IAstarAI ai;
        [BoxGroup("Etats")] [SerializeField, ReadOnly] protected bool isFlipped = false;
        [BoxGroup("Etats")] [SerializeField, ReadOnly] protected bool isInAction = false;
        [BoxGroup("Target")] [SerializeField] protected GameObject targetGameObject;
        [BoxGroup("Target")] [SerializeField] protected float targetDistanceThreshold = 1f;
        public GameObject TargetGameObject => targetGameObject;
        
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
            
            if (isInAction)
            {
                return;
            }

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
            targetGameObject = target;
        }

        /// <summary>
        /// Vérifie si l'unité est proche de sa cible actuelle.
        /// </summary>
        /// <param name="maxDistance">Distance maximale pour considérer la cible comme proche.</param>
        /// <returns>True si une cible existe et qu'elle est à portée, sinon false.</returns>
        public bool IsNearTargetGameObject()
        {
            if (targetGameObject == null)
            {
                return false;
            }

            float sqrDistance = (transform.position - targetGameObject.transform.position).sqrMagnitude;
            float sqrMaxDistance = targetDistanceThreshold * targetDistanceThreshold;
            return sqrDistance <= sqrMaxDistance;
        }

        /// <summary>
        /// Déplace l'unité vers la position de sa cible actuelle.
        /// </summary>
        /// <returns>True si une cible existe et que le déplacement est lancé, sinon false.</returns>
        public void MoveToTargetGameObject()
        {
            if (targetGameObject == null)
            {
                return;
            }

            Vector2 targetPosition = targetGameObject.transform.position;

            if (targetGameObject.TryGetComponent(out ResourceCollector resource))
            {
                SetTargetGameObject(resource.gameObject);
                targetPosition = resource.ExtractionPoints.FirstOrDefault()?.position ?? targetPosition;
            }
            if (targetGameObject.TryGetComponent(out ResourceStorage storage))
            {
                SetTargetGameObject(storage.gameObject);
                targetPosition = storage.StoragePoints.FirstOrDefault()?.position ?? targetPosition;
            }

            MoveTo(targetPosition);
        }

        /// <summary>
        /// Déplace l'unité vers la position spécifiée.
        /// </summary>
        /// <param name="position">La position vers laquelle déplacer l'unité.</param>
        public virtual void MoveTo(Vector3 position)
        {
            ai.destination = position;
            ai.SearchPath();
        }
    }
}