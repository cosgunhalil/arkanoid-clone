using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ArkanoidCloneProject.Physics
{
    public class BallPrefabHolder
    {
        public GameObject Prefab { get; }
        
        public BallPrefabHolder(GameObject prefab)
        {
            Prefab = prefab;
        }
    }
    
    public static class PhysicsInstaller
    {
        public static void Install(IContainerBuilder builder, PhysicsSettings settings, GameObject ballPrefab)
        {
            builder.RegisterInstance(settings);
            builder.Register<ArkanoidPhysicsWorld>(Lifetime.Singleton).AsSelf().As<ITickable>();
            builder.RegisterInstance(new BallPrefabHolder(ballPrefab));
            builder.Register<BallFactory>(Lifetime.Singleton).As<IBallFactory>();
            builder.Register<BallManager>(Lifetime.Singleton);
            builder.Register<BrickManager>(Lifetime.Singleton);
        }
    }
}