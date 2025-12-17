using System.Collections.Generic;
using UnityEngine;

namespace ArkanoidCloneProject.LevelEditor
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private LevelEditorConfig _config;
        [SerializeField] private Transform _levelContainer;
        [SerializeField] private float _uniformScale = 1f;

        private List<GameObject> _spawnedTiles = new List<GameObject>();

        public void LoadLevel(TextAsset jsonAsset)
        {
            if (jsonAsset == null) return;
            LoadLevelFromJson(jsonAsset.text);
        }

        public void LoadLevel(string json)
        {
            LoadLevelFromJson(json);
        }

        public void LoadLevelFromJson(string json)
        {
            ClearLevel();

            LevelData levelData = JsonUtility.FromJson<LevelData>(json);
            if (levelData == null) return;

            Transform container = _levelContainer != null ? _levelContainer : transform;

            for (int row = 0; row < levelData.gridSize.rows; row++)
            {
                for (int col = 0; col < levelData.gridSize.columns; col++)
                {
                    TileData tile = levelData.GetTile(row, col);
                    if (tile == null || tile.IsEmpty) continue;

                    if (tile.typeIndex >= 0 && tile.typeIndex < _config.palette.Count)
                    {
                        var prefab = _config.palette[tile.typeIndex].prefab;
                        if (prefab != null)
                        {
                            Vector3 position = new Vector3(
                                col * levelData.tileSize.x,
                                -row * levelData.tileSize.y,
                                0
                            );

                            var obj = Instantiate(prefab, container);
                            obj.transform.localPosition = position;
                            obj.transform.localScale = Vector3.one * _uniformScale;

                            if (tile.hasPowerUp)
                            {
                                var powerUpHolder = obj.GetComponent<IPowerUpHolder>();
                                if (powerUpHolder != null)
                                {
                                    powerUpHolder.SetHasPowerUp(true);
                                }
                            }

                            _spawnedTiles.Add(obj);
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

        public LevelData GetLevelDataFromJson(string json)
        {
            return JsonUtility.FromJson<LevelData>(json);
        }
    }

    public interface IPowerUpHolder
    {
        void SetHasPowerUp(bool value);
        bool HasPowerUp { get; }
    }
}
