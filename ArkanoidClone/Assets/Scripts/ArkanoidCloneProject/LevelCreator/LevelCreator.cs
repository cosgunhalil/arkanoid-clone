using System;
using System.Collections.Generic;
using ArkanoidCloneProject.Physics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ArkanoidCloneProject.LevelEditor
{
    public class LevelCreator : MonoBehaviour
    {
        [SerializeField] private LevelEditorConfig _config;
        [SerializeField] private Transform _levelContainer;

        private LevelData _currentLevelData;
        private List<Brick> _spawnedBricks = new List<Brick>();
        private AsyncOperationHandle<TextAsset> _levelAssetHandle;
        private LevelBounds _levelBounds;

        public event Action<LevelBounds> OnLevelCreated;

        public LevelBounds LevelBounds => _levelBounds;

        public async UniTask LoadAndCreateLevelAsync(string levelAddress)
        {
            await LoadLevelDataAsync(levelAddress);
            CreateLevel();
        }

        public async UniTask LoadLevelDataAsync(string levelAddress)
        {
            _levelAssetHandle = Addressables.LoadAssetAsync<TextAsset>(levelAddress);
            TextAsset levelAsset = await _levelAssetHandle.ToUniTask();
            
            if (levelAsset != null)
            {
                _currentLevelData = JsonUtility.FromJson<LevelData>(levelAsset.text);
            }
        }

        public void CreateLevel()
        {
            if (_currentLevelData == null) return;

            ClearLevel();

            Transform container = _levelContainer != null ? _levelContainer : transform;

            float horizontalSpacing = 0.1f;
            float verticalSpacing = 0.1f;

            for (int row = 0; row < _currentLevelData.gridSize.rows; row++)
            {
                for (int col = 0; col < _currentLevelData.gridSize.columns; col++)
                {
                    TileData tile = _currentLevelData.GetTile(row, col);
                    if (tile == null || tile.IsEmpty) continue;

                    if (tile.typeIndex >= 0 && tile.typeIndex < _config.palette.Count)
                    {
                        GameObject prefab = _config.palette[tile.typeIndex].prefab;
                        if (prefab != null)
                        {
                            Vector3 position = new Vector3(
                                col * (_currentLevelData.tileSize.x + horizontalSpacing),
                                -row * (_currentLevelData.tileSize.y + verticalSpacing),
                                0
                            );

                            GameObject tileObject = Instantiate(prefab, container);
                            tileObject.transform.localPosition = position;
                            tileObject.transform.localScale = new Vector2(_currentLevelData.tileSize.x, _currentLevelData.tileSize.y);
                            tileObject.AddComponent<BoxCollider2D>();
                            tileObject.tag = "Brick";
                            var brick = tileObject.AddComponent<Brick>();

                            if (tile.hasPowerUp)
                            {
                                brick.SetRandomPowerUp();
                            }

                            _spawnedBricks.Add(brick);
                        }
                    }
                }
            }

            CalculateLevelBounds(horizontalSpacing, verticalSpacing);
            OnLevelCreated?.Invoke(_levelBounds);
        }

        private void CalculateLevelBounds(float horizontalSpacing, float verticalSpacing)
        {
            if (_currentLevelData == null)
            {
                _levelBounds = new LevelBounds(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
                return;
            }

            Transform container = _levelContainer != null ? _levelContainer : transform;
            Vector3 containerPosition = container.position;

            float tileWidth = _currentLevelData.tileSize.x;
            float tileHeight = _currentLevelData.tileSize.y;
            int rows = _currentLevelData.gridSize.rows;
            int columns = _currentLevelData.gridSize.columns;

            float totalWidth = columns * tileWidth + (columns - 1) * horizontalSpacing;
            float totalHeight = rows * tileHeight + (rows - 1) * verticalSpacing;

            float halfTileWidth = tileWidth / 2f;
            float halfTileHeight = tileHeight / 2f;

            float leftEdge = containerPosition.x - halfTileWidth;
            float rightEdge = containerPosition.x + totalWidth - halfTileWidth;
            float topEdge = containerPosition.y + halfTileHeight;
            float bottomEdge = containerPosition.y - totalHeight + halfTileHeight;

            Vector2 topLeft = new Vector2(leftEdge, topEdge);
            Vector2 topRight = new Vector2(rightEdge, topEdge);
            Vector2 bottomLeft = new Vector2(leftEdge, bottomEdge);
            Vector2 bottomRight = new Vector2(rightEdge, bottomEdge);

            _levelBounds = new LevelBounds(topLeft, topRight, bottomLeft, bottomRight);
        }

        public void ClearLevel()
        {
            for (int i = _spawnedBricks.Count - 1; i >= 0; i--)
            {
                if (_spawnedBricks[i] != null)
                {
                    Destroy(_spawnedBricks[i]);
                }
            }
            _spawnedBricks.Clear();
        }

        public LevelData GetCurrentLevelData()
        {
            return _currentLevelData;
        }

        public List<Brick> GetSpawnedBricks()
        {
            return _spawnedBricks;
        }

        private void OnDestroy()
        {
            if (_levelAssetHandle.IsValid())
            {
                Addressables.Release(_levelAssetHandle);
            }
        }
    }
}