using UnityEngine;
using VContainer;

namespace ArkanoidCloneProject.Paddle
{
    public class PaddleFactory : IPaddleFactory
    {
        private readonly GameObject _paddlePrefab;
        private readonly IObjectResolver _resolver;

        [Inject]
        public PaddleFactory(IObjectResolver resolver, GameObject paddlePrefab)
        {
            _resolver = resolver;
            _paddlePrefab = paddlePrefab;
        }

        public Paddle Create()
        {
            var paddleObject = Object.Instantiate(_paddlePrefab);
            var paddle = paddleObject.GetComponent<Paddle>();
            _resolver.Inject(paddle);
            return paddle;
        }
    }
}