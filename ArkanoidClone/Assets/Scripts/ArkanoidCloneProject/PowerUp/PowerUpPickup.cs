using System;
using UnityEngine;

namespace ArkanoidCloneProject.PowerUp
{
    [RequireComponent(typeof(Collider2D))]
    public class PowerUpPickup : MonoBehaviour
    {
        private const float FallSpeed = 4f;

        public event Action<PowerUpPickup> OnPickedUp;

        public PowerUpType PowerUpType { get; private set; }

        public void Initialize(PowerUpType type)
        {
            PowerUpType = type;
        }

        private void Update()
        {
            transform.position += Vector3.down * FallSpeed * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Paddle"))
            {
                OnPickedUp?.Invoke(this);
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Hazard"))
            {
                gameObject.SetActive(false);
            }
        }
    }
}
