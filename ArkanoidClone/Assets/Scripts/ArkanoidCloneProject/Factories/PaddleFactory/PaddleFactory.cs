using UnityEngine;
using VContainer;

namespace ArkanoidCloneProject.Paddle
{
    public class PaddleFactory : IPaddleFactory
    {
        private readonly IObjectResolver _resolver;
        private readonly GameObject _paddlePrefab;

        [Inject]
        public PaddleFactory(IObjectResolver resolver, GameObject paddlePrefab)
        {
            _resolver = resolver;
            _paddlePrefab = paddlePrefab;
        }

        public Paddle Create()
        {
            GameObject paddleObject = Object.Instantiate(_paddlePrefab);
            Paddle paddle = paddleObject.GetComponent<Paddle>();
            _resolver.Inject(paddle);
            return paddle;
        }
    }
}
