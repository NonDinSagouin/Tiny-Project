using UnityEngine;
using NaughtyAttributes;

using TinyProject.Entities;
using System.Collections.Generic;

namespace TinyProject.Resources
{
    /// <summary>
    /// Représente les différents types de ressources disponibles dans le jeu.
    /// </summary>
    public enum ResourceType
    {
        None,
        Wood,
        Gold,
        Food
    }

    public class ResourceCollector : MonoBehaviour
    {
        [BoxGroup("Resource Info")] [SerializeField] private ResourceType resourceType;
        [BoxGroup("Resource Info")] [SerializeField] private WorkerRole workerRole;
        [BoxGroup("Resource Info")] [SerializeField] private int resourceAmount = 100;
        [BoxGroup("Resource Info")] [SerializeField] private int currentResourceAmount;

        [BoxGroup("Resource Info")] [SerializeField] private List<Transform> extractionPoints;

        public ResourceType ResourceType => resourceType;
        public WorkerRole WorkerRole => workerRole;
        public int ResourceAmount => resourceAmount;
        public int CurrentResourceAmount => currentResourceAmount;
        public List<Transform> ExtractionPoints => extractionPoints;

        void Start()
        {
            currentResourceAmount = resourceAmount;
        }

        /// <summary>
        /// Réduit la quantité de ressource actuelle de la ressource.
        /// </summary>
        /// <param name="amount">La quantité de ressource à retirer.</param>
        public void Take(int amount)
        {
            currentResourceAmount -= amount;
        }

        /// <summary>
        /// Retourne une représentation sous forme de chaîne de caractères de l'objet Resource, incluant le type de ressource, la quantité totale et la quantité actuelle.
        /// </summary>
        /// <returns>Une chaîne de caractères représentant l'objet Resource.</returns>
        public override string ToString()
        {
            return $"Resource Type: {resourceType}, Total Amount: {resourceAmount}, Current Amount: {currentResourceAmount}";
        }
    }
}
