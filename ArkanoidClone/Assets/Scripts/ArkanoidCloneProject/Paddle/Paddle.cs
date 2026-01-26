using ArkanoidCloneProject.InputSystem;
using UnityEngine;

namespace ArkanoidCloneProject.Paddle
{
    public class Paddle : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 10f;
        
        private IInputManager _inputManager;
        private bool _isMovingLeft;
        private bool _isMovingRight;
        private float _minX;
        private float _maxX;

        public void Initialize(IInputManager inputManager, float minX, float maxX)
        {
            _inputManager = inputManager;
            _minX = minX;
            _maxX = maxX;
            
            _inputManager.OnLeftButtonDown += HandleLeftButtonDown;
            _inputManager.OnLeftButtonUp += HandleLeftButtonUp;
            _inputManager.OnRightButtonDown += HandleRightButtonDown;
            _inputManager.OnRightButtonUp += HandleRightButtonUp;
        }

        public void SetBounds(float minX, float maxX)
        {
            _minX = minX;
            _maxX = maxX;
        }

        private void HandleLeftButtonDown()
        {
            _isMovingLeft = true;
        }

        private void HandleLeftButtonUp()
        {
            _isMovingLeft = false;
        }

        private void HandleRightButtonDown()
        {
            _isMovingRight = true;
        }

        private void HandleRightButtonUp()
        {
            _isMovingRight = false;
        }

        private void Update()
        {
            if (_inputManager == null) return;

            float direction = 0f;
            
            if (_isMovingLeft)
            {
                direction = -1f;
            }
            else if (_isMovingRight)
            {
                direction = 1f;
            }

            if (direction != 0f)
            {
                Vector3 position = transform.position;
                position.x += direction * _moveSpeed * Time.deltaTime;
                position.x = Mathf.Clamp(position.x, _minX, _maxX);
                transform.position = position;
            }
        }

        private void OnDestroy()
        {
            if (_inputManager == null) return;
            
            _inputManager.OnLeftButtonDown -= HandleLeftButtonDown;
            _inputManager.OnLeftButtonUp -= HandleLeftButtonUp;
            _inputManager.OnRightButtonDown -= HandleRightButtonDown;
            _inputManager.OnRightButtonUp -= HandleRightButtonUp;
        }
    }
}