namespace ArkanoidProject 
{
    [System.Serializable]
    public class LevelItemModel
    {
        private float width;
        private float height;
        private LevelItemType levelItemType;

        public LevelItemType LevelItemType
        {
            get { return levelItemType; }
            set { levelItemType = value; }
        }

        public float Height 
        { 
            get => height; 
            set => height = value; 
        }

        public float Width 
        { 
            get => width; 
            set => width = value; 
        }
    }
}


