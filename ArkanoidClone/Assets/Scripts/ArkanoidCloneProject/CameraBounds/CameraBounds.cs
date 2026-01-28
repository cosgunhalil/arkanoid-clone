namespace ArkanoidCloneProject
{
    public struct CameraBounds
    {
        public float Left;
        public float Right;
        public float Top;
        public float Bottom;
        public float CenterX;
        public float CenterY;

        public CameraBounds(float left, float right, float top, float bottom, float centerX, float centerY)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
            CenterX = centerX;
            CenterY = centerY;
        }
    }
}