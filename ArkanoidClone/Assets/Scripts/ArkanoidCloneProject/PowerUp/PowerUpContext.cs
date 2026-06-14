using ArkanoidCloneProject.Paddle;
using ArkanoidCloneProject.Physics;

namespace ArkanoidCloneProject.PowerUp
{
    public class PowerUpContext
    {
        public PaddlePlacer PaddlePlacer { get; }
        public BallManager BallManager { get; }
        public float BaseBallSpeed { get; }

        public Paddle.Paddle Paddle => PaddlePlacer.GetCurrentPaddle();

        public PowerUpContext(PaddlePlacer paddlePlacer, BallManager ballManager, float baseBallSpeed)
        {
            PaddlePlacer = paddlePlacer;
            BallManager = ballManager;
            BaseBallSpeed = baseBallSpeed;
        }
    }
}
