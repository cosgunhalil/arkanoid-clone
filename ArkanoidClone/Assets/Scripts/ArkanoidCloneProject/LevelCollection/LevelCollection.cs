using System.Collections.Generic;
using UnityEngine;

namespace ArkanoidCloneProject.LevelEditor
{
    [CreateAssetMenu(fileName = "LevelCollection", menuName = "Arkanoid/Level Collection")]
    public class LevelCollection : ScriptableObject
    {
        [SerializeField] private List<string> _levelAddresses = new List<string>();

        public int LevelCount => _levelAddresses.Count;

        public string GetLevelAddress(int index)
        {
            if (index < 0 || index >= _levelAddresses.Count)
            {
                return null;
            }
            return _levelAddresses[index];
        }

        public int GetLevelIndex(string levelAddress)
        {
            return _levelAddresses.IndexOf(levelAddress);
        }

        public string GetNextLevelAddress(string currentLevelAddress)
        {
            int currentIndex = GetLevelIndex(currentLevelAddress);
            if (currentIndex < 0 || currentIndex >= _levelAddresses.Count - 1)
            {
                return null;
            }
            return _levelAddresses[currentIndex + 1];
        }

        public bool HasNextLevel(string currentLevelAddress)
        {
            int currentIndex = GetLevelIndex(currentLevelAddress);
            return currentIndex >= 0 && currentIndex < _levelAddresses.Count - 1;
        }

        public bool HasNextLevel(int currentIndex)
        {
            return currentIndex >= 0 && currentIndex < _levelAddresses.Count - 1;
        }

        public List<string> GetAllLevelAddresses()
        {
            return new List<string>(_levelAddresses);
        }

        public void AddLevel(string levelAddress)
        {
            if (!_levelAddresses.Contains(levelAddress))
            {
                _levelAddresses.Add(levelAddress);
            }
        }

        public void RemoveLevel(string levelAddress)
        {
            _levelAddresses.Remove(levelAddress);
        }

        public void RemoveLevelAt(int index)
        {
            if (index >= 0 && index < _levelAddresses.Count)
            {
                _levelAddresses.RemoveAt(index);
            }
        }

        public void ClearAllLevels()
        {
            _levelAddresses.Clear();
        }

        public void SetLevelAddress(int index, string levelAddress)
        {
            if (index >= 0 && index < _levelAddresses.Count)
            {
                _levelAddresses[index] = levelAddress;
            }
        }

        public void InsertLevel(int index, string levelAddress)
        {
            if (index >= 0 && index <= _levelAddresses.Count)
            {
                _levelAddresses.Insert(index, levelAddress);
            }
        }

        public void SwapLevels(int indexA, int indexB)
        {
            if (indexA >= 0 && indexA < _levelAddresses.Count && indexB >= 0 && indexB < _levelAddresses.Count)
            {
                string temp = _levelAddresses[indexA];
                _levelAddresses[indexA] = _levelAddresses[indexB];
                _levelAddresses[indexB] = temp;
            }
        }
    }
}
