using UnityEngine;
using Pathfinding;
using NaughtyAttributes;

using TinyProject.Singleton;

namespace TinyProject.Entities.Buildings
{
    public class Building : Entity
    {
        [BoxGroup("Building Info")] [SerializeField] private int constructionTime;
        [BoxGroup("Building Info")] [SerializeField] private int woodCost;
        [BoxGroup("Building Info")] [SerializeField] private int goldCost;
        [BoxGroup("Building Info")] [SerializeField] private int foodCost;

        public int ConstructionTime => constructionTime;
        public int WoodCost => woodCost;
        public int GoldCost => goldCost;
        public int FoodCost => foodCost;

        public bool CanAfford()
        {
            return PlayerRessourceSingleton.Instance.Wood >= woodCost &&
                   PlayerRessourceSingleton.Instance.Gold >= goldCost &&
                   PlayerRessourceSingleton.Instance.Food >= foodCost;
        }
    }
}