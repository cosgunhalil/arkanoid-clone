using UnityEngine;
using VContainer;

namespace ArkanoidCloneProject.Physics
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DeathZone : MonoBehaviour, IPhysicsBody
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
        public PhysicsBodyType BodyType => PhysicsBodyType.DeathZone;
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
            _collider.isTrigger = true;
            gameObject.tag = "DeathZone";
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
