using UnityEngine;
using VContainer;

namespace ArkanoidCloneProject.Physics
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PhysicsPaddle : MonoBehaviour, IPhysicsBody
    {
        private BoxCollider2D _collider;
        private ArkanoidPhysicsWorld _physicsWorld;

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
        public PhysicsBodyType BodyType => PhysicsBodyType.Paddle;
        public Collider2D Collider => _collider;
        public bool IsActive => gameObject.activeInHierarchy;

        [Inject]
        public void Construct(ArkanoidPhysicsWorld physicsWorld)
        {
            _physicsWorld = physicsWorld;
        }

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            gameObject.tag = "Paddle";
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
        }
    }
}
