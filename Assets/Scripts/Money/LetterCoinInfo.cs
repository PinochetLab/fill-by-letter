using UnityEngine;

namespace Money
{
    [System.Serializable]
    public class LetterCoinInfo
    {
        [SerializeField] private Vector2Int position;
        [SerializeField] private char letter;

        public Vector2Int Position => position;
        public char Letter => letter;
    }
}