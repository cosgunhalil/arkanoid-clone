using System;
using ArkanoidCloneProject.InputSystem;
using UnityEngine;
using VContainer;

namespace ArkanoidCloneProject.Ball
{
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Ball : MonoBehaviour
    {
        [SerializeField] private float _speed = 8f;
        [SerializeField] private float _maxPaddleBounceAngle = 75f;
        [SerializeField] private float _skinWidth = 0.01f;
        [SerializeField] private float _paddleOffsetY = 0.5f;

        private CircleCollider2D _collider;
        private Rigidbody2D _rigidbody;
        private IInputManager _inputManager;
        private Paddle.Paddle _paddle;
        private Vector2 _velocity;
        private BallState _state;

        public event Action OnBallDeath;

        public BallState State => _state;
        public float Speed => _speed;

        [Inject]
        public void Construct(IInputManager inputManager)
        {
            _inputManager = inputManager;
        }

        public void SetPaddle(Paddle.Paddle paddle)
        {
            _paddle = paddle;
        }

        private void Awake()
        {
            _collider = GetComponent<CircleCollider2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
            
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _collider.isTrigger = false;
            
            _state = BallState.Hold;
            _velocity = Vector2.zero;
        }

        public void Initialiaze()
        {
            _inputManager.OnSpaceButtonDown += HandleSpacePressed;
        }

        public void OnDestroy()
        {
            _inputManager.OnSpaceButtonDown -= HandleSpacePressed;
        }

        private void HandleSpacePressed()
        {
            if (_state == BallState.Hold)
            {
                Launch();
            }
        }

        public void Launch()
        {
            _state = BallState.Free;
            
            var randomAngle = UnityEngine.Random.Range(-30f, 30f);
            var angleRadians = randomAngle * Mathf.Deg2Rad;
            var direction = new Vector2(Mathf.Sin(angleRadians), Mathf.Cos(angleRadians));
            
            _velocity = direction.normalized * _speed;
        }

        public void Launch(Vector2 direction)
        {
            _state = BallState.Free;
            _velocity = direction.normalized * _speed;
        }

        public void Stop()
        {
            _state = BallState.Hold;
            _velocity = Vector2.zero;
        }

        public void SetSpeed(float speed)
        {
            _speed = speed;
            if (_state == BallState.Free && _velocity.magnitude > 0)
            {
                _velocity = _velocity.normalized * _speed;
            }
        }

        private void FixedUpdate()
        {
            if (_state == BallState.Hold)
            {
                UpdateHoldState();
            }
            else
            {
                UpdateFreeState();
            }
        }

        private void UpdateHoldState()
        {
            if (_paddle == null) return;
            
            Vector3 paddlePosition = _paddle.transform.position;
            Vector3 newPosition = new Vector3(paddlePosition.x, paddlePosition.y + _paddleOffsetY, 0f);
            _rigidbody.MovePosition(newPosition);
        }

        private void UpdateFreeState()
        {
            var movement = _velocity * Time.fixedDeltaTime;
            var newPosition = _rigidbody.position + movement;
            _rigidbody.MovePosition(newPosition);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_state != BallState.Free) return;
            
            string tag = collision.gameObject.tag;
            
            if (tag == "DeathZone")
            {
                HandleDeathZoneCollision();
                return;
            }
            
            if (tag == "Paddle")
            {
                HandlePaddleCollision(collision);
            }
            else
            {
                HandleStandardCollision(collision);
            }
            
            ApplySkinWidthCorrection(collision);
        }

        private void HandleDeathZoneCollision()
        {
            _velocity = Vector2.zero;
            _state = BallState.Hold;
            OnBallDeath?.Invoke();
        }

        private void HandleStandardCollision(Collision2D collision)
        {
            if (collision.contactCount == 0) return;
            
            Vector2 normal = collision.contacts[0].normal;
            _velocity = Vector2.Reflect(_velocity, normal);
            _velocity = _velocity.normalized * _speed;
        }

        private void HandlePaddleCollision(Collision2D collision)
        {
            if (collision.contactCount == 0) return;
            
            var paddleCollider = collision.collider;
            var paddleCenterX = paddleCollider.bounds.center.x;
            var paddleHalfWidth = paddleCollider.bounds.extents.x;
            
            var contactPoint = collision.contacts[0].point;
            var hitOffset = contactPoint.x - paddleCenterX;
            var normalizedOffset = Mathf.Clamp(hitOffset / paddleHalfWidth, -1f, 1f);
            
            var bounceAngle = normalizedOffset * _maxPaddleBounceAngle;
            var angleRadians = bounceAngle * Mathf.Deg2Rad;
            
            var newDirection = new Vector2(Mathf.Sin(angleRadians), Mathf.Cos(angleRadians));
            _velocity = newDirection.normalized * _speed;
        }

        private void ApplySkinWidthCorrection(Collision2D collision)
        {
            if (collision.contactCount == 0) return;
            
            var normal = collision.contacts[0].normal;
            var correctedPosition = _rigidbody.position + normal * _skinWidth;
            transform.position = correctedPosition;
        }
    }
}