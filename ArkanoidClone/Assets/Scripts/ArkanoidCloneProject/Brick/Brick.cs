using System;
using UnityEngine;

namespace ArkanoidCloneProject.Physics
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Brick : MonoBehaviour
    {
        [SerializeField] private int _health = 1;
        [SerializeField] private int _scoreValue = 10;
        
        private BoxCollider2D _collider;
        private int _currentHealth;

        public event Action<Brick> OnBrickDestroyed;
        public event Action<Brick, int> OnBrickDamaged;

        public int Health => _currentHealth;
        public int ScoreValue => _scoreValue;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            _currentHealth = _health;
            gameObject.tag = "Brick";
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<Ball>() == null) return;
            
            TakeDamage(1);
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            OnBrickDamaged?.Invoke(this, _currentHealth);
            
            if (_currentHealth <= 0)
            {
                DestroyBrick();
            }
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
    }
}