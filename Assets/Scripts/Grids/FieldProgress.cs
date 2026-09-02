using System.Collections.Generic;
using UnityEngine;

namespace Grids
{
    public class FieldProgress
    {
        public char?[,] Letters { get; private set; }
        
        public List<Vector2Int> Flags { get; private set; }

        public FieldProgress(char?[,] letters, List<Vector2Int> flags)
        {
            Letters = letters;
            Flags = flags;
        }
    }
}