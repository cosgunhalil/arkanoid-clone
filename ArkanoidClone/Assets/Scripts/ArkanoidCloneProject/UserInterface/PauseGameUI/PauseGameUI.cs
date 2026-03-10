using System;
using UnityEngine;

namespace ArkanoidProject.State
{
    [RequireComponent(typeof(Canvas))]
    public class PauseGameUI : MonoBehaviour
    {
        private Canvas _canvas;

        private void Start()
        {
            _canvas = GetComponent<Canvas>();
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
    }
}