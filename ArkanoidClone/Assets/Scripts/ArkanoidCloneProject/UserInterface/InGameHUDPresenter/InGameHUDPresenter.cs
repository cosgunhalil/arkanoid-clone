using ArkanoidCloneProject.LevelEditor;
using ArkanoidCloneProject.Physics;
using ArkanoidCloneProject.Player;
using VContainer;

namespace ArkanoidCloneProject.UserInterface
{
    public class InGameHUDPresenter
    {
        private readonly InGameHUD _hud;
        private readonly PlayerHealth _playerHealth;
        private readonly BrickManager _brickManager;
        private readonly LevelCreator _levelCreator;

        [Inject]
        public InGameHUDPresenter(
            InGameHUD hud,
            PlayerHealth playerHealth,
            BrickManager brickManager,
            LevelCreator levelCreator)
        {
            _hud = hud;
            _playerHealth = playerHealth;
            _brickManager = brickManager;
            _levelCreator = levelCreator;
        }

        public void Enable()
        {
            _playerHealth.OnHealthChanged += HandleHealthChanged;
            _brickManager.OnScoreChanged += HandleScoreChanged;
            _levelCreator.OnLevelCreated += HandleLevelCreated;

            _hud.SetHealth(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
            _hud.SetScore(_brickManager.TotalScore);
            _hud.SetLevel(_levelCreator.CurrentLevelIndex);
            _hud.Show();
        }

        public void Disable()
        {
            _playerHealth.OnHealthChanged -= HandleHealthChanged;
            _brickManager.OnScoreChanged -= HandleScoreChanged;
            _levelCreator.OnLevelCreated -= HandleLevelCreated;

            _hud.Hide();
        }

        private void HandleHealthChanged()
        {
            _hud.SetHealth(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
        }

        private void HandleScoreChanged(int newScore)
        {
            _hud.SetScore(newScore);
        }

        private void HandleLevelCreated(LevelBounds _)
        {
            _hud.SetLevel(_levelCreator.CurrentLevelIndex);
        }
    }
}
