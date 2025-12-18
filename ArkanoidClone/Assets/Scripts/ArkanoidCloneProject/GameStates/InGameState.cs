using ArkanoidCloneProject.LevelEditor;
using UnityEngine;
using VContainer;

namespace ArkanoidProject.State 
{
    using Devkit.HSM;

    public class InGameState : StateMachine
    {
        [Inject] private LevelCreator _levelCreator;
        protected override async void OnEnter()
        {
            Debug.Log("InGameState.OnEnter");
            await _levelCreator.LoadAndCreateLevelAsync("Level1");
        }

        protected override void OnExit()
        {
            Debug.Log("InGameState.OnExit");
        }

        protected override void OnUpdate()
        {
            Debug.Log("InGameState.OnUpdate");
        }
    }
}

