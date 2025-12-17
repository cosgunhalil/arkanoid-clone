using System.IO;
using UnityEngine;
using UnityEditor;

namespace ArkanoidCloneProject.LevelEditor.Editor
{
    public class LevelEditorWindow : EditorWindow
    {
        private LevelData _levelData;
        private LevelEditorConfig _config;

        private int _selectedPaletteIndex = 0;
        private bool _isPowerUpMode = false;
        private bool _isPainting = false;
        private Vector2 _scrollPosition;
        private Vector2 _paletteScrollPosition;

        private int _newRows = 10;
        private int _newColumns = 8;
        private string _levelName = "New_Level";
        private string _savePath = "Assets/Levels";

        private const float CELL_SIZE = 30f;

        [MenuItem("Tools/Arkanoid/Level Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<LevelEditorWindow>("Level Editor");
            window.minSize = new Vector2(400, 600);
        }

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
            DrawPalette();
            DrawGrid();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Level Settings", EditorStyles.boldLabel);

            _config = (LevelEditorConfig)EditorGUILayout.ObjectField("Config", _config, typeof(LevelEditorConfig), false);

            EditorGUILayout.Space(5);

            _levelName = EditorGUILayout.TextField("Level Name", _levelName);
            if (_levelData != null)
            {
                _levelData.levelName = _levelName;
            }

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            _newRows = EditorGUILayout.IntField("Rows", _newRows);
            _newColumns = EditorGUILayout.IntField("Columns", _newColumns);
            if (GUILayout.Button("Resize", GUILayout.Width(60)))
            {
                ResizeGrid();
            }
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
            {
                _isPowerUpMode = !_isPowerUpMode;
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("New Level"))
            {
                NewLevel();
            }
            if (GUILayout.Button("Save"))
            {
                SaveLevel();
            }
            if (GUILayout.Button("Load"))
            {
                LoadLevel();
            }
            if (GUILayout.Button("Clear All"))
            {
                ClearAll();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawPalette()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Palette", EditorStyles.boldLabel);

            if (_config == null)
            {
                EditorGUILayout.HelpBox("Assign a LevelEditorConfig to use custom palette.", MessageType.Info);
                if (GUILayout.Button("Create New Config"))
                {
                    CreateNewConfig();
                }
            }

            _paletteScrollPosition = EditorGUILayout.BeginScrollView(_paletteScrollPosition, GUILayout.Height(60));
            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = _selectedPaletteIndex == -1 ? Color.green : Color.white;
            if (GUILayout.Button("Empty", GUILayout.Width(50), GUILayout.Height(40)))
            {
                _selectedPaletteIndex = -1;
            }

            if (_config != null)
            {
                for (int i = 0; i < _config.palette.Count; i++)
                {
                    var entry = _config.palette[i];
                    GUI.backgroundColor = _selectedPaletteIndex == i ? Color.green : entry.editorColor;
                    if (GUILayout.Button(entry.name, GUILayout.Width(50), GUILayout.Height(40)))
                    {
                        _selectedPaletteIndex = i;
                    }
                }
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

            Event e = Event.current;
            Rect gridRect = GUILayoutUtility.GetRect(
                _levelData.gridSize.columns * CELL_SIZE,
                _levelData.gridSize.rows * CELL_SIZE
            );

            for (int row = 0; row < _levelData.gridSize.rows; row++)
            {
                for (int col = 0; col < _levelData.gridSize.columns; col++)
                {
                    Rect cellRect = new Rect(
                        gridRect.x + col * CELL_SIZE,
                        gridRect.y + row * CELL_SIZE,
                        CELL_SIZE - 1,
                        CELL_SIZE - 1
                    );

                    TileData tile = _levelData.GetTile(row, col);
                    Color cellColor = GetTileColor(tile);
                    EditorGUI.DrawRect(cellRect, cellColor);

                    if (tile != null && tile.hasPowerUp)
                    {
                        DrawPowerUpIndicator(cellRect);
                    }

                    if (cellRect.Contains(e.mousePosition))
                    {
                        HandleCellInput(e, row, col);
                    }
                }
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
            TileData tile = _levelData.GetTile(row, col);
            if (tile == null) return;

            if (_isPowerUpMode)
            {
                if (e.button == 0)
                {
                    tile.hasPowerUp = !tile.hasPowerUp;
                }
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
                if (_config != null)
                {
                    return _config.emptyTileColor;
                }
                return new Color(0.2f, 0.2f, 0.2f, 1f);
            }

            if (_config != null && tile.typeIndex >= 0 && tile.typeIndex < _config.palette.Count)
            {
                return _config.palette[tile.typeIndex].editorColor;
            }

            return new Color(0.2f, 0.2f, 0.2f, 1f);
        }

        private void DrawPowerUpIndicator(Rect cellRect)
        {
            Color originalColor = GUI.color;
            GUI.color = _config != null ? _config.powerUpIndicatorColor : Color.magenta;

            GUIStyle style = new GUIStyle(GUI.skin.label)
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
            string path = EditorUtility.SaveFilePanelInProject("Create Level Editor Config", "LevelEditorConfig", "asset", "Choose location for the config file");
            if (string.IsNullOrEmpty(path)) return;

            var config = ScriptableObject.CreateInstance<LevelEditorConfig>();
            AssetDatabase.CreateAsset(config, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _config = config;
            EditorGUIUtility.PingObject(config);

            Debug.Log($"LevelEditorConfig created at: {path}");
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
            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(_savePath);
            }

            string path = EditorUtility.SaveFilePanel("Save Level", _savePath, _levelData.levelName, "json");
            if (string.IsNullOrEmpty(path)) return;

            string json = JsonUtility.ToJson(_levelData, true);
            File.WriteAllText(path, json);
            AssetDatabase.Refresh();

            Debug.Log($"Level saved to: {path}");
        }

        private void LoadLevel()
        {
            string path = EditorUtility.OpenFilePanel("Load Level", _savePath, "json");
            if (string.IsNullOrEmpty(path)) return;

            string json = File.ReadAllText(path);
            _levelData = JsonUtility.FromJson<LevelData>(json);

            _levelName = _levelData.levelName;
            _newRows = _levelData.gridSize.rows;
            _newColumns = _levelData.gridSize.columns;

            Repaint();

            Debug.Log($"Level loaded from: {path}");
        }
    }
}