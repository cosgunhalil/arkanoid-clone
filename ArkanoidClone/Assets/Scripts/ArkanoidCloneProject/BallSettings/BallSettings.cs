using UnityEngine;

namespace ArkanoidCloneProject.Physics
{
    [CreateAssetMenu(fileName = "BallSettings", menuName = "Arkanoid/Ball Settings")]
    public class BallSettings : ScriptableObject
    {
        [Header("Ball Settings")]
        public float BallSpeed = 8f;
        public float MaxPaddleBounceAngle = 75f;
        public float SkinWidth = 0.01f;
        public float PaddleOffsetY = 0.5f;
    }
}
