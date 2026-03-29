using ArkanoidCloneProject.LevelEditor;
using ArkanoidCloneProject.Physics;
using ArkanoidCloneProject.Player;
using VContainer;

namespace ArkanoidProject.State
{
    using Devkit.HSM;

    public class EndGameState : StateMachine
    {
        [Inject] private EndGameUI _endGameUI;
        [Inject] private LevelCreator _levelCreator;
        [Inject] private BallManager _ballManager;
        [Inject] private PlayerHealth _playerHealth;

        protected override void OnEnter()
        {
            _ballManager.RemoveAllBalls();
            _playerHealth.Reset();
            _endGameUI.Show(_levelCreator.CurrentLevelIndex);
            _endGameUI.OnRetryClicked += HandleRetry;
        }

        private void HandleRetry()
        {
            _endGameUI.OnRetryClicked -= HandleRetry;
            _endGameUI.Hide();
            SendTrigger((int)StateTriggers.RETRY_GAME_REQUEST);
        }

        protected override void OnExit()
        {
            _endGameUI.OnRetryClicked -= HandleRetry;
        }
    }
}