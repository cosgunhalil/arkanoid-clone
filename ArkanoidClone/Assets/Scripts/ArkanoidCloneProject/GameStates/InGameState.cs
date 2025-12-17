using UnityEngine;

namespace ArkanoidProject.State 
{
    using Devkit.HSM;

    public class InGameState : StateMachine
    {
        protected override void OnEnter()
        {
            Debug.Log("InGameState.OnEnter");
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

