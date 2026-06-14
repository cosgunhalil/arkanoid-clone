namespace ArkanoidCloneProject.PowerUp.Effects
{
    public class WidePaddleEffect : IPowerUpEffect
    {
        private const float ScaleFactor = 1.8f;

        public bool HasDuration => true;
        public float Duration => 10f;

        public void Apply(PowerUpContext context)
        {
            var paddle = context.Paddle;
            if (paddle == null) return;

            paddle.SetSizeMultiplier(ScaleFactor);
            context.PaddlePlacer.RecalculateBounds();
        }

        public void Revert(PowerUpContext context)
        {
            var paddle = context.Paddle;
            if (paddle == null) return;

            paddle.SetSizeMultiplier(1f);
            context.PaddlePlacer.RecalculateBounds();
        }
    }
}
