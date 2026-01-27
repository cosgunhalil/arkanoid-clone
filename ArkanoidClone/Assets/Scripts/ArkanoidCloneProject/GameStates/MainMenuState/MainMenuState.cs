using UnityEngine;

namespace ArkanoidProject.State 
{
    using Devkit.HSM;

    public class MainMenuState : StateMachine
    {
        protected override void OnEnter()
        {
            Debug.Log("MainMenuState.OnEnter");
            SendTrigger((int)StateTriggers.START_GAME_REQUEST);
        }

        protected override void OnUpdate()
        {
            Debug.Log("MainMenuState.OnUpdate");
        }

        protected override void OnExit()
        {
            Debug.Log("MainMenuState.OnExit");
        }
    }
}

