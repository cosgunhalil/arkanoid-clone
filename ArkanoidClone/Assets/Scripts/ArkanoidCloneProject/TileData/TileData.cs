using System;

namespace ArkanoidCloneProject.LevelEditor
{
    [Serializable]
    public class TileData
    {
        public int typeIndex;
        public bool hasPowerUp;

        public TileData()
        {
            typeIndex = -1;
            hasPowerUp = false;
        }

        public TileData(int typeIndex, bool hasPowerUp)
        {
            this.typeIndex = typeIndex;
            this.hasPowerUp = hasPowerUp;
        }

        public bool IsEmpty => typeIndex < 0;
    }
}
