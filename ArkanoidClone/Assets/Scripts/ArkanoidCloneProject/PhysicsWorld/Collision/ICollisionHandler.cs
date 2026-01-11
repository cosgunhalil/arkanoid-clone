using UnityEngine;

namespace ArkanoidCloneProject.Physics
{
    public interface ICollisionHandler
    {
        Vector2 CalculateReflection(Vector2 velocity, CollisionData collisionData);
    }
}
