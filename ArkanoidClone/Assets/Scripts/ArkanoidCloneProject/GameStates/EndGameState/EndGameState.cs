using UnityEngine;

namespace ArkanoidProject.State 
{
    using Devkit.HSM;

    public class EndGameState : StateMachine
    {
        protected override void OnEnter()
        {
            Debug.Log("EndGameState.OnEnter");
        }

        protected override void OnUpdate()
        {
            Debug.Log("EndGameState.OnUpdate");
        }

        protected override void OnExit()
        {
            Debug.Log("EndGameState.OnExit");
        }
    }
}

