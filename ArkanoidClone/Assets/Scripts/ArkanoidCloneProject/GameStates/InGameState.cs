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

        private Ball _currentBall;
        private bool _isTransitioningLevel;

        protected override async void OnEnter()
        {
            Debug.Log("InGameState.OnEnter");

            _isTransitioningLevel = false;
            
            _levelCreator.OnLevelCreated += HandleLevelCreated;
            _levelCreator.OnAllLevelsCompleted += HandleAllLevelsCompleted;
            _levelCreator.OnLevelStarted += HandleLevelStarted;
            _ballManager.OnAllBallsLost += HandleAllBallsLost;
            _brickManager.OnAllBricksDestroyed += HandleAllBricksDestroyed;
            _brickManager.OnScoreChanged += HandleScoreChanged;
            
            _levelCreator.SetBrickManager(_brickManager);
            
            await _levelCreator.LoadFirstLevelAsync();
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
            
            if (_paddlePlacer.GetCurrentPaddle() == null)
            {
                _paddlePlacer.Place();
            }
            
            _brickManager.UnregisterAllBricks();
            _brickManager.RegisterBricksFromGameObjects(_levelCreator.GetSpawnedBricks());
            
            SpawnBall();
            
            _isTransitioningLevel = false;
        }

        private void HandleLevelStarted(int levelIndex)
        {
            Debug.Log($"Starting Level {levelIndex + 1}");
            
            _ballManager.RemoveAllBalls();
            
            _isTransitioningLevel = true;
        }

        private void SpawnBall()
        {
            Paddle paddle = _paddlePlacer.GetCurrentPaddle();
            if (paddle != null)
            {
                _currentBall = _ballManager.SpawnBallAbovePaddle(paddle);
            }
        }

        private void HandleAllBallsLost()
        {
            if (_isTransitioningLevel)
            {
                return;
            }
            
            Debug.Log("All balls lost!");
            SendTrigger((int)StateTriggers.GAME_OVER_REQUEST);
        }

        private void HandleAllBricksDestroyed()
        {
            Debug.Log($"Level {_levelCreator.CurrentLevelIndex + 1} Complete!");
        }

        private void HandleAllLevelsCompleted()
        {
            Debug.Log("All levels completed! You win!");
            SendTrigger((int)StateTriggers.GAME_OVER_REQUEST);
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
            _levelCreator.OnAllLevelsCompleted -= HandleAllLevelsCompleted;
            _levelCreator.OnLevelStarted -= HandleLevelStarted;
            _ballManager.OnAllBallsLost -= HandleAllBallsLost;
            _brickManager.OnAllBricksDestroyed -= HandleAllBricksDestroyed;
            _brickManager.OnScoreChanged -= HandleScoreChanged;
            
            _ballManager.RemoveAllBalls();
            _brickManager.UnregisterAllBricks();
            _paddlePlacer.RemovePaddle();
            _levelCreator.ClearLevel();
        }
    }
}