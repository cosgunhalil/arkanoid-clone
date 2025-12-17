using ArkanoidCloneProject.Controllers.Scripts;
using ArkanoidCloneProject.Factories.StateFactory;
using ArkanoidProject.State;
using UnityEngine;
using VContainer;

public class GameLifetimeScope : VContainer.Unity.LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<VContainerStateFactory>(Lifetime.Singleton).As<IStateFactory>();

        builder.Register<AppState>(Lifetime.Transient);
        builder.Register<MainMenuState>(Lifetime.Transient);
        builder.Register<InGameState>(Lifetime.Transient);
        builder.Register<PauseGameState>(Lifetime.Transient);
        builder.Register<EndGameState>(Lifetime.Transient);
            
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