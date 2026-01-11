using System;
using System.Collections.Generic;
using ArkanoidCloneProject.LevelEditor;
using UnityEngine;
using VContainer;

namespace ArkanoidCloneProject.Physics
{
    public class BallManager
    {
        private readonly IBallFactory _ballFactory;
        private readonly ArkanoidPhysicsWorld _physicsWorld;
        private readonly PhysicsSettings _settings;
        private readonly CameraManager _cameraManager;
        private readonly List<Ball> _activeBalls;

        public event Action OnAllBallsLost;
        public event Action<Ball> OnBallSpawned;
        
        public int ActiveBallCount => _activeBalls.Count;

        [Inject]
        public BallManager(
            IBallFactory ballFactory, 
            ArkanoidPhysicsWorld physicsWorld, 
            PhysicsSettings settings,
            CameraManager cameraManager)
        {
            _ballFactory = ballFactory;
            _physicsWorld = physicsWorld;
            _settings = settings;
            _cameraManager = cameraManager;
            _activeBalls = new List<Ball>();
            
            _physicsWorld.OnBallLost += HandleBallLost;
        }

        public Ball SpawnBall(Vector2 position)
        {
            Ball ball = _ballFactory.Create(position);
            _activeBalls.Add(ball);
            OnBallSpawned?.Invoke(ball);
            return ball;
        }

        public Ball SpawnBallAbovePaddle(Transform paddleTransform, float offsetY = 0.5f)
        {
            Vector2 spawnPosition = (Vector2)paddleTransform.position + Vector2.up * offsetY;
            return SpawnBall(spawnPosition);
        }

        public void LaunchBall(Ball ball, Vector2 direction)
        {
            ball.Launch(direction, _settings.BallSpeed);
        }

        public void LaunchBallUpward(Ball ball)
        {
            float randomAngle = UnityEngine.Random.Range(-30f, 30f);
            float angleRadians = randomAngle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Sin(angleRadians), Mathf.Cos(angleRadians));
            LaunchBall(ball, direction);
        }

        private void HandleBallLost(IPhysicsBody body)
        {
            Ball ball = body as Ball;
            if (ball == null) return;
            
            _activeBalls.Remove(ball);
            UnityEngine.Object.Destroy(ball.gameObject);
            
            if (_activeBalls.Count == 0)
            {
                OnAllBallsLost?.Invoke();
            }
        }

        public void RemoveAllBalls()
        {
            int count = _activeBalls.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                Ball ball = _activeBalls[i];
                if (ball != null && ball.gameObject != null)
                {
                    UnityEngine.Object.Destroy(ball.gameObject);
                }
            }
            _activeBalls.Clear();
        }

        public void StopAllBalls()
        {
            int count = _activeBalls.Count;
            for (int i = 0; i < count; i++)
            {
                _activeBalls[i].Stop();
            }
        }

        public List<Ball> GetActiveBalls()
        {
            return new List<Ball>(_activeBalls);
        }
    }
}
