using ArkanoidCloneProject.LevelEditor;
using UnityEngine;
using VContainer;

namespace ArkanoidProject.State
{
    using Devkit.HSM;

    public class InGameState : StateMachine
    {
        [Inject] private LevelCreator _levelCreator;
        [Inject] private CameraManager _cameraManager;

        protected override async void OnEnter()
        {
            Debug.Log("InGameState.OnEnter");

            _levelCreator.OnLevelCreated += HandleLevelCreated;
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
        }

        protected override void OnUpdate()
        {
            Debug.Log("InGameState.OnUpdate");
        }
    }
}