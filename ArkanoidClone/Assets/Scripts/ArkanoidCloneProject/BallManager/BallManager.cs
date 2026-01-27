using System;
using System.Collections.Generic;
using ArkanoidCloneProject.LevelEditor;
using ArkanoidCloneProject.Paddle;
using UnityEngine;
using VContainer;

namespace ArkanoidCloneProject.Physics
{
    public class BallManager
    {
        private readonly IBallFactory _ballFactory;
        private readonly BallSettings _settings;
        private readonly CameraManager _cameraManager;
        private readonly List<Ball.Ball> _activeBalls;

        public event Action OnAllBallsLost;
        public event Action<Ball.Ball> OnBallSpawned;
        
        public int ActiveBallCount => _activeBalls.Count;

        [Inject]
        public BallManager(
            IBallFactory ballFactory, 
            BallSettings settings,
            CameraManager cameraManager)
        {
            _ballFactory = ballFactory;
            _settings = settings;
            _cameraManager = cameraManager;
            _activeBalls = new List<Ball.Ball>();
        }

        public Ball.Ball SpawnBall(Vector2 position, Paddle.Paddle paddle)
        {
            Ball.Ball ball = _ballFactory.Create(position);
            ball.SetPaddle(paddle);
            ball.SetSpeed(_settings.BallSpeed);
            ball.OnBallDeath += () => HandleBallDeath(ball);
            _activeBalls.Add(ball);
            OnBallSpawned?.Invoke(ball);
            return ball;
        }

        public Ball.Ball SpawnBallAbovePaddle(Paddle.Paddle paddle, float offsetY = 0.5f)
        {
            Vector2 spawnPosition = (Vector2)paddle.transform.position + Vector2.up * offsetY;
            return SpawnBall(spawnPosition, paddle);
        }

        public void LaunchBall(Ball.Ball ball)
        {
            ball.Launch();
        }

        public void LaunchBall(Ball.Ball ball, Vector2 direction)
        {
            ball.Launch(direction);
        }

        private void HandleBallDeath(Ball.Ball ball)
        {
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
                Ball.Ball ball = _activeBalls[i];
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

        public List<Ball.Ball> GetActiveBalls()
        {
            return new List<Ball.Ball>(_activeBalls);
        }
    }
}