using UnityEngine;
using NaughtyAttributes;

namespace TinyProject.Resource
{
    /// <summary>
    /// Représente les différents types de ressources disponibles dans le jeu.
    /// </summary>
    public enum ResourceType
    {
        Wood,
        Gold,
        Food
    }

    public class Resource : MonoBehaviour
    {
        [BoxGroup("Resource Info")] [SerializeField] private ResourceType resourceType;
        [BoxGroup("Resource Info")] [SerializeField] private int resourceAmount = 100;
        [BoxGroup("Resource Info")] [SerializeField] private int currentResourceAmount;

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
        /// Retourne le type de ressource de la ressource.
        /// </summary>
        /// <returns>Le type de ressource de la ressource.</returns>
        public ResourceType GetResourceType()
        {
            return resourceType;
        }

        /// <summary>
        /// Retourne la quantité totale de ressource de la ressource.
        /// </summary>
        /// <returns>La quantité totale de ressource de la ressource.</returns>
        public int GetResourceAmount()
        {
            return resourceAmount;
        }

        /// <summary>
        /// Retourne la quantité actuelle de ressource de la ressource.
        /// </summary>
        /// <returns>La quantité actuelle de ressource de la ressource.</returns>
        public int GetCurrentResourceAmount()
        {
            return currentResourceAmount;
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
