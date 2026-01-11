using System;
using UnityEngine;
using VContainer;

namespace ArkanoidCloneProject.Physics
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class Ball : MonoBehaviour, IPhysicsBody
    {
        [SerializeField] private float _radius = 0.25f;
        
        private CircleCollider2D _collider;
        private Vector2 _velocity;
        private bool _isLaunched;
        private ArkanoidPhysicsWorld _physicsWorld;

        public event Action<CollisionData> OnBallCollision;

        public Vector2 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Vector2 Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        public float Radius => _radius;
        public PhysicsBodyType BodyType => PhysicsBodyType.Ball;
        public Collider2D Collider => _collider;
        public bool IsActive => _isLaunched && gameObject.activeInHierarchy;
        public bool IsLaunched => _isLaunched;

        [Inject]
        public void Construct(ArkanoidPhysicsWorld physicsWorld)
        {
            _physicsWorld = physicsWorld;
        }

        private void Awake()
        {
            _collider = GetComponent<CircleCollider2D>();
            _collider.isTrigger = true;
            _collider.radius = _radius;
        }

        private void OnEnable()
        {
            _physicsWorld?.RegisterBody(this);
        }

        private void OnDisable()
        {
            _physicsWorld?.UnregisterBody(this);
        }

        public void Launch(Vector2 direction, float speed)
        {
            _velocity = direction.normalized * speed;
            _isLaunched = true;
        }

        public void Stop()
        {
            _velocity = Vector2.zero;
            _isLaunched = false;
        }

        public void OnCollision(CollisionData data)
        {
            OnBallCollision?.Invoke(data);
        }

        public void SetRadius(float radius)
        {
            _radius = radius;
            if (_collider != null)
            {
                _collider.radius = radius;
            }
        }
    }
}
