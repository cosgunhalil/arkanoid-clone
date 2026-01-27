using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ArkanoidCloneProject.Physics
{
    public class BallFactory : IBallFactory
    {
        private readonly IObjectResolver _resolver;
        private readonly GameObject _ballPrefab;

        [Inject]
        public BallFactory(IObjectResolver resolver, BallPrefabHolder ballPrefabHolder)
        {
            _resolver = resolver;
            _ballPrefab = ballPrefabHolder.Prefab;
        }

        public Ball.Ball Create(Vector2 position)
        {
            GameObject ballObject = Object.Instantiate(_ballPrefab, position, Quaternion.identity);
            Ball.Ball ball = ballObject.GetComponent<Ball.Ball>();
            _resolver.Inject(ball);
            ball.Initialiaze();
            return ball;
        }
    }
}