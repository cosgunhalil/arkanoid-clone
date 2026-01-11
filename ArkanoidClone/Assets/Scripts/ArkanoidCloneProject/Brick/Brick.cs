using System;
using UnityEngine;
using VContainer;

namespace ArkanoidCloneProject.Physics
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Brick : MonoBehaviour, IPhysicsBody
    {
        [SerializeField] private int _health = 1;
        [SerializeField] private int _scoreValue = 10;
        
        private BoxCollider2D _collider;
        private ArkanoidPhysicsWorld _physicsWorld;
        private int _currentHealth;

        public event Action<Brick> OnBrickDestroyed;
        public event Action<Brick, int> OnBrickDamaged;

        public Vector2 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Vector2 Velocity
        {
            get => Vector2.zero;
            set { }
        }

        public float Radius => 0f;
        public PhysicsBodyType BodyType => PhysicsBodyType.Brick;
        public Collider2D Collider => _collider;
        public bool IsActive => gameObject.activeInHierarchy;
        public int Health => _currentHealth;
        public int ScoreValue => _scoreValue;

        [Inject]
        public void Construct(ArkanoidPhysicsWorld physicsWorld)
        {
            _physicsWorld = physicsWorld;
        }

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            _currentHealth = _health;
        }

        private void OnEnable()
        {
            _physicsWorld?.RegisterBody(this);
        }

        private void OnDisable()
        {
            _physicsWorld?.UnregisterBody(this);
        }

        public void OnCollision(CollisionData data)
        {
            if (data.OtherBodyType != PhysicsBodyType.Ball) return;
            
            TakeDamage(1);
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            OnBrickDamaged?.Invoke(this, _currentHealth);
            
            if (_currentHealth <= 0)
            {
                Destroy();
            }
        }

        private void Destroy()
        {
            OnBrickDestroyed?.Invoke(this);
            _physicsWorld?.UnregisterBody(this);
            gameObject.SetActive(false);
        }

        public void SetHealth(int health)
        {
            _health = health;
            _currentHealth = health;
        }

        public void SetScoreValue(int value)
        {
            _scoreValue = value;
        }
    }
}
