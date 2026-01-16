using UnityEngine;

namespace ArkanoidCloneProject.Physics
{
    public class BallPrefabHolder
    {
        public GameObject Prefab { get; }
        
        public BallPrefabHolder(GameObject prefab)
        {
            Prefab = prefab;
        }
    }
}
