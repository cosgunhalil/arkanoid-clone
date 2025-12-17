using System.Collections.Generic;
using UnityEngine;

namespace ArkanoidCloneProject.LevelEditor
{
    [CreateAssetMenu(fileName = "LevelEditorConfig", menuName = "Arkanoid/Level Editor Config")]
    public class LevelEditorConfig : ScriptableObject
    {
        public List<TilePaletteEntry> palette = new List<TilePaletteEntry>();
        public Color emptyTileColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        public Color gridLineColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        public Color powerUpIndicatorColor = Color.magenta;
        public float defaultUniformScale = 1f;
    }

    [System.Serializable]
    public class TilePaletteEntry
    {
        public string name;
        public GameObject prefab;
        public Color editorColor = Color.white;
    }
}
