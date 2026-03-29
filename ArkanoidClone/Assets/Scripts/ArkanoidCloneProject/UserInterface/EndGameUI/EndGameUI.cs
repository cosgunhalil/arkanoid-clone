using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArkanoidProject.State
{
    [RequireComponent(typeof(Canvas))]
    public class EndGameUI : MonoBehaviour
    {
        [SerializeField] private Button _retryButton;
        [SerializeField] private TextMeshProUGUI _levelText;

        private Canvas _canvas;

        public event Action OnRetryClicked;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;
            _retryButton.onClick.AddListener(HandleRetryClicked);
        }

        public void Show(int levelIndex)
        {
            if (_levelText != null)
                _levelText.text = $"Game Over\nLevel {levelIndex + 1}";

            _canvas.enabled = true;
        }

        public void Hide()
        {
            _canvas.enabled = false;
        }

        private void HandleRetryClicked()
        {
            OnRetryClicked?.Invoke();
        }

        private void OnDestroy()
        {
            _retryButton.onClick.RemoveListener(HandleRetryClicked);
        }
    }
}