using UnityEngine;

namespace Grids
{
    [CreateAssetMenu(fileName = "Block State Color Palette", menuName = "Block State Color Palette", order = 0)]
    public class BlockStateColorPalette : ScriptableObject
    {
        [System.Serializable]
        public struct StateColorPair
        {
            public BlockState state;
            public Color color;
        }

        [SerializeField] private StateColorPair[] colors;

        public Color GetColor(BlockState state)
        {
            foreach (var pair in colors)
            {
                if (pair.state == state)
                    return pair.color;
            }
            return Color.white;
        }
    }
}