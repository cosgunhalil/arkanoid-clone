using System.Collections.Generic;
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
        private List<GameObject> _spawnedTiles = new List<GameObject>();
        private AsyncOperationHandle<TextAsset> _levelAssetHandle;

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

                            if (tile.hasPowerUp)
                            {
                                //TODO: handle
                            }

                            _spawnedTiles.Add(tileObject);
                        }
                    }
                }
            }
        }

        public void ClearLevel()
        {
            for (int i = _spawnedTiles.Count - 1; i >= 0; i--)
            {
                if (_spawnedTiles[i] != null)
                {
                    Destroy(_spawnedTiles[i]);
                }
            }
            _spawnedTiles.Clear();
        }

        public LevelData GetCurrentLevelData()
        {
            return _currentLevelData;
        }

        public List<GameObject> GetSpawnedTiles()
        {
            return _spawnedTiles;
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
