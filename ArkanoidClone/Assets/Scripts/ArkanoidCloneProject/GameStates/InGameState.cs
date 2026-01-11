using ArkanoidCloneProject.InputSystem;
using ArkanoidCloneProject.LevelEditor;
using ArkanoidCloneProject.Paddle;
using ArkanoidCloneProject.Physics;
using UnityEngine;
using VContainer;

namespace ArkanoidProject.State
{
    using Devkit.HSM;

    public class InGameState : StateMachine
    {
        [Inject] private LevelCreator _levelCreator;
        [Inject] private CameraManager _cameraManager;
        [Inject] private BorderManager _borderManager;
        [Inject] private IInputManager _inputSystem;
        [Inject] private PaddlePlacer _paddlePlacer;
        [Inject] private BallManager _ballManager;
        [Inject] private BrickManager _brickManager;
        [Inject] private ArkanoidPhysicsWorld _physicsWorld;

        private Ball _currentBall;
        private bool _ballLaunched;

        protected override async void OnEnter()
        {
            Debug.Log("InGameState.OnEnter");

            _levelCreator.OnLevelCreated += HandleLevelCreated;
            _ballManager.OnAllBallsLost += HandleAllBallsLost;
            _brickManager.OnAllBricksDestroyed += HandleAllBricksDestroyed;
            _brickManager.OnScoreChanged += HandleScoreChanged;
            _inputSystem.OnSpaceButtonDown += HandleSpacePressed;
            
            await _levelCreator.LoadAndCreateLevelAsync("Level1");
        }

        private void HandleLevelCreated(LevelBounds levelBounds)
        {
            float worldHeight = CalculateWorldHeight();
            float worldWidth = CalculateWorldWidth();

            float topMargin = worldHeight * 0.05f;
            float bottomMargin = worldHeight * 0.35f;
            float leftMargin = worldWidth * 0.02f;
            float rightMargin = worldWidth * 0.02f;

            _cameraManager.SetMargins(leftMargin, rightMargin, topMargin, bottomMargin);
            _cameraManager.FocusOnLevel(levelBounds);

            Debug.Log($"Level Bounds - TopLeft: {levelBounds.TopLeft}, TopRight: {levelBounds.TopRight}");
            Debug.Log($"Level Bounds - BottomLeft: {levelBounds.BottomLeft}, BottomRight: {levelBounds.BottomRight}");
            Debug.Log($"Level Bounds - Center: {levelBounds.Center}");
            Debug.Log($"Playable Area Center: {_cameraManager.PlayableAreaCenter}");
            
            _borderManager.CreateBorders();
            _paddlePlacer.Place();
            
            _brickManager.RegisterBricksFromGameObjects(_levelCreator.GetSpawnedTiles());
            
            SpawnBall();
        }

        private void SpawnBall()
        {
            Paddle paddle = _paddlePlacer.GetCurrentPaddle();
            if (paddle != null)
            {
                _currentBall = _ballManager.SpawnBallAbovePaddle(paddle.transform);
                _ballLaunched = false;
            }
        }

        private void HandleSpacePressed()
        {
            if (_currentBall != null && !_ballLaunched)
            {
                _ballManager.LaunchBallUpward(_currentBall);
                _ballLaunched = true;
            }
        }

        private void HandleAllBallsLost()
        {
            Debug.Log("All balls lost!");
            SendTrigger((int)StateTriggers.GAME_OVER_REQUEST);
        }

        private void HandleAllBricksDestroyed()
        {
            Debug.Log("Level Complete!");
        }

        private void HandleScoreChanged(int newScore)
        {
            Debug.Log($"Score: {newScore}");
        }

        private float CalculateWorldHeight()
        {
            Camera camera = Camera.main;
            return camera.orthographicSize * 2f;
        }

        private float CalculateWorldWidth()
        {
            Camera camera = Camera.main;
            float screenAspect = (float)Screen.width / Screen.height;
            return camera.orthographicSize * 2f * screenAspect;
        }

        protected override void OnExit()
        {
            Debug.Log("InGameState.OnExit");

            _levelCreator.OnLevelCreated -= HandleLevelCreated;
            _ballManager.OnAllBallsLost -= HandleAllBallsLost;
            _brickManager.OnAllBricksDestroyed -= HandleAllBricksDestroyed;
            _brickManager.OnScoreChanged -= HandleScoreChanged;
            _inputSystem.OnSpaceButtonDown -= HandleSpacePressed;
            
            _ballManager.RemoveAllBalls();
            _brickManager.UnregisterAllBricks();
            _paddlePlacer.RemovePaddle();
        }

        protected override void OnUpdate()
        {
            if (_currentBall != null && !_ballLaunched)
            {
                Paddle paddle = _paddlePlacer.GetCurrentPaddle();
                if (paddle != null)
                {
                    Vector2 paddlePos = paddle.transform.position;
                    _currentBall.Position = paddlePos + Vector2.up * 0.5f;
                }
            }
            else
            {
                _physicsWorld.Tick();
            }
        }
    }
}