using System.IO;
using UnityEditor;
using UnityEngine;

namespace ArkanoidCloneProject.LevelEditor.Editor
{
    public class LevelEditorWindow : EditorWindow
    {
        private const float CELL_SIZE = 30f;
        private readonly string _savePath = "Assets/Levels";
        private string _addressablePrefix = "";
        private LevelEditorConfig _config;
        private bool _isPainting;
        private bool _isPowerUpMode;
        private LevelCollection _levelCollection;
        private LevelData _levelData;
        private Vector2 _levelListScrollPosition;
        private string _levelName = "New_Level";
        private int _newColumns = 8;

        private int _newRows = 10;
        private Vector2 _paletteScrollPosition;
        private Vector2 _scrollPosition;
        private int _selectedLevelIndex = -1;

        private int _selectedPaletteIndex;
        private bool _showLevelList;

        private void OnEnable()
        {
            _levelData = new LevelData();
            _newRows = _levelData.gridSize.rows;
            _newColumns = _levelData.gridSize.columns;
        }

        private void OnDisable()
        {
        }

        private void OnGUI()
        {
            DrawToolbar();
            DrawLevelCollectionSection();
            DrawPalette();
            DrawGrid();
        }

        [MenuItem("Tools/Arkanoid/Level Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<LevelEditorWindow>("Level Editor");
            window.minSize = new Vector2(400, 600);
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Level Settings", EditorStyles.boldLabel);

            _config = (LevelEditorConfig)EditorGUILayout.ObjectField("Config", _config, typeof(LevelEditorConfig),
                false);
            _levelCollection = (LevelCollection)EditorGUILayout.ObjectField("Level Collection", _levelCollection,
                typeof(LevelCollection), false);

            EditorGUILayout.Space(5);

            _levelName = EditorGUILayout.TextField("Level Name", _levelName);
            if (_levelData != null) _levelData.levelName = _levelName;

            _addressablePrefix = EditorGUILayout.TextField("Addressable Prefix", _addressablePrefix);

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            _newRows = EditorGUILayout.IntField("Rows", _newRows);
            _newColumns = EditorGUILayout.IntField("Columns", _newColumns);
            if (GUILayout.Button("Resize", GUILayout.Width(60))) ResizeGrid();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            _levelData.tileSize.x = EditorGUILayout.FloatField("Tile Width", _levelData.tileSize.x);
            _levelData.tileSize.y = EditorGUILayout.FloatField("Tile Height", _levelData.tileSize.y);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = _isPowerUpMode ? Color.magenta : Color.white;
            if (GUILayout.Button(_isPowerUpMode ? "Power-Up Mode (ON)" : "Power-Up Mode (OFF)"))
                _isPowerUpMode = !_isPowerUpMode;
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("New Level")) NewLevel();
            if (GUILayout.Button("Save")) SaveLevel();
            if (GUILayout.Button("Load")) LoadLevel();
            if (GUILayout.Button("Clear All")) ClearAll();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawLevelCollectionSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Level Collection Management", EditorStyles.boldLabel);
            if (GUILayout.Button(_showLevelList ? "Hide" : "Show", GUILayout.Width(50)))
                _showLevelList = !_showLevelList;
            EditorGUILayout.EndHorizontal();

            if (_levelCollection == null)
            {
                EditorGUILayout.HelpBox("Assign a LevelCollection to manage levels.", MessageType.Info);
                if (GUILayout.Button("Create New Level Collection")) CreateNewLevelCollection();
            }
            else if (_showLevelList)
            {
                EditorGUILayout.LabelField($"Total Levels: {_levelCollection.LevelCount}");

                _levelListScrollPosition =
                    EditorGUILayout.BeginScrollView(_levelListScrollPosition, GUILayout.Height(150));

                var levelAddresses = _levelCollection.GetAllLevelAddresses();
                for (var i = 0; i < levelAddresses.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    GUI.backgroundColor = _selectedLevelIndex == i ? Color.cyan : Color.white;
                    if (GUILayout.Button($"{i}: {levelAddresses[i]}", GUILayout.ExpandWidth(true)))
                        _selectedLevelIndex = i;
                    GUI.backgroundColor = Color.white;

                    if (GUILayout.Button("↑", GUILayout.Width(25))) MoveLevelUp(i);
                    if (GUILayout.Button("↓", GUILayout.Width(25))) MoveLevelDown(i);
                    if (GUILayout.Button("X", GUILayout.Width(25))) RemoveLevelFromCollection(i);

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Current Level")) AddCurrentLevelToCollection();
                if (GUILayout.Button("Remove Selected")) RemoveSelectedLevelFromCollection();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawPalette()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Palette", EditorStyles.boldLabel);

            if (_config == null)
            {
                EditorGUILayout.HelpBox("Assign a LevelEditorConfig to use custom palette.", MessageType.Info);
                if (GUILayout.Button("Create New Config")) CreateNewConfig();
            }

            _paletteScrollPosition = EditorGUILayout.BeginScrollView(_paletteScrollPosition, GUILayout.Height(60));
            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = _selectedPaletteIndex == -1 ? Color.green : Color.white;
            if (GUILayout.Button("Empty", GUILayout.Width(50), GUILayout.Height(40))) _selectedPaletteIndex = -1;

            if (_config != null)
                for (var i = 0; i < _config.palette.Count; i++)
                {
                    var entry = _config.palette[i];
                    GUI.backgroundColor = _selectedPaletteIndex == i ? Color.green : entry.editorColor;
                    if (GUILayout.Button(entry.name, GUILayout.Width(50), GUILayout.Height(40)))
                        _selectedPaletteIndex = i;
                }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawGrid()
        {
            if (_levelData == null) return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Grid (Left-Click: Paint, Right-Click: Erase)", EditorStyles.boldLabel);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            var e = Event.current;
            var gridRect = GUILayoutUtility.GetRect(
                _levelData.gridSize.columns * CELL_SIZE,
                _levelData.gridSize.rows * CELL_SIZE
            );

            for (var row = 0; row < _levelData.gridSize.rows; row++)
            for (var col = 0; col < _levelData.gridSize.columns; col++)
            {
                var cellRect = new Rect(
                    gridRect.x + col * CELL_SIZE,
                    gridRect.y + row * CELL_SIZE,
                    CELL_SIZE - 1,
                    CELL_SIZE - 1
                );

                var tile = _levelData.GetTile(row, col);
                var cellColor = GetTileColor(tile);
                EditorGUI.DrawRect(cellRect, cellColor);

                if (tile != null && tile.hasPowerUp) DrawPowerUpIndicator(cellRect);

                if (cellRect.Contains(e.mousePosition)) HandleCellInput(e, row, col);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void HandleCellInput(Event e, int row, int col)
        {
            if (e.type == EventType.MouseDown)
            {
                _isPainting = true;
                ProcessCellClick(e, row, col);
                e.Use();
            }
            else if (e.type == EventType.MouseDrag && _isPainting)
            {
                ProcessCellClick(e, row, col);
                e.Use();
            }
            else if (e.type == EventType.MouseUp)
            {
                _isPainting = false;
            }
        }

        private void ProcessCellClick(Event e, int row, int col)
        {
            var tile = _levelData.GetTile(row, col);
            if (tile == null) return;

            if (_isPowerUpMode)
            {
                if (e.button == 0) tile.hasPowerUp = !tile.hasPowerUp;
            }
            else
            {
                if (e.button == 0)
                {
                    tile.typeIndex = _selectedPaletteIndex;
                    tile.hasPowerUp = false;
                }
                else if (e.button == 1)
                {
                    tile.typeIndex = -1;
                    tile.hasPowerUp = false;
                }
            }

            Repaint();
        }

        private Color GetTileColor(TileData tile)
        {
            if (tile == null || tile.IsEmpty)
            {
                if (_config != null) return _config.emptyTileColor;
                return new Color(0.2f, 0.2f, 0.2f, 1f);
            }

            if (_config != null && tile.typeIndex >= 0 && tile.typeIndex < _config.palette.Count)
                return _config.palette[tile.typeIndex].editorColor;

            return new Color(0.2f, 0.2f, 0.2f, 1f);
        }

        private void DrawPowerUpIndicator(Rect cellRect)
        {
            var originalColor = GUI.color;
            GUI.color = _config != null ? _config.powerUpIndicatorColor : Color.magenta;

            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 14
            };

            GUI.Label(cellRect, "P", style);
            GUI.color = originalColor;
        }

        private void CreateNewConfig()
        {
            var path = EditorUtility.SaveFilePanelInProject("Create Level Editor Config", "LevelEditorConfig", "asset",
                "Choose location for the config file");
            if (string.IsNullOrEmpty(path)) return;

            var config = CreateInstance<LevelEditorConfig>();
            AssetDatabase.CreateAsset(config, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _config = config;
            EditorGUIUtility.PingObject(config);

            Debug.Log($"LevelEditorConfig created at: {path}");
        }

        private void CreateNewLevelCollection()
        {
            var path = EditorUtility.SaveFilePanelInProject("Create Level Collection", "LevelCollection", "asset",
                "Choose location for the level collection file");
            if (string.IsNullOrEmpty(path)) return;

            var collection = CreateInstance<LevelCollection>();
            AssetDatabase.CreateAsset(collection, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _levelCollection = collection;
            EditorGUIUtility.PingObject(collection);

            Debug.Log($"LevelCollection created at: {path}");
        }

        private void AddCurrentLevelToCollection()
        {
            if (_levelCollection == null)
            {
                Debug.LogWarning("No LevelCollection assigned!");
                return;
            }

            var addressableName = GetAddressableName();
            _levelCollection.AddLevel(addressableName);
            EditorUtility.SetDirty(_levelCollection);
            AssetDatabase.SaveAssets();

            Debug.Log($"Added level '{addressableName}' to collection");
        }

        private void RemoveSelectedLevelFromCollection()
        {
            if (_levelCollection == null || _selectedLevelIndex < 0) return;

            _levelCollection.RemoveLevelAt(_selectedLevelIndex);
            EditorUtility.SetDirty(_levelCollection);
            AssetDatabase.SaveAssets();

            _selectedLevelIndex = -1;
        }

        private void RemoveLevelFromCollection(int index)
        {
            if (_levelCollection == null) return;

            _levelCollection.RemoveLevelAt(index);
            EditorUtility.SetDirty(_levelCollection);
            AssetDatabase.SaveAssets();

            if (_selectedLevelIndex == index) _selectedLevelIndex = -1;
        }

        private void MoveLevelUp(int index)
        {
            if (_levelCollection == null || index <= 0) return;

            _levelCollection.SwapLevels(index, index - 1);
            EditorUtility.SetDirty(_levelCollection);
            AssetDatabase.SaveAssets();
        }

        private void MoveLevelDown(int index)
        {
            if (_levelCollection == null || index >= _levelCollection.LevelCount - 1) return;

            _levelCollection.SwapLevels(index, index + 1);
            EditorUtility.SetDirty(_levelCollection);
            AssetDatabase.SaveAssets();
        }

        private string GetAddressableName()
        {
            if (string.IsNullOrEmpty(_addressablePrefix)) return _levelName;
            return $"{_addressablePrefix}{_levelName}";
        }

        private void ResizeGrid()
        {
            _newRows = Mathf.Max(1, _newRows);
            _newColumns = Mathf.Max(1, _newColumns);
            _levelData.Resize(_newRows, _newColumns);
            Repaint();
        }

        private void NewLevel()
        {
            _levelData = new LevelData(_levelName, _newRows, _newColumns);
            Repaint();
        }

        private void ClearAll()
        {
            if (EditorUtility.DisplayDialog("Clear All", "Are you sure you want to clear all tiles?", "Yes", "No"))
            {
                _levelData.InitializeTiles();
                Repaint();
            }
        }

        private void SaveLevel()
        {
            if (!Directory.Exists(_savePath)) Directory.CreateDirectory(_savePath);

            var path = EditorUtility.SaveFilePanel("Save Level", _savePath, _levelData.levelName, "json");
            if (string.IsNullOrEmpty(path)) return;

            var json = JsonUtility.ToJson(_levelData, true);
            File.WriteAllText(path, json);
            AssetDatabase.Refresh();

            if (_levelCollection != null)
            {
                var addressableName = GetAddressableName();
                if (!_levelCollection.GetAllLevelAddresses().Contains(addressableName))
                    if (EditorUtility.DisplayDialog("Add to Collection",
                            $"Do you want to add '{addressableName}' to the level collection?", "Yes", "No"))
                    {
                        _levelCollection.AddLevel(addressableName);
                        EditorUtility.SetDirty(_levelCollection);
                        AssetDatabase.SaveAssets();
                    }
            }

            Debug.Log($"Level saved to: {path}");
        }

        private void LoadLevel()
        {
            var path = EditorUtility.OpenFilePanel("Load Level", _savePath, "json");
            if (string.IsNullOrEmpty(path)) return;

            var json = File.ReadAllText(path);
            _levelData = JsonUtility.FromJson<LevelData>(json);

            _levelName = _levelData.levelName;
            _newRows = _levelData.gridSize.rows;
            _newColumns = _levelData.gridSize.columns;

            Repaint();

            Debug.Log($"Level loaded from: {path}");
        }
    }
}