using UnityEngine;

namespace ArkanoidCloneProject.VFX
{
    public interface IBrickExplosionEffect
    {
        void Spawn(Vector3 worldPosition, Vector3 worldSize, Color color);
    }
}
