using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ArkanoidCloneProject.Physics
{
    public class ArkanoidPhysicsWorld : ITickable, IDisposable
    {
        private readonly PhysicsSettings _settings;
        private readonly List<IPhysicsBody> _bodies;
        private readonly Dictionary<PhysicsBodyType, ICollisionHandler> _collisionHandlers;
        private readonly List<IPhysicsBody> _bodiesToRemove;
        private readonly List<IPhysicsBody> _bodiesToAdd;
        private bool _isProcessing;

        public event Action<IPhysicsBody, CollisionData> OnCollisionDetected;
        public event Action<IPhysicsBody> OnBallLost;

        [Inject]
        public ArkanoidPhysicsWorld(PhysicsSettings settings)
        {
            _settings = settings;
            _bodies = new List<IPhysicsBody>();
            _bodiesToRemove = new List<IPhysicsBody>();
            _bodiesToAdd = new List<IPhysicsBody>();
            _collisionHandlers = new Dictionary<PhysicsBodyType, ICollisionHandler>();
            
            InitializeCollisionHandlers();
        }

        private void InitializeCollisionHandlers()
        {
            var standardHandler = new StandardCollisionHandler();
            var paddleHandler = new PaddleCollisionHandler(
                _settings.MaxPaddleBounceAngle,
                _settings.MinVerticalVelocity
            );
            
            _collisionHandlers[PhysicsBodyType.Wall] = standardHandler;
            _collisionHandlers[PhysicsBodyType.Brick] = standardHandler;
            _collisionHandlers[PhysicsBodyType.Paddle] = paddleHandler;
        }

        public void RegisterBody(IPhysicsBody body)
        {
            if (_isProcessing)
            {
                _bodiesToAdd.Add(body);
                return;
            }
            
            if (!_bodies.Contains(body))
            {
                _bodies.Add(body);
            }
        }

        public void UnregisterBody(IPhysicsBody body)
        {
            if (_isProcessing)
            {
                _bodiesToRemove.Add(body);
                return;
            }
            
            _bodies.Remove(body);
        }

        public void Tick()
        {
            _isProcessing = true;
            
            ProcessPendingChanges();
            
            int bodyCount = _bodies.Count;
            for (int i = 0; i < bodyCount; i++)
            {
                IPhysicsBody body = _bodies[i];
                
                if (!body.IsActive) continue;
                if (body.BodyType != PhysicsBodyType.Ball) continue;
                
                ProcessBallMovement(body);
            }
            
            _isProcessing = false;
            ProcessPendingChanges();
        }

        private void ProcessPendingChanges()
        {
            int removeCount = _bodiesToRemove.Count;
            for (int i = 0; i < removeCount; i++)
            {
                _bodies.Remove(_bodiesToRemove[i]);
            }
            _bodiesToRemove.Clear();
            
            int addCount = _bodiesToAdd.Count;
            for (int i = 0; i < addCount; i++)
            {
                if (!_bodies.Contains(_bodiesToAdd[i]))
                {
                    _bodies.Add(_bodiesToAdd[i]);
                }
            }
            _bodiesToAdd.Clear();
        }

        private void ProcessBallMovement(IPhysicsBody ball)
        {
            Vector2 velocity = ball.Velocity;
            Vector2 position = ball.Position;
            float radius = ball.Radius;
            float remainingDistance = velocity.magnitude * Time.fixedDeltaTime;
            
            int collisionCount = 0;
            
            while (remainingDistance > 0.0001f && collisionCount < _settings.MaxCollisionsPerFrame)
            {
                Vector2 direction = velocity.normalized;
                
                RaycastHit2D hit = Physics2D.CircleCast(
                    position,
                    radius,
                    direction,
                    remainingDistance,
                    _settings.CollisionMask
                );
                
                if (hit.collider == null)
                {
                    position += direction * remainingDistance;
                    break;
                }
                
                float moveDistance = hit.distance - _settings.ContactOffset;
                if (moveDistance > 0)
                {
                    position += direction * moveDistance;
                }
                
                CollisionData collisionData = CreateCollisionData(hit, ball);
                
                ball.OnCollision(collisionData);
                OnCollisionDetected?.Invoke(ball, collisionData);
                
                IPhysicsBody hitBody = hit.collider.GetComponent<IPhysicsBody>();
                if (hitBody != null)
                {
                    hitBody.OnCollision(new CollisionData(
                        hit.point,
                        -hit.normal,
                        ball,
                        ball.Collider,
                        ball.BodyType,
                        hit.distance
                    ));
                }
                
                if (collisionData.OtherBodyType == PhysicsBodyType.DeathZone)
                {
                    OnBallLost?.Invoke(ball);
                    return;
                }
                
                ICollisionHandler handler = GetCollisionHandler(collisionData.OtherBodyType);
                velocity = handler.CalculateReflection(velocity, collisionData);
                velocity = ClampVelocity(velocity);
                
                remainingDistance -= (moveDistance + _settings.ContactOffset);
                collisionCount++;
            }
            
            ball.Position = position;
            ball.Velocity = velocity;
        }

        private CollisionData CreateCollisionData(RaycastHit2D hit, IPhysicsBody ball)
        {
            IPhysicsBody otherBody = hit.collider.GetComponent<IPhysicsBody>();
            PhysicsBodyType bodyType = DetermineBodyType(hit.collider, otherBody);
            
            return new CollisionData(
                hit.centroid,
                hit.normal,
                otherBody,
                hit.collider,
                bodyType,
                hit.distance
            );
        }

        private PhysicsBodyType DetermineBodyType(Collider2D collider, IPhysicsBody body)
        {
            if (body != null)
            {
                return body.BodyType;
            }
            
            string tag = collider.tag;
            
            if (tag == "Wall") return PhysicsBodyType.Wall;
            if (tag == "Paddle") return PhysicsBodyType.Paddle;
            if (tag == "Brick") return PhysicsBodyType.Brick;
            if (tag == "DeathZone") return PhysicsBodyType.DeathZone;
            
            return PhysicsBodyType.Wall;
        }

        private ICollisionHandler GetCollisionHandler(PhysicsBodyType bodyType)
        {
            if (_collisionHandlers.TryGetValue(bodyType, out ICollisionHandler handler))
            {
                return handler;
            }
            return _collisionHandlers[PhysicsBodyType.Wall];
        }

        private Vector2 ClampVelocity(Vector2 velocity)
        {
            float speed = velocity.magnitude;
            
            if (speed < _settings.MinBallSpeed)
            {
                return velocity.normalized * _settings.MinBallSpeed;
            }
            
            if (speed > _settings.MaxBallSpeed)
            {
                return velocity.normalized * _settings.MaxBallSpeed;
            }
            
            return velocity;
        }

        public void Clear()
        {
            _bodies.Clear();
            _bodiesToAdd.Clear();
            _bodiesToRemove.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
