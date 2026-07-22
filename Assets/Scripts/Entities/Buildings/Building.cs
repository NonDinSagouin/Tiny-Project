using UnityEngine;
using Pathfinding;
using NaughtyAttributes;

using TinyProject.Singleton;
using System.Collections.Generic;

namespace TinyProject.Entities.Buildings
{
    public class Building : Entity
    {
        [BoxGroup("Building Info")] [SerializeField] private bool isConstructed = false;
        [BoxGroup("Building Info")] [SerializeField] private Color constructionColor = new Color(1f, 1f, 1f, 0.5f);
        [BoxGroup("Building Info")] [SerializeField] private int constructionValues;
        [BoxGroup("Building Info")] [SerializeField] private int woodCost;
        [BoxGroup("Building Info")] [SerializeField] private int goldCost;
        [BoxGroup("Building Info")] [SerializeField] private int foodCost;

        [BoxGroup("Resource Info")] [SerializeField] private List<Transform> constructionPoints;

        public bool IsConstructed => isConstructed;
        public int ConstructionValues => constructionValues;
        public int WoodCost => woodCost;
        public int GoldCost => goldCost;
        public int FoodCost => foodCost;
        public List<Transform> ConstructionPoints => constructionPoints;

        protected override void Start()
        {
            base.Start();

            if (!isConstructed)
            {
                SetBuildingActive(false);
            }
        }

        public bool CanAfford()
        {
            return PlayerRessourceSingleton.Instance.Wood >= woodCost &&
                   PlayerRessourceSingleton.Instance.Gold >= goldCost &&
                   PlayerRessourceSingleton.Instance.Food >= foodCost;
        }

        

        [Button("Finish Construction")]
        public void FinishConstruction()
        {
            SetBuildingActive(true);
        }

        public void SetBuildingActive(bool active)
        {
            isConstructed = active;
            selfCollider.enabled = active;

            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
                sr.color = active ? Color.white : constructionColor;

            Bounds placedBounds = new Bounds(transform.position, Vector3.zero);
            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
                placedBounds.Encapsulate(sr.bounds);
            AstarPath.active.UpdateGraphs(placedBounds);
        }
    }
}