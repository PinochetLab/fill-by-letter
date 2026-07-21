using System.Collections.Generic;
using UnityEngine;

namespace AIs
{
    public class Answer
    {
        public Vector2Int LetterPos { get; private set; }
        public char Letter { get; private set; }
        public string Word { get; private set; }
        public List<Vector2Int> Path { get; private set; }

        public Answer(Vector2Int letterPos, char letter, string word, List<Vector2Int> path)
        {
            LetterPos = letterPos;
            Letter = letter;
            Word = word;
            Path = path;
        }
    }
}