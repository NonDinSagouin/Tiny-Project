using UnityEngine;
using Pathfinding;
using NaughtyAttributes;

namespace TinyProject.Entities
{
    public enum EntityType
    {
        none,
        builder,
        chopper,
        miner
    }

    [RequireComponent(typeof(IAstarAI))]
    public class Pawn : Unit
    {
        [BoxGroup("Pawn")] [SerializeField] private EntityType entityType = EntityType.none;
        
        public EntityType EntityType => entityType;
    }
}