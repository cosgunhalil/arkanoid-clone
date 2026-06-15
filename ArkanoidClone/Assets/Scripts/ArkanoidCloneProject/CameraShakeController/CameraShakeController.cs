using System;
using ArkanoidCloneProject.Physics;
using ArkanoidCloneProject.LevelEditor;
using VContainer;

namespace ArkanoidCloneProject.Camera
{
    public class CameraShakeController : IDisposable
    {
        private const float BrickDestroyedIntensity = 0.05f;
        private const float BrickDestroyedDuration  = 0.12f;
        private const float BallLostIntensity        = 0.30f;
        private const float BallLostDuration         = 0.45f;

        private readonly CameraManager _cameraManager;
        private readonly BallManager _ballManager;
        private readonly BrickManager _brickManager;

        [Inject]
        public CameraShakeController(CameraManager cameraManager, BallManager ballManager, BrickManager brickManager)
        {
            _cameraManager = cameraManager;
            _ballManager   = ballManager;
            _brickManager  = brickManager;

            _ballManager.OnAllBallsLost      += HandleAllBallsLost;
            _brickManager.OnBrickDestroyed   += HandleBrickDestroyed;
        }

        private void HandleAllBallsLost() =>
            _cameraManager.Shake(BallLostIntensity, BallLostDuration);

        private void HandleBrickDestroyed(Brick _, int __) =>
            _cameraManager.Shake(BrickDestroyedIntensity, BrickDestroyedDuration);

        public void Dispose()
        {
            _ballManager.OnAllBallsLost    -= HandleAllBallsLost;
            _brickManager.OnBrickDestroyed -= HandleBrickDestroyed;
        }
    }
}
