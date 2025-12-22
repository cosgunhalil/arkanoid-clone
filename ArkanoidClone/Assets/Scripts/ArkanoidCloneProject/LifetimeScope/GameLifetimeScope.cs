using ArkanoidCloneProject.Controllers.Scripts;
using ArkanoidCloneProject.Factories.StateFactory;
using ArkanoidCloneProject.LevelEditor;
using ArkanoidProject.State;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : VContainer.Unity.LifetimeScope
{
    [SerializeField] private LevelCreator levelCreatorPrefab;
    [SerializeField] private CameraManager cameraManagerPrefab;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<VContainerStateFactory>(Lifetime.Singleton).As<IStateFactory>();

        builder.Register<AppState>(Lifetime.Transient);
        builder.Register<MainMenuState>(Lifetime.Transient);
        builder.Register<InGameState>(Lifetime.Transient);
        builder.Register<PauseGameState>(Lifetime.Transient);
        builder.Register<EndGameState>(Lifetime.Transient);
        builder.RegisterComponentInNewPrefab<LevelCreator>(levelCreatorPrefab, Lifetime.Singleton);
        builder.RegisterComponentInNewPrefab<CameraManager>(cameraManagerPrefab, Lifetime.Singleton);
            
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