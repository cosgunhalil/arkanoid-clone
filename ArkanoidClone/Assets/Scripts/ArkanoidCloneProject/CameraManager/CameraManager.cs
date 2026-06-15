using System;
using UnityEngine;

namespace ArkanoidCloneProject.LevelEditor
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _leftMargin = 1f;
        [SerializeField] private float _rightMargin = 1f;
        [SerializeField] private float _topMargin = 1f;
        [SerializeField] private float _bottomMargin = 1f;

        [Header("Screen Shake")]
        [SerializeField] private float _defaultShakeIntensity = 0.15f;
        [SerializeField] private float _defaultShakeDuration = 0.25f;

        private LevelBounds _boundsWithMargins;
        private LevelBounds _currentBounds;

        private Vector3 _basePosition;
        private float _shakeIntensity;
        private float _shakeDuration;
        private float _shakeTimeRemaining;

        public LevelBounds BoundsWithMargins => _boundsWithMargins;
        public Vector2 PlayableAreaCenter => _boundsWithMargins.Center;

        private void Awake()
        {
            if (_camera == null) _camera = Camera.main;
        }

        public void FocusOnLevel(LevelBounds levelBounds)
        {
            _currentBounds = levelBounds;
            CalculateBoundsWithMargins();
            AdjustCameraToFitBounds();
        }

        public void SetMargins(float left, float right, float top, float bottom)
        {
            _leftMargin = left;
            _rightMargin = right;
            _topMargin = top;
            _bottomMargin = bottom;

            CalculateBoundsWithMargins();
            AdjustCameraToFitBounds();
        }

        private void CalculateBoundsWithMargins()
        {
            var topLeft = new Vector2(
                _currentBounds.TopLeft.x - _leftMargin,
                _currentBounds.TopLeft.y + _topMargin
            );

            var topRight = new Vector2(
                _currentBounds.TopRight.x + _rightMargin,
                _currentBounds.TopRight.y + _topMargin
            );

            var bottomLeft = new Vector2(
                _currentBounds.BottomLeft.x - _leftMargin,
                _currentBounds.BottomLeft.y - _bottomMargin
            );

            var bottomRight = new Vector2(
                _currentBounds.BottomRight.x + _rightMargin,
                _currentBounds.BottomRight.y - _bottomMargin
            );

            _boundsWithMargins = new LevelBounds(topLeft, topRight, bottomLeft, bottomRight);
        }

        private void Update()
        {
            if (_shakeTimeRemaining > 0f)
            {
                _shakeTimeRemaining -= Time.deltaTime;
                _camera.transform.position = _basePosition + ShakeOffset();
            }
        }

        public void Shake(float intensity, float duration)
        {
            _shakeIntensity = intensity;
            _shakeDuration = duration;
            _shakeTimeRemaining = duration;
        }

        public void Shake() => Shake(_defaultShakeIntensity, _defaultShakeDuration);

        private Vector3 ShakeOffset()
        {
            if (_shakeTimeRemaining <= 0f) return Vector3.zero;
            float t = _shakeTimeRemaining / _shakeDuration;
            return (Vector3)(UnityEngine.Random.insideUnitCircle * (_shakeIntensity * t));
        }

        private void AdjustCameraToFitBounds()
        {
            if (_camera == null) return;

            _basePosition = new Vector3(
                _boundsWithMargins.Center.x,
                _boundsWithMargins.Center.y,
                _camera.transform.position.z
            );
            _camera.transform.position = _basePosition + ShakeOffset();

            if (_camera.orthographic)
            {
                var screenAspect = (float)Screen.width / Screen.height;
                var boundsAspect = _boundsWithMargins.Width / _boundsWithMargins.Height;

                if (boundsAspect > screenAspect)
                    _camera.orthographicSize = _boundsWithMargins.Width / (2f * screenAspect);
                else
                    _camera.orthographicSize = _boundsWithMargins.Height / 2f;
            }
        }

        public Vector2 GetWorldPositionFromViewport(Vector2 viewportPosition)
        {
            var x = Mathf.Lerp(_boundsWithMargins.BottomLeft.x, _boundsWithMargins.BottomRight.x, viewportPosition.x);
            var y = Mathf.Lerp(_boundsWithMargins.BottomLeft.y, _boundsWithMargins.TopLeft.y, viewportPosition.y);
            return new Vector2(x, y);
        }

        public bool IsPositionInPlayableArea(Vector2 position)
        {
            return position.x >= _boundsWithMargins.BottomLeft.x &&
                   position.x <= _boundsWithMargins.BottomRight.x &&
                   position.y >= _boundsWithMargins.BottomLeft.y &&
                   position.y <= _boundsWithMargins.TopLeft.y;
        }

        public Vector2 ClampPositionToPlayableArea(Vector2 position)
        {
            var clampedX = Mathf.Clamp(position.x, _boundsWithMargins.BottomLeft.x, _boundsWithMargins.BottomRight.x);
            var clampedY = Mathf.Clamp(position.y, _boundsWithMargins.BottomLeft.y, _boundsWithMargins.TopLeft.y);
            return new Vector2(clampedX, clampedY);
        }

        public Tuple<float, float> GetCameraMinMaxX(float margin)
        {
            var cameraHeight = _camera.orthographicSize * 2f;
            var cameraWidth = cameraHeight * _camera.aspect;
            var screenLeftEdge = _camera.transform.position.x - cameraWidth * .5f;
            var screenRightEdge = _camera.transform.position.x + cameraWidth * .5f;
            var minX = screenLeftEdge + margin;
            var maxX = screenRightEdge - margin;

            return new Tuple<float, float>(minX, maxX);
        }

        public Vector2 GetCameraBottomCoordinate()
        {
            var cameraHeight = _camera.orthographicSize * 2f;
            var cameraBottom = _camera.transform.position.y - cameraHeight * .5f;
            var cameraCenterX = _camera.transform.position.x;

            return new Vector2(cameraCenterX, cameraBottom);
        }

        public CameraBounds GetCameraBounds()
        {
            var cameraHeight = _camera.orthographicSize * 2f;
            var cameraWidth = cameraHeight * _camera.aspect;

            var cameraPosition = _camera.transform.position;

            var left = cameraPosition.x - cameraWidth / 2f;
            var right = cameraPosition.x + cameraWidth / 2f;
            var top = cameraPosition.y + cameraHeight / 2f;
            var bottom = cameraPosition.y - cameraHeight / 2f;

            return new CameraBounds(left, right, top, bottom, cameraPosition.x, cameraPosition.y);
        }
    }
}