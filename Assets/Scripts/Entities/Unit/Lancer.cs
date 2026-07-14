using UnityEngine;
using Pathfinding;
using NaughtyAttributes;

namespace TinyProject.Entities
{
    [RequireComponent(typeof(IAstarAI))]
    public class Lancer : Unit
    {
        protected override void Update()
        {
            base.Update();
        }
    }
}