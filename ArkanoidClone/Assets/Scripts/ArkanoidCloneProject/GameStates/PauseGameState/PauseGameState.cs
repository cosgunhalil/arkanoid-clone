using ArkanoidCloneProject.InputSystem;
using ArkanoidCloneProject.Physics;
using Devkit.HSM;
using UnityEngine;
using VContainer;

namespace ArkanoidProject.State
{
    public class PauseGameState : StateMachine
    {
        [Inject] private BallManager _ballManager;
        [Inject] private IInputManager _inputManager;
        [Inject] private PauseGameUI _pauseGameUI;

        protected override void OnEnter()
        {
            _ballManager.PauseAllBalls();
            _pauseGameUI.Show();
            _inputManager.OnESCButtonUp += HandleContinue;
            Debug.Log("PauseGameState.OnEnter");
        }
        
        private void HandleContinue()
        {
            _ballManager.ResumeAllBalls();
            SendTrigger((int)StateTriggers.CONTINUE_GAME_REQUEST);
        }

        protected override void OnExit()
        {
            _pauseGameUI.Hide();
            _inputManager.OnESCButtonUp -= HandleContinue;
            Debug.Log("PauseGameState.OnExit");
        }
    }
}