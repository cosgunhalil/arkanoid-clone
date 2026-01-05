using ArkanoidCloneProject.InputSystem;
using ArkanoidCloneProject.LevelEditor;
using UnityEngine;
using VContainer;

namespace ArkanoidCloneProject.Paddle
{
    public class PaddlePlacer
    {
        private readonly IPaddleFactory _paddleFactory;
        private readonly CameraManager _cameraManager;
        private readonly IInputManager _inputManager;
        private readonly Camera _camera;
        
        private Paddle _currentPaddle;

        [Inject]
        public PaddlePlacer(IPaddleFactory paddleFactory, CameraManager cameraManager, IInputManager inputManager)
        {
            _paddleFactory = paddleFactory;
            _cameraManager = cameraManager;
            _inputManager = inputManager;
            _camera = Camera.main;
        }

        public Paddle Place()
        {
            _currentPaddle = _paddleFactory.Create();
            
            LevelBounds bounds = _cameraManager.BoundsWithMargins;
            
            float paddleY = bounds.BottomLeft.y + 0.5f;
            float centerX = bounds.Center.x;
            
            _currentPaddle.transform.position = new Vector3(centerX, paddleY, 0f);
            
            float paddleHalfWidth = _currentPaddle.transform.localScale.x / 2f;
            
            float cameraHeight = _camera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * _camera.aspect;
            float screenLeftEdge = _camera.transform.position.x - cameraWidth / 2f;
            float screenRightEdge = _camera.transform.position.x + cameraWidth / 2f;
            
            float minX = screenLeftEdge + paddleHalfWidth;
            float maxX = screenRightEdge - paddleHalfWidth;
            
            _currentPaddle.Initialize(_inputManager, minX, maxX);
            
            return _currentPaddle;
        }

        public Paddle GetCurrentPaddle()
        {
            return _currentPaddle;
        }

        public void RemovePaddle()
        {
            if (_currentPaddle != null)
            {
                Object.Destroy(_currentPaddle.gameObject);
                _currentPaddle = null;
            }
        }
    }
}