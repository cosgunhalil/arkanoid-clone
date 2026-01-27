using UnityEngine;
using VContainer;

namespace ArkanoidCloneProject.LevelEditor
{
    public class BorderManager : MonoBehaviour
    {
        [SerializeField] private float _wallThickness = 1f;
        [SerializeField] private PhysicsMaterial2D _wallPhysicsMaterial;
        [SerializeField] private Camera _camera;
        
        private CameraManager _cameraManager;
        private GameObject _topWall;
        private GameObject _bottomWall;
        private GameObject _leftWall;
        private GameObject _rightWall;
        private Transform _wallContainer;

        [Inject]
        public void Construct(CameraManager cameraManager)
        {
            _cameraManager = cameraManager;
        }

        public GameObject TopWall => _topWall;
        public GameObject BottomWall => _bottomWall;
        public GameObject LeftWall => _leftWall;
        public GameObject RightWall => _rightWall;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }

        public void CreateBorders()
        {
            ClearBorders();
            CreateWallContainer();
            
            CameraBounds cameraBounds = GetCameraBounds();
            
            _topWall = CreateWall("Wall", GetTopWallPosition(cameraBounds), GetHorizontalWallSize(cameraBounds));
            _bottomWall = CreateWall("Wall", GetBottomWallPosition(cameraBounds), GetHorizontalWallSize(cameraBounds));
            _leftWall = CreateWall("Wall", GetLeftWallPosition(cameraBounds), GetVerticalWallSize(cameraBounds));
            _rightWall = CreateWall("Wall", GetRightWallPosition(cameraBounds), GetVerticalWallSize(cameraBounds));
        }

        public void ClearBorders()
        {
            if (_wallContainer != null)
            {
                Destroy(_wallContainer.gameObject);
                _wallContainer = null;
            }
            
            _topWall = null;
            _bottomWall = null;
            _leftWall = null;
            _rightWall = null;
        }

        public void UpdateBorders()
        {
            if (_cameraManager == null) return;
            CreateBorders();
        }

        private CameraBounds GetCameraBounds()
        {
            float cameraHeight = _camera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * _camera.aspect;
            
            Vector3 cameraPosition = _camera.transform.position;
            
            float left = cameraPosition.x - cameraWidth / 2f;
            float right = cameraPosition.x + cameraWidth / 2f;
            float top = cameraPosition.y + cameraHeight / 2f;
            float bottom = cameraPosition.y - cameraHeight / 2f;
            
            return new CameraBounds(left, right, top, bottom, cameraPosition.x, cameraPosition.y);
        }

        private void CreateWallContainer()
        {
            GameObject containerObject = new GameObject("BorderWalls");
            _wallContainer = containerObject.transform;
            _wallContainer.SetParent(transform);
        }

        private GameObject CreateWall(string wallName, Vector3 position, Vector2 size)
        {
            GameObject wall = new GameObject(wallName);
            wall.transform.SetParent(_wallContainer);
            wall.transform.position = position;
            wall.tag = "Wall";
            
            var colliderComponent = wall.AddComponent<BoxCollider2D>();
            colliderComponent.size = size;
            
            if (_wallPhysicsMaterial != null)
            {
                colliderComponent.sharedMaterial = _wallPhysicsMaterial;
            }
            
            return wall;
        }

        private Vector3 GetTopWallPosition(CameraBounds cameraBounds)
        {
            float x = cameraBounds.CenterX;
            float y = cameraBounds.Top + _wallThickness / 2f;
            return new Vector3(x, y, 0f);
        }

        private Vector3 GetBottomWallPosition(CameraBounds cameraBounds)
        {
            float x = cameraBounds.CenterX;
            float y = cameraBounds.Bottom - _wallThickness / 2f;
            return new Vector3(x, y, 0f);
        }

        private Vector3 GetLeftWallPosition(CameraBounds cameraBounds)
        {
            float x = cameraBounds.Left - _wallThickness / 2f;
            float y = cameraBounds.CenterY;
            return new Vector3(x, y, 0f);
        }

        private Vector3 GetRightWallPosition(CameraBounds cameraBounds)
        {
            float x = cameraBounds.Right + _wallThickness / 2f;
            float y = cameraBounds.CenterY;
            return new Vector3(x, y, 0f);
        }

        private Vector2 GetHorizontalWallSize(CameraBounds cameraBounds)
        {
            float width = (cameraBounds.Right - cameraBounds.Left) + _wallThickness * 2f;
            return new Vector2(width, _wallThickness);
        }

        private Vector2 GetVerticalWallSize(CameraBounds cameraBounds)
        {
            float height = (cameraBounds.Top - cameraBounds.Bottom) + _wallThickness * 2f;
            return new Vector2(_wallThickness, height);
        }

        private void OnDestroy()
        {
            ClearBorders();
        }
    }

    public struct CameraBounds
    {
        public float Left;
        public float Right;
        public float Top;
        public float Bottom;
        public float CenterX;
        public float CenterY;

        public CameraBounds(float left, float right, float top, float bottom, float centerX, float centerY)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
            CenterX = centerX;
            CenterY = centerY;
        }
    }
}