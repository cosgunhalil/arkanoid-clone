using UnityEngine;

namespace ArkanoidCloneProject.Physics
{
    public interface IPhysicsBody
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        float Radius { get; }
        PhysicsBodyType BodyType { get; }
        Collider2D Collider { get; }
        bool IsActive { get; }
        void OnCollision(CollisionData data);
    }
}
