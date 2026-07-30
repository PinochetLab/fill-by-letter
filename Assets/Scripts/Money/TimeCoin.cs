using UnityEngine;

namespace Money
{
    [System.Serializable]
    public class TimeCoin
    {
        [SerializeField] private Vector2Int position;
        [SerializeField] private float duration;

        public Vector2Int Position => position;
        public float Duration => duration;
    }
}