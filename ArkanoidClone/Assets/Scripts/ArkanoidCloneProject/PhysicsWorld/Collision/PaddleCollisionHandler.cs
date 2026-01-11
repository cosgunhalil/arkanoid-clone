using UnityEngine;

namespace ArkanoidCloneProject.Physics
{
    public class PaddleCollisionHandler : ICollisionHandler
    {
        private readonly float _maxBounceAngle;
        private readonly float _minVerticalVelocity;

        public PaddleCollisionHandler(float maxBounceAngle = 75f, float minVerticalVelocity = 0.3f)
        {
            _maxBounceAngle = maxBounceAngle;
            _minVerticalVelocity = minVerticalVelocity;
        }

        public Vector2 CalculateReflection(Vector2 velocity, CollisionData collisionData)
        {
            float speed = velocity.magnitude;
            
            Collider2D paddleCollider = collisionData.OtherCollider;
            float paddleCenterX = paddleCollider.bounds.center.x;
            float paddleHalfWidth = paddleCollider.bounds.extents.x;
            
            float hitOffset = collisionData.ContactPoint.x - paddleCenterX;
            float normalizedOffset = Mathf.Clamp(hitOffset / paddleHalfWidth, -1f, 1f);
            
            float bounceAngle = normalizedOffset * _maxBounceAngle;
            float angleRadians = bounceAngle * Mathf.Deg2Rad;
            
            Vector2 newDirection = new Vector2(Mathf.Sin(angleRadians), Mathf.Cos(angleRadians));
            
            if (newDirection.y < _minVerticalVelocity)
            {
                newDirection.y = _minVerticalVelocity;
                newDirection = newDirection.normalized;
            }
            
            return newDirection * speed;
        }
    }
}
