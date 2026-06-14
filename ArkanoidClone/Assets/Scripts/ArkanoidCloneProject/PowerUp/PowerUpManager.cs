using System;
using System.Collections.Generic;
using ArkanoidCloneProject.Paddle;
using ArkanoidCloneProject.Physics;
using ArkanoidCloneProject.PowerUp.Effects;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ArkanoidCloneProject.PowerUp
{
    public class PowerUpManager : ITickable, IDisposable
    {
        private readonly PowerUpContext _context;
        private readonly BrickManager _brickManager;
        private readonly List<ActiveEffect> _activeEffects = new List<ActiveEffect>();
        private readonly List<PowerUpPickup> _activePickups = new List<PowerUpPickup>();

        private struct ActiveEffect
        {
            public IPowerUpEffect Effect;
            public float TimeRemaining;
        }

        [Inject]
        public PowerUpManager(BrickManager brickManager, BallManager ballManager, BallSettings ballSettings, PaddlePlacer paddlePlacer)
        {
            _brickManager = brickManager;
            _context = new PowerUpContext(paddlePlacer, ballManager, ballSettings.BallSpeed);
            _brickManager.OnBrickDestroyed += HandleBrickDestroyed;
        }

        private void HandleBrickDestroyed(Brick brick, int score)
        {
            if (!brick.HasPowerUp) return;

            var pickup = CreatePickup(brick.transform.position, brick.PowerUpType);
            pickup.OnPickedUp += HandlePickupCollected;
            _activePickups.Add(pickup);
        }

        private void HandlePickupCollected(PowerUpPickup pickup)
        {
            pickup.OnPickedUp -= HandlePickupCollected;
            _activePickups.Remove(pickup);

            ApplyEffect(pickup.PowerUpType);
            UnityEngine.Object.Destroy(pickup.gameObject);
        }

        private void ApplyEffect(PowerUpType type)
        {
            var effect = CreateEffect(type);
            effect.Apply(_context);

            if (effect.HasDuration)
            {
                _activeEffects.Add(new ActiveEffect { Effect = effect, TimeRemaining = effect.Duration });
            }
        }

        public void Tick()
        {
            for (int i = _activeEffects.Count - 1; i >= 0; i--)
            {
                var entry = _activeEffects[i];
                entry.TimeRemaining -= Time.deltaTime;

                if (entry.TimeRemaining <= 0f)
                {
                    entry.Effect.Revert(_context);
                    _activeEffects.RemoveAt(i);
                }
                else
                {
                    _activeEffects[i] = entry;
                }
            }
        }

        public void ClearAllEffects()
        {
            foreach (var entry in _activeEffects)
                entry.Effect.Revert(_context);
            _activeEffects.Clear();

            foreach (var pickup in _activePickups)
            {
                if (pickup != null && pickup.gameObject != null)
                    UnityEngine.Object.Destroy(pickup.gameObject);
            }
            _activePickups.Clear();
        }

        public void Dispose()
        {
            _brickManager.OnBrickDestroyed -= HandleBrickDestroyed;
        }

        private static PowerUpPickup CreatePickup(Vector3 position, PowerUpType type)
        {
            var go = new GameObject($"PowerUpPickup_{type}");
            go.transform.position = position;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreateCircleSprite();
            sr.color = GetPickupColor(type);
            go.transform.localScale = Vector3.one * 0.4f;

            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;

            var pickup = go.AddComponent<PowerUpPickup>();
            pickup.Initialize(type);
            return pickup;
        }

        private static Sprite CreateCircleSprite()
        {
            const int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float radius = size / 2f;
            var center = new Vector2(radius, radius);

            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                tex.SetPixel(x, y, dist <= radius ? Color.white : Color.clear);
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        }

        private static Color GetPickupColor(PowerUpType type) => type switch
        {
            PowerUpType.WidePaddle => new Color(0.2f, 0.8f, 0.2f),
            PowerUpType.MultiBall  => new Color(0.2f, 0.5f, 1f),
            PowerUpType.BallSpeedUp => new Color(1f, 0.6f, 0.1f),
            _ => Color.white,
        };

        private static IPowerUpEffect CreateEffect(PowerUpType type) => type switch
        {
            PowerUpType.WidePaddle  => new WidePaddleEffect(),
            PowerUpType.MultiBall   => new MultiBallEffect(),
            PowerUpType.BallSpeedUp => new BallSpeedEffect(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }
}
