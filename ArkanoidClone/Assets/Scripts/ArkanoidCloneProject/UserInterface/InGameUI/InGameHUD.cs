using TMPro;
using UnityEngine;

namespace ArkanoidCloneProject.UserInterface
{
    [RequireComponent(typeof(Canvas))]
    public class InGameHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _healthText;

        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.enabled = false;
        }

        public void Show()
        {
            _canvas.enabled = true;
        }

        public void Hide()
        {
            _canvas.enabled = false;
        }

        public void SetScore(int score)
        {
            if (_scoreText != null)
                _scoreText.text = $"SCORE\n{score:D6}";
        }

        public void SetLevel(int levelIndex)
        {
            if (_levelText != null)
                _levelText.text = $"LEVEL\n{levelIndex + 1:D2}";
        }

        public void SetHealth(int currentHealth, int maxHealth)
        {
            if (_healthText != null)
                _healthText.text = $"LIVES\n{currentHealth}/{maxHealth}";
        }
    }
}