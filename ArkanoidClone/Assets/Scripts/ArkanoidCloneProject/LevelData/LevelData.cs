using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArkanoidCloneProject.LevelEditor
{
    [Serializable]
    public class LevelData
    {
        public string levelName;
        public GridSize gridSize;
        public TileSize tileSize;
        public List<TileData> tiles;

        public LevelData()
        {
            levelName = "New_Level";
            gridSize = new GridSize(10, 8);
            tileSize = new TileSize(1.0f, 0.5f);
            tiles = new List<TileData>();
            InitializeTiles();
        }

        public LevelData(string name, int rows, int columns)
        {
            levelName = name;
            gridSize = new GridSize(rows, columns);
            tileSize = new TileSize(1.0f, 0.5f);
            tiles = new List<TileData>();
            InitializeTiles();
        }

        public void InitializeTiles()
        {
            tiles.Clear();
            int totalTiles = gridSize.rows * gridSize.columns;
            for (int i = 0; i < totalTiles; i++)
            {
                tiles.Add(new TileData());
            }
        }

        public int GetIndex(int row, int col)
        {
            return row * gridSize.columns + col;
        }

        public Vector2Int GetCoordinates(int index)
        {
            int row = index / gridSize.columns;
            int col = index % gridSize.columns;
            return new Vector2Int(col, row);
        }

        public TileData GetTile(int row, int col)
        {
            int index = GetIndex(row, col);
            if (index >= 0 && index < tiles.Count)
            {
                return tiles[index];
            }
            return null;
        }

        public void SetTile(int row, int col, TileData data)
        {
            int index = GetIndex(row, col);
            if (index >= 0 && index < tiles.Count)
            {
                tiles[index] = data;
            }
        }

        public void Resize(int newRows, int newColumns)
        {
            var newTiles = new List<TileData>();
            for (int row = 0; row < newRows; row++)
            {
                for (int col = 0; col < newColumns; col++)
                {
                    if (row < gridSize.rows && col < gridSize.columns)
                    {
                        newTiles.Add(GetTile(row, col));
                    }
                    else
                    {
                        newTiles.Add(new TileData());
                    }
                }
            }

            gridSize.rows = newRows;
            gridSize.columns = newColumns;
            tiles = newTiles;
        }
    }

    [Serializable]
    public class GridSize
    {
        public int rows;
        public int columns;

        public GridSize(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;
        }
    }

    [Serializable]
    public class TileSize
    {
        public float x;
        public float y;

        public TileSize(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
