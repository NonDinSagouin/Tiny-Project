using UnityEngine;
using Pathfinding;
using NaughtyAttributes;

using TinyProject.Singleton;
using System.Collections.Generic;

namespace TinyProject.Entities.Buildings
{
    public class Building : Entity
    {
        [BoxGroup("Stats Info")] [SerializeField] private bool isConstructed = false;
        [BoxGroup("Stats Info")] [SerializeField] private int healthMax = 100;
        [BoxGroup("Stats Info")] [SerializeField] private int healthCurrent = 100;

        [BoxGroup("Building Info")] [SerializeField] private Transform notConstructedParent;
        [BoxGroup("Building Info")] [SerializeField] private Color constructionColor = new Color(1f, 1f, 1f, 0.5f);
        [BoxGroup("Building Info")] [SerializeField] private int woodCost;
        [BoxGroup("Building Info")] [SerializeField] private int goldCost;
        [BoxGroup("Building Info")] [SerializeField] private int foodCost;

        [BoxGroup("Building Animation")] [SerializeField, Min(0.01f)] private float constructionTickScaleDuration = 0.12f;
        [BoxGroup("Building Animation")] [SerializeField, Min(0.01f)] private Vector3 constructionTickScale = new Vector3(1.05f, 0.98f, 1f);

        [BoxGroup("Resource Info")] [SerializeField] private List<Transform> constructionPoints;

        public bool IsConstructed => isConstructed;
        public int HealthMax => healthMax;
        public int HealthCurrent => healthCurrent;

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
            else
            {
                SetBuildingActive(true);
                healthCurrent = healthMax;
            }
        }

        public bool CanAfford()
        {
            return PlayerRessourceSingleton.Instance.Wood >= woodCost &&
                   PlayerRessourceSingleton.Instance.Gold >= goldCost &&
                   PlayerRessourceSingleton.Instance.Food >= foodCost;
        }

        public void Construct(int constructionValue)
        {
            if (!isConstructed)
            {
                healthCurrent += constructionValue;
                PlayConstructionTickAnimation();

                if (healthCurrent >= healthMax)
                {
                    FinishConstruction();
                }
            }
        }

        private void PlayConstructionTickAnimation()
        {
            LeanTween.cancel(gameObject);

            LeanTween.scale(gameObject, constructionTickScale, constructionTickScaleDuration)
                .setEaseOutQuad()
                .setOnComplete(() => LeanTween.scale(gameObject, new Vector3(1f, 1f, 1f), constructionTickScaleDuration).setEaseInQuad());
        }

        [Button("Finish Construction")]
        public void FinishConstruction()
        {
            healthCurrent = healthMax;
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