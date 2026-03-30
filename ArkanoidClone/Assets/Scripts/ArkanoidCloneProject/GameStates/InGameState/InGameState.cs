using ArkanoidCloneProject.LevelEditor;
using ArkanoidCloneProject.Paddle;
using ArkanoidCloneProject.Physics;
using ArkanoidCloneProject.Player;
using ArkanoidCloneProject.InputSystem;
using ArkanoidCloneProject.UserInterface;
using Cysharp.Threading.Tasks;
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
        [Inject] private PlayerHealth _playerHealth;
        [Inject] private InGameHUDPresenter _hudPresenter;

        private bool _isTransitioningToPause;
        private bool _isTransitioningLevel;
        private int _retryLevelIndex;

        protected override void OnEnter()
        {
            _isTransitioningToPause = false;
            _isTransitioningLevel = false;

            _levelCreator.OnLevelCreated += HandleLevelCreated;
            _levelCreator.OnAllLevelsCompleted += HandleAllLevelsCompleted;
            _levelCreator.OnLevelStarted += HandleLevelStarted;
            _ballManager.OnAllBallsLost += HandleAllBallsLost;
            _brickManager.OnAllBricksDestroyed += HandleAllBricksDestroyed;
            _inputSystem.OnESCButtonUp += HandlePauseRequest;

            _hudPresenter.Enable();

            _retryLevelIndex = _levelCreator.CurrentLevelIndex;

            LoadRetryLevel();
        }

        private void LoadRetryLevel()
        {
            _levelCreator.LoadLevelAtIndexAsync(_retryLevelIndex).Forget();
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
            _borderManager.CreateBorders();

            _paddlePlacer.Reposition();

            _brickManager.UnregisterAllBricks();
            _brickManager.RegisterBricksFromGameObjects(_levelCreator.GetSpawnedBricks());

            SpawnBall();

            _isTransitioningLevel = false;
        }

        private void HandleLevelStarted(int levelIndex)
        {
            _ballManager.RemoveAllBalls();
            _isTransitioningLevel = true;
        }

        private void SpawnBall()
        {
            Paddle paddle = _paddlePlacer.GetCurrentPaddle();
            if (paddle != null)
                _ballManager.SpawnBallAbovePaddle(paddle);
        }

        private void HandleAllBallsLost()
        {
            if (_isTransitioningLevel) return;

            _playerHealth.TakeDamage(1);

            if (_playerHealth.CurrentHealth <= 0)
            {
                SendTrigger((int)StateTriggers.GAME_OVER_REQUEST);
                return;
            }

            SpawnBall();
        }

        private void HandleAllBricksDestroyed()
        {
            _retryLevelIndex = _levelCreator.CurrentLevelIndex;
        }

        private void HandleAllLevelsCompleted()
        {
            SendTrigger((int)StateTriggers.GAME_OVER_REQUEST);
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

        private void HandlePauseRequest()
        {
            if (_isTransitioningToPause) return;
            _isTransitioningToPause = true;
            SendTrigger((int)StateTriggers.PAUSE_GAME_REQUEST);
        }

        protected override void OnExit()
        {
            _levelCreator.OnLevelCreated -= HandleLevelCreated;
            _levelCreator.OnAllLevelsCompleted -= HandleAllLevelsCompleted;
            _levelCreator.OnLevelStarted -= HandleLevelStarted;
            _ballManager.OnAllBallsLost -= HandleAllBallsLost;
            _brickManager.OnAllBricksDestroyed -= HandleAllBricksDestroyed;
            _inputSystem.OnESCButtonUp -= HandlePauseRequest;

            _hudPresenter.Disable();

            _ballManager.RemoveAllBalls();
            _brickManager.UnregisterAllBricks();
            _paddlePlacer.RemovePaddle();
            _levelCreator.ClearLevel();
        }
    }
}