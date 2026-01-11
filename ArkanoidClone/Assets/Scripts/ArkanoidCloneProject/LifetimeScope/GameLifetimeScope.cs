using ArkanoidCloneProject.Controllers.Scripts;
using ArkanoidCloneProject.Factories.StateFactory;
using ArkanoidCloneProject.InputSystem;
using ArkanoidCloneProject.LevelEditor;
using ArkanoidCloneProject.Paddle;
using ArkanoidCloneProject.Physics;
using ArkanoidProject.State;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ArkanoidCloneProject.LifetimeScope
{
    public class GameLifetimeScope : VContainer.Unity.LifetimeScope
    {
        [SerializeField] private LevelCreator levelCreatorPrefab;
        [SerializeField] private CameraManager cameraManagerPrefab;
        [SerializeField] private BorderManager borderManagerPrefab;
        [SerializeField] private GameObject paddlePrefab;
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private PhysicsSettings physicsSettings;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<VContainerStateFactory>(Lifetime.Singleton).As<IStateFactory>();
            builder.Register<IInputManager, InputManager>(Lifetime.Singleton);
            builder.Register<AppState>(Lifetime.Transient);
            builder.Register<MainMenuState>(Lifetime.Transient);
            builder.Register<InGameState>(Lifetime.Transient);
            builder.Register<PauseGameState>(Lifetime.Transient);
            builder.Register<EndGameState>(Lifetime.Transient);
            builder.RegisterComponentInNewPrefab<LevelCreator>(levelCreatorPrefab, Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab<CameraManager>(cameraManagerPrefab, Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab<BorderManager>(borderManagerPrefab, Lifetime.Singleton);
            
            builder.RegisterInstance(paddlePrefab).As<GameObject>();
            builder.Register<PaddleFactory>(Lifetime.Singleton).As<IPaddleFactory>();
            builder.Register<PaddlePlacer>(Lifetime.Singleton);
            
            PhysicsInstaller.Install(builder, physicsSettings, ballPrefab);
            
            Debug.Log("GameLifetimeScope Configure");
        }

        protected override void Awake()
        {
            base.Awake();
            
            var go = new GameObject("AppController");
            var appController = go.AddComponent<AppController>();
            Container.Inject(appController);
        }
    }
}