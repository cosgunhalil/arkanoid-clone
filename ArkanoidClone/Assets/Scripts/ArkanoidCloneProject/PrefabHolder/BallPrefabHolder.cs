using UnityEngine;

namespace ArkanoidCloneProject.Physics
{
    public class BallPrefabHolder
    {
        public BallPrefabHolder(GameObject prefab)
        {
            Prefab = prefab;
        }

        public GameObject Prefab { get; }
    }
}