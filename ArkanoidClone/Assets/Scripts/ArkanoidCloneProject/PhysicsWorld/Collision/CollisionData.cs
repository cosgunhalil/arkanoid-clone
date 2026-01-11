using UnityEngine;

namespace ArkanoidCloneProject.Physics
{
    public struct CollisionData
    {
        public Vector2 ContactPoint;
        public Vector2 Normal;
        public IPhysicsBody OtherBody;
        public Collider2D OtherCollider;
        public PhysicsBodyType OtherBodyType;
        public float Distance;

        public CollisionData(Vector2 contactPoint, Vector2 normal, IPhysicsBody otherBody, 
            Collider2D otherCollider, PhysicsBodyType otherBodyType, float distance)
        {
            ContactPoint = contactPoint;
            Normal = normal;
            OtherBody = otherBody;
            OtherCollider = otherCollider;
            OtherBodyType = otherBodyType;
            Distance = distance;
        }
    }
}
