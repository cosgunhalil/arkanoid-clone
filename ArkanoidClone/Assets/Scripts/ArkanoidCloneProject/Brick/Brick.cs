using System;
using UnityEngine;

namespace ArkanoidCloneProject.Physics
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Brick : MonoBehaviour
    {
        [SerializeField] private int _scoreValue = 10;

        private BoxCollider2D _collider;
        private int _currentHealth;
        private int _health;
        private Color _originalColor;

        public event Action<Brick> OnBrickDestroyed;
        public event Action<Brick, int> OnBrickDamaged;

        public int Health => _currentHealth;
        public int ScoreValue => _scoreValue;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            gameObject.tag = "Brick";
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                TakeDamage(1);
            }
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            OnBrickDamaged?.Invoke(this, _currentHealth);

            UpdateColor();

            if (_currentHealth <= 0)
            {
                DestroyBrick();
            }
        }

        private void UpdateColor()
        {
            var renderer = GetComponent<SpriteRenderer>();
            if (renderer == null) return;

            float healthRatio = _health > 0 ? (float)_currentHealth / _health : 0f;
            renderer.color = Color.Lerp(Color.black, _originalColor, healthRatio);
        }

        private void DestroyBrick()
        {
            OnBrickDestroyed?.Invoke(this);
            gameObject.SetActive(false);
        }

        public void SetHealth(int health)
        {
            _health = health;
            _currentHealth = health;
        }

        public void SetScoreValue(int value)
        {
            _scoreValue = value;
        }

        public void SetColor(Color color)
        {
            _originalColor = color;
            var renderer = GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.color = color;
        }

        public void SetRandomPowerUp()
        {
            //TODO: implement!
        }
    }
}