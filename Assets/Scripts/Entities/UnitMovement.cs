using UnityEngine;
using Pathfinding;

namespace TinyProject.Entities
{
    public class UnitMovement : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        public static void HandleMovement(Animator animator, IAstarAI ai)
        {
            if (ai != null)
            {
                animator.SetBool("isWalking", ai.velocity.magnitude > 0.1f);
            }
        }

        public static void Flip(IAstarAI ai, Transform transform, ref bool isFlipped)
        {
            if (ai != null && ai.velocity.magnitude > 0.1f)
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

        public static void MoveTo(Vector3 position, IAstarAI ai)
        {
            ai.destination = position;
            ai.SearchPath();
        }
    }
}
