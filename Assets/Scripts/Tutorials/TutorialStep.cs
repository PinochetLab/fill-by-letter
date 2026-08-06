using System.Collections.Generic;
using UnityEngine;

namespace Tutorials
{
	[System.Serializable]
    public class TutorialStep
    {
        [SerializeField] private Vector2Int letterCell;
        [SerializeField] private char letter;
        [SerializeField] private List<Vector2Int> path;
        [SerializeField] private bool oneLine;
        [SerializeField] private bool cancel;
        
        public Vector2Int LetterCell => letterCell;
        public char Letter => letter;
        public List<Vector2Int> Path => path;
        public bool OneLine => oneLine;
        public bool Cancel => cancel;
    }
}