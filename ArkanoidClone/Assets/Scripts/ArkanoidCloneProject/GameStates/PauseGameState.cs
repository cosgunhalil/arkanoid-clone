using UnityEngine;
using UnityEngine.PlayerLoop;

namespace ArkanoidProject.State 
{
    using Devkit.HSM;

    public class PauseGameState : StateMachine
    {
        protected override void OnEnter()
        {
            Debug.Log("PauseGameState.OnEnter");
        }

        protected override void OnUpdate()
        {
            Debug.Log("PauseGameState.OnUpdate");
        }

        protected override void OnExit()
        {
            Debug.Log("PauseGameState.OnExit");
        }
    }
}

