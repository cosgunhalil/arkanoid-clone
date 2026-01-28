using ArkanoidCloneProject.InputSystem;
using ArkanoidCloneProject.LevelEditor;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace ArkanoidCloneProject.Paddle
{
    public class PaddlePlacer
    {
        private readonly IPaddleFactory _paddleFactory;
        private readonly CameraManager _cameraManager;
        private readonly IInputManager _inputManager;
        
        private Paddle _currentPaddle;

        [Inject]
        public PaddlePlacer(IPaddleFactory paddleFactory, CameraManager cameraManager, IInputManager inputManager)
        {
            _paddleFactory = paddleFactory;
            _cameraManager = cameraManager;
            _inputManager = inputManager;
        }

        public Paddle Place()
        {
            _currentPaddle = _paddleFactory.Create();
            Reposition();
            var paddleHalfWidth = _currentPaddle.transform.localScale.x * .5f;
            var cameraMinMaxX = _cameraManager.GetCameraMinMaxX(paddleHalfWidth);
            _currentPaddle.Initialize(_inputManager, cameraMinMaxX.Item1, cameraMinMaxX.Item2);
            
            return _currentPaddle;
        }

        public void RecalculateBounds()
        {
            if (_currentPaddle == null) return;

            var paddleHalfWidth = _currentPaddle.transform.localScale.x * .5f;
            var cameraMinMaxX = _cameraManager.GetCameraMinMaxX(paddleHalfWidth);
            _currentPaddle.SetBounds(cameraMinMaxX.Item1, cameraMinMaxX.Item2);
        }

        public void Reposition()
        {
            if (_currentPaddle == null) return;
            
            var cameraBottomCoordinate = _cameraManager.GetCameraBottomCoordinate();
            var paddleY = cameraBottomCoordinate.y + 0.5f;
            _currentPaddle.transform.position = new Vector3(cameraBottomCoordinate.x, paddleY, 0f);
            
            RecalculateBounds();
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