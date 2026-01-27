using UnityEngine;

namespace ArkanoidCloneProject.Physics
{
    public interface IBallFactory
    {
        Ball.Ball Create(Vector2 position);
    }
}