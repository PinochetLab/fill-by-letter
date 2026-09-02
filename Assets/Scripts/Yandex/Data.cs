using Grids;
using Levels;

namespace Yandex
{
    [System.Serializable]
    public class Data
    {
        public int Money { get; set; } = 500;
        public int LevelIndex { get; set; } = 0;
        public Level Level { get; set; } = null;
        public LevelProgress LevelProgress { get; set; } = null;
    }
}