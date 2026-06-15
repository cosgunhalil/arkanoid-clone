using System;
using ArkanoidCloneProject.Physics;
using UnityEngine;
using VContainer;

namespace ArkanoidCloneProject.VFX
{
    public class BrickExplosionController : IBrickExplosionEffect, IDisposable
    {
        private const float EffectDuration  = 0.55f;
        private const string ShaderName     = "Arkanoid/BrickExplosion";

        private readonly BrickManager _brickManager;
        private readonly Material     _material;

        [Inject]
        public BrickExplosionController(BrickManager brickManager)
        {
            _brickManager = brickManager;

            var shader = Shader.Find(ShaderName);
            if (shader == null)
                Debug.LogError($"[BrickExplosionController] Shader '{ShaderName}' not found. Ensure BrickExplosion.shader is in the project.");

            _material = new Material(shader) { name = "BrickExplosionMat" };

            _brickManager.OnBrickDestroyed += HandleBrickDestroyed;
        }

        private void HandleBrickDestroyed(Brick brick, int _)
        {
            var sr    = brick.GetComponent<SpriteRenderer>();
            var color = sr != null ? sr.color : Color.white;
            Spawn(brick.transform.position, brick.transform.lossyScale, color);
        }

        public void Spawn(Vector3 worldPosition, Vector3 worldSize, Color color)
        {
            BrickExplosionEffect.Spawn(worldPosition, worldSize, color, EffectDuration, _material);
        }

        public void Dispose()
        {
            _brickManager.OnBrickDestroyed -= HandleBrickDestroyed;
            if (_material != null)
                UnityEngine.Object.Destroy(_material);
        }
    }
}
