using UnityEngine;

namespace ArkanoidCloneProject.Physics
{
    public class StandardCollisionHandler : ICollisionHandler
    {
        public Vector2 CalculateReflection(Vector2 velocity, CollisionData collisionData)
        {
            return Vector2.Reflect(velocity, collisionData.Normal);
        }
    }
}
