using UnityEngine;
using VContainer;

namespace ArkanoidCloneProject.LevelEditor
{
    public class BorderManager : MonoBehaviour
    {
        [SerializeField] private float _wallThickness = 1f;
        [SerializeField] private PhysicsMaterial2D _wallPhysicsMaterial;
        
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

        public void CreateBorders()
        {
            ClearBorders();
            CreateWallContainer();
            
            var cameraBounds = _cameraManager.GetCameraBounds();
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
        
        private void CreateWallContainer()
        {
            GameObject containerObject = new GameObject("BorderWalls");
            _wallContainer = containerObject.transform;
            _wallContainer.SetParent(transform);
        }

        private GameObject CreateWall(string wallName, Vector3 position, Vector2 size)
        {
            var wall = new GameObject(wallName);
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
            var x = cameraBounds.CenterX;
            var y = cameraBounds.Top + _wallThickness / 2f;
            return new Vector3(x, y, 0f);
        }

        private Vector3 GetBottomWallPosition(CameraBounds cameraBounds)
        {
            var x = cameraBounds.CenterX;
            var y = cameraBounds.Bottom - _wallThickness / 2f;
            return new Vector3(x, y, 0f);
        }

        private Vector3 GetLeftWallPosition(CameraBounds cameraBounds)
        {
            var x = cameraBounds.Left - _wallThickness / 2f;
            var y = cameraBounds.CenterY;
            return new Vector3(x, y, 0f);
        }

        private Vector3 GetRightWallPosition(CameraBounds cameraBounds)
        {
            var x = cameraBounds.Right + _wallThickness / 2f;
            var y = cameraBounds.CenterY;
            return new Vector3(x, y, 0f);
        }

        private Vector2 GetHorizontalWallSize(CameraBounds cameraBounds)
        {
            var width = (cameraBounds.Right - cameraBounds.Left) + _wallThickness * 2f;
            return new Vector2(width, _wallThickness);
        }

        private Vector2 GetVerticalWallSize(CameraBounds cameraBounds)
        {
            var height = (cameraBounds.Top - cameraBounds.Bottom) + _wallThickness * 2f;
            return new Vector2(_wallThickness, height);
        }

        private void OnDestroy()
        {
            ClearBorders();
        }
    }
}