using ArkanoidCloneProject.InputSystem;
using ArkanoidCloneProject.Paddle;
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
        [Inject] private PaddlePlacer _paddlePlacer;
        
        private bool _isTransitioningToGame;

        protected override void OnEnter()
        {
            _isTransitioningToGame = false;
            _ballManager.PauseAllBalls();
            _paddlePlacer.PausePaddle();
            _pauseGameUI.Show();
            _inputManager.OnESCButtonUp += HandleContinue;
            Debug.Log("PauseGameState.OnEnter");
        }
        
        private void HandleContinue()
        {
            if (_isTransitioningToGame) return;
            _isTransitioningToGame = true;
            _ballManager.ResumeAllBalls();
            _paddlePlacer.ResumePaddle();
            _pauseGameUI.Hide();
            SendTrigger((int)StateTriggers.CONTINUE_GAME_REQUEST);
        }

        protected override void OnExit()
        {
            _inputManager.OnESCButtonUp -= HandleContinue;
            Debug.Log("PauseGameState.OnExit");
        }
    }
}