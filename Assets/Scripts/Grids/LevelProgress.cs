using System.Collections.Generic;

namespace Grids
{
    public class LevelProgress
    {
        public FieldProgress FieldProgress { get; private set; }
        
        public List<string> Words { get; private set; }

        public LevelProgress(FieldProgress fieldProgress, List<string> words)
        {
            FieldProgress = fieldProgress;
            Words = words;
        }
    }
}