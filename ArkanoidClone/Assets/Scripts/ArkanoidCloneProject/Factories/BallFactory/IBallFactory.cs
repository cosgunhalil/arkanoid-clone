using UnityEngine;

namespace ArkanoidCloneProject.Physics
{
    public interface IBallFactory
    {
        Ball Create(Vector2 position);
    }
}
