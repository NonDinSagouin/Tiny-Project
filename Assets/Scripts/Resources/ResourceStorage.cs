using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

using TinyProject.Enums;

namespace TinyProject.Resources
{
    public class ResourceStorage : MonoBehaviour
    {
        [BoxGroup("Resource Info")] [SerializeField] private int wood;
        [BoxGroup("Resource Info")] [SerializeField] private int gold;
        [BoxGroup("Resource Info")] [SerializeField] private int food;

        [BoxGroup("Resource Info")] [SerializeField] private List<Transform> storagePoints;

        public int Wood => wood;
        public int Gold => gold;
        public int Food => food;
        public List<Transform> StoragePoints => storagePoints;

        void Start()
        {
        }
    }
}
