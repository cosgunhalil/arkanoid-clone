using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArkanoidCloneProject.Physics
{
    public class BrickManager
    {
        private readonly List<Brick> _activeBricks;
        private int _totalScore;

        public event Action OnAllBricksDestroyed;
        public event Action<Brick, int> OnBrickDestroyed;
        public event Action<int> OnScoreChanged;

        public int ActiveBrickCount => _activeBricks.Count;
        public int TotalScore => _totalScore;

        public BrickManager()
        {
            _activeBricks = new List<Brick>();
            _totalScore = 0;
        }

        public void RegisterBricksFromGameObjects(List<Brick> brickObjects)
        {
            var count = brickObjects.Count;
            for (var i = 0; i < count; i++)
            {
                RegisterBrick(brickObjects[i]);
            }
        }
        
        private void RegisterBrick(Brick brick)
        {
            _activeBricks.Add(brick);
            brick.OnBrickDestroyed += HandleBrickDestroyed;
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