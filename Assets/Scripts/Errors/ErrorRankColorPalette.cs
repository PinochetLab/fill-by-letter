using System;
using System.Collections.Generic;
using UnityEngine;

namespace Errors
{
    [CreateAssetMenu(fileName = "Error Rank Color Palette", menuName = "Error Rank Color Palette", order = 0)]
    public class ErrorRankColorPalette : ScriptableObject
    {
        [SerializeField] private List<ErrorRankColor> errorRankColors;

        public Color GetColor(ErrorRank rank)
        {
            var errorRankColor = errorRankColors.Find(x => x.Rank == rank);
            if (errorRankColor == null)
            {
                throw new NotImplementedException($"No color for rank {rank}!");
            }
            return errorRankColor.Color;
        }

        [System.Serializable]
        private class ErrorRankColor
        {
            [SerializeField] private ErrorRank rank;
            [SerializeField] private Color color;
            
            public ErrorRank Rank => rank;
            public Color Color => color;
        }
    }
}