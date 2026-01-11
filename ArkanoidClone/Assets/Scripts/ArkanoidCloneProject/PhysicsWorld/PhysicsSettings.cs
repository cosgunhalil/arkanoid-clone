using UnityEngine;

namespace ArkanoidCloneProject.Physics
{
    [CreateAssetMenu(fileName = "PhysicsSettings", menuName = "Arkanoid/Physics Settings")]
    public class PhysicsSettings : ScriptableObject
    {
        [Header("Ball Settings")]
        public float BallSpeed = 8f;
        public float MaxBallSpeed = 15f;
        public float MinBallSpeed = 5f;
        
        [Header("Collision Settings")]
        public float ContactOffset = 0.01f;
        public int MaxCollisionsPerFrame = 5;
        
        [Header("Paddle Settings")]
        public float MaxPaddleBounceAngle = 75f;
        public float MinVerticalVelocity = 0.3f;
        
        [Header("Layer Masks")]
        public LayerMask CollisionMask;
    }
}
