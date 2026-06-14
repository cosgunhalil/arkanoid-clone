namespace ArkanoidCloneProject.PowerUp.Effects
{
    public class BallSpeedEffect : IPowerUpEffect
    {
        private const float SpeedMultiplier = 1.5f;

        public bool HasDuration => true;
        public float Duration => 8f;

        public void Apply(PowerUpContext context)
        {
            foreach (var ball in context.BallManager.GetActiveBalls())
                ball.SetSpeed(context.BaseBallSpeed * SpeedMultiplier);
        }

        public void Revert(PowerUpContext context)
        {
            foreach (var ball in context.BallManager.GetActiveBalls())
                ball.SetSpeed(context.BaseBallSpeed);
        }
    }
}
