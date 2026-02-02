using ArkanoidCloneProject.LevelEditor;
using ArkanoidCloneProject.Paddle;
using ArkanoidCloneProject.Physics;
using ArkanoidProject.State;
using Devkit.HSM;
using UnityEngine;
using VContainer;

namespace ArkanoidCloneProject.GameStates.PrepareGameState
{
    public class PrepareGameState : StateMachine
    {
        [Inject] private LevelCreator _levelCreator;
        [Inject] private CameraManager _cameraManager;
        [Inject] private BorderManager _borderManager;
        [Inject] private PaddlePlacer _paddlePlacer;
        [Inject] private BrickManager _brickManager;
        [Inject] private BallManager _ballManager;

        protected override async void OnEnter()
        {
            Debug.Log("InGameState.OnEnter");
            _levelCreator.OnLevelCreated += HandleLevelCreated;
            _levelCreator.SetBrickManager(_brickManager);
            await _levelCreator.LoadFirstLevelAsync();
            SendTrigger((int)StateTriggers.PREPARE_COMPLETE);
        }

        protected override void OnExit()
        {
            _levelCreator.OnLevelCreated -= HandleLevelCreated;
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
            _paddlePlacer.Place();
            
            _brickManager.UnregisterAllBricks();
            _brickManager.RegisterBricksFromGameObjects(_levelCreator.GetSpawnedBricks());
            
            SpawnBall();
        }
        
        private void SpawnBall()
        {
            Paddle.Paddle paddle = _paddlePlacer.GetCurrentPaddle();
            if (paddle != null)
            {
                _ballManager.SpawnBallAbovePaddle(paddle);
            }
        }
        
        private float CalculateWorldHeight()
        {
            var camera = Camera.main;
            return camera.orthographicSize * 2f;
        }

        private float CalculateWorldWidth()
        {
            var camera = Camera.main;
            float screenAspect = (float)Screen.width / Screen.height;
            return camera.orthographicSize * 2f * screenAspect;
        }
    }
}
