using System;
using VContainer;

namespace ArkanoidCloneProject.Player
{
    public class PlayerHealth
    {
        private int _currentHealth;
        private int _maxHealth;

        public event Action OnHealthChanged;
        public event Action OnHealthDepleted;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;

        public PlayerHealth(int maxHealth = 3)
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
        }

        public void TakeDamage(int amount)
        {
            _currentHealth -= amount;
            if (_currentHealth < 0) _currentHealth = 0;
            OnHealthChanged?.Invoke();

            if (_currentHealth == 0)
            {
                OnHealthDepleted?.Invoke();
            }
        }

        public void Reset()
        {
            _currentHealth = _maxHealth;
            OnHealthChanged?.Invoke();
        }
    }
}