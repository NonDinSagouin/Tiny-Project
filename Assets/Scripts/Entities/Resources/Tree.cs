using UnityEngine;
using NaughtyAttributes;

using TinyProject.Resources;

namespace TinyProject.Entities.Resources
{
    [RequireComponent(typeof(ResourceCollector))]
    public class Tree : Entity
    {
        [BoxGroup("Components")] [SerializeField] private ResourceCollector resourceCollector;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (resourceCollector != null && resourceCollector.IsDepleted())
            {
                Destroy(gameObject);
            }
        }
    }
}