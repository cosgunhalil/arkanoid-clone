using UnityEngine;
using VContainer;

namespace ArkanoidCloneProject.Physics
{
    public static class BallInstaller
    {
        public static void Install(IContainerBuilder builder, BallSettings settings, GameObject ballPrefab)
        {
            builder.RegisterInstance(settings);
            builder.RegisterInstance(new BallPrefabHolder(ballPrefab));
            builder.Register<BallFactory>(Lifetime.Singleton).As<IBallFactory>();
            builder.Register<BallManager>(Lifetime.Singleton);
        }
    }
}
