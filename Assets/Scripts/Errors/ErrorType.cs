using UnityEngine;

namespace Errors
{
    [CreateAssetMenu(fileName = "Error Type", menuName = "Error Type", order = 0)]
    public class ErrorType : ScriptableObject
    {
        [SerializeField] private ErrorRank rank;
        [SerializeField] private string text;
        
        public ErrorRank Rank => rank;
        public string Text => text;
    }
}