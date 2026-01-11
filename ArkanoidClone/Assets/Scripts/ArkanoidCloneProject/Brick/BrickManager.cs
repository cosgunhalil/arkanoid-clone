using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace ArkanoidCloneProject.Physics
{
    public class BrickManager
    {
        private readonly ArkanoidPhysicsWorld _physicsWorld;
        private readonly IObjectResolver _resolver;
        private readonly List<Brick> _activeBricks;
        private int _totalScore;

        public event Action OnAllBricksDestroyed;
        public event Action<Brick, int> OnBrickDestroyed;
        public event Action<int> OnScoreChanged;

        public int ActiveBrickCount => _activeBricks.Count;
        public int TotalScore => _totalScore;

        [Inject]
        public BrickManager(ArkanoidPhysicsWorld physicsWorld, IObjectResolver resolver)
        {
            _physicsWorld = physicsWorld;
            _resolver = resolver;
            _activeBricks = new List<Brick>();
            _totalScore = 0;
        }

        public void RegisterBrick(Brick brick)
        {
            _resolver.Inject(brick);
            _activeBricks.Add(brick);
            brick.OnBrickDestroyed += HandleBrickDestroyed;
        }

        public void RegisterBricksFromGameObjects(List<GameObject> brickObjects)
        {
            int count = brickObjects.Count;
            for (int i = 0; i < count; i++)
            {
                Brick brick = brickObjects[i].GetComponent<Brick>();
                if (brick != null)
                {
                    RegisterBrick(brick);
                }
            }
        }

        private void HandleBrickDestroyed(Brick brick)
        {
            brick.OnBrickDestroyed -= HandleBrickDestroyed;
            _activeBricks.Remove(brick);
            
            _totalScore += brick.ScoreValue;
            OnScoreChanged?.Invoke(_totalScore);
            OnBrickDestroyed?.Invoke(brick, brick.ScoreValue);
            
            if (_activeBricks.Count == 0)
            {
                OnAllBricksDestroyed?.Invoke();
            }
        }

        public void UnregisterAllBricks()
        {
            int count = _activeBricks.Count;
            for (int i = 0; i < count; i++)
            {
                _activeBricks[i].OnBrickDestroyed -= HandleBrickDestroyed;
            }
            _activeBricks.Clear();
        }

        public void ResetScore()
        {
            _totalScore = 0;
            OnScoreChanged?.Invoke(_totalScore);
        }

        public List<Brick> GetActiveBricks()
        {
            return new List<Brick>(_activeBricks);
        }
    }
}
