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

        private LevelBounds _currentBounds;
        private LevelBounds _boundsWithMargins;

        public float LeftMargin
        {
            get => _leftMargin;
            set => _leftMargin = value;
        }

        public float RightMargin
        {
            get => _rightMargin;
            set => _rightMargin = value;
        }

        public float TopMargin
        {
            get => _topMargin;
            set => _topMargin = value;
        }

        public float BottomMargin
        {
            get => _bottomMargin;
            set => _bottomMargin = value;
        }

        public LevelBounds CurrentBounds => _currentBounds;
        public LevelBounds BoundsWithMargins => _boundsWithMargins;

        public Vector2 PlayableAreaTopLeft => _boundsWithMargins.TopLeft;
        public Vector2 PlayableAreaTopRight => _boundsWithMargins.TopRight;
        public Vector2 PlayableAreaBottomLeft => _boundsWithMargins.BottomLeft;
        public Vector2 PlayableAreaBottomRight => _boundsWithMargins.BottomRight;
        public Vector2 PlayableAreaCenter => _boundsWithMargins.Center;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
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
            Vector2 topLeft = new Vector2(
                _currentBounds.TopLeft.x - _leftMargin,
                _currentBounds.TopLeft.y + _topMargin
            );

            Vector2 topRight = new Vector2(
                _currentBounds.TopRight.x + _rightMargin,
                _currentBounds.TopRight.y + _topMargin
            );

            Vector2 bottomLeft = new Vector2(
                _currentBounds.BottomLeft.x - _leftMargin,
                _currentBounds.BottomLeft.y - _bottomMargin
            );

            Vector2 bottomRight = new Vector2(
                _currentBounds.BottomRight.x + _rightMargin,
                _currentBounds.BottomRight.y - _bottomMargin
            );

            _boundsWithMargins = new LevelBounds(topLeft, topRight, bottomLeft, bottomRight);
        }

        private void AdjustCameraToFitBounds()
        {
            if (_camera == null) return;

            Vector3 cameraPosition = _camera.transform.position;
            cameraPosition.x = _boundsWithMargins.Center.x;
            cameraPosition.y = _boundsWithMargins.Center.y;
            _camera.transform.position = cameraPosition;

            if (_camera.orthographic)
            {
                float screenAspect = (float)Screen.width / Screen.height;
                float boundsAspect = _boundsWithMargins.Width / _boundsWithMargins.Height;

                if (boundsAspect > screenAspect)
                {
                    _camera.orthographicSize = _boundsWithMargins.Width / (2f * screenAspect);
                }
                else
                {
                    _camera.orthographicSize = _boundsWithMargins.Height / 2f;
                }
            }
        }

        public Vector2 GetWorldPositionFromViewport(Vector2 viewportPosition)
        {
            float x = Mathf.Lerp(_boundsWithMargins.BottomLeft.x, _boundsWithMargins.BottomRight.x, viewportPosition.x);
            float y = Mathf.Lerp(_boundsWithMargins.BottomLeft.y, _boundsWithMargins.TopLeft.y, viewportPosition.y);
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
            float clampedX = Mathf.Clamp(position.x, _boundsWithMargins.BottomLeft.x, _boundsWithMargins.BottomRight.x);
            float clampedY = Mathf.Clamp(position.y, _boundsWithMargins.BottomLeft.y, _boundsWithMargins.TopLeft.y);
            return new Vector2(clampedX, clampedY);
        }
    }
}
