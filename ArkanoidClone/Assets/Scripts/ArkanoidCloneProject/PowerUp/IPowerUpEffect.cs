namespace ArkanoidCloneProject.PowerUp
{
    public interface IPowerUpEffect
    {
        bool HasDuration { get; }
        float Duration { get; }
        void Apply(PowerUpContext context);
        void Revert(PowerUpContext context);
    }
}
