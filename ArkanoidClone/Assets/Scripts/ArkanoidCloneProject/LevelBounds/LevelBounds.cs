using UnityEngine;

namespace ArkanoidCloneProject.LevelEditor
{
    public struct LevelBounds
    {
        public Vector2 TopLeft;
        public Vector2 TopRight;
        public Vector2 BottomLeft;
        public Vector2 BottomRight;
        public Vector2 Center;
        public float Width;
        public float Height;

        public LevelBounds(Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            Center = (topLeft + bottomRight) / 2f;
            Width = topRight.x - topLeft.x;
            Height = topLeft.y - bottomLeft.y;
        }
    }
}
