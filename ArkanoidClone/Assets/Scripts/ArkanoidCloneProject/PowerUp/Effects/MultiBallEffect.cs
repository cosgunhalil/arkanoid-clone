using UnityEngine;

namespace ArkanoidCloneProject.PowerUp.Effects
{
    public class MultiBallEffect : IPowerUpEffect
    {
        private const int ExtraBalls = 2;

        public bool HasDuration => false;
        public float Duration => 0f;

        public void Apply(PowerUpContext context)
        {
            var balls = context.BallManager.GetActiveBalls();
            if (balls.Count == 0) return;

            var refBall = balls[0];
            var refPosition = (Vector2)refBall.transform.position;

            for (int i = 0; i < ExtraBalls; i++)
            {
                var angle = Random.Range(-60f, 60f) * Mathf.Deg2Rad;
                var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                context.BallManager.SpawnAndLaunchBall(refPosition, direction);
            }
        }

        public void Revert(PowerUpContext context) { }
    }
}
