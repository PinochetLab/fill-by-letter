using System;
using System.Collections.Generic;
using System.Linq;
using Grids;
using Keyboards;
using Progress;
using Tries;
using UnityEngine;
using UnityEngine.Localization;
using Words;
using Zenject;

namespace AIs
{
    public class Solver
    {
        [Inject] private TrieGlossaryLoader _trieGlossaryLoader;
        [Inject] private GridController _gridController;
        [Inject] private ProgressBoard _progressBoard;
        
        private SolverTrie _solverTrie;

        private string _alphabet;

        private SolverTrie Trie
        {
            get
            {
                _solverTrie ??= _trieGlossaryLoader.SolverTrie;
                return _solverTrie;
            }
        }

        [Inject]
        private void Init()
        {
            var alphabetEntry = new LocalizedAsset<Alphabet>
            {
                TableReference = "AlphabetTable",
                TableEntryReference = "Alphabet"
            };
            
            var handle = alphabetEntry.LoadAssetAsync();
            handle.WaitForCompletion();

            var alphabet = handle.Result;
            _alphabet = alphabet.AllLetters;
        }

        public Answer FindBestWord(char?[,] letters, List<Vector2Int> availablePositions)
        {
            List<Vector2Int> maxPath = null;
            char? answerLetter = null;
            Vector2Int? answerLetterPos = null;

            var directions = new Vector2Int[letters.GetLength(0), letters.GetLength(1)];
            for (var i = 0; i < letters.GetLength(0); i++)
            {
                for (var j = 0; j < letters.GetLength(1); j++)
                {
                    directions[i, j] = Vector2Int.zero;
                }
            }

            foreach (var position in availablePositions)
            {
                foreach (var letter in _alphabet)
                {
                    letters[position.x, position.y] = letter;

                    var path = FindBestPathWithNewLetter(letters, directions, position, letter == 'ы' && position == new Vector2Int(8, 5));

                    if (maxPath is null || (path is not null && path.Count > maxPath.Count))
                    {
                        maxPath = path;
                        answerLetter = letter;
                        answerLetterPos = position;
                    }

                    letters[position.x, position.y] = null;
                }
            }

            if (maxPath is null || answerLetter is null || answerLetterPos is null)
            {
                throw new NotImplementedException("No words are found!");
            }

            letters[answerLetterPos.Value.x, answerLetterPos.Value.y] = answerLetter.Value;
            
            var word = new string(maxPath.Select(v => letters[v.x, v.y].Value).ToArray());

            return new Answer(answerLetterPos.Value, answerLetter.Value, word, maxPath);
        }
        
        public List<Vector2Int> FindBestPathWithNewLetter(char?[,] letters, Vector2Int[,] directions, Vector2Int newLetterPos, bool print)
        {
            List<Vector2Int> maxPath = null;
            
            for (var i = 0; i < letters.GetLength(0); i++)
            {
                for (var j = 0; j < letters.GetLength(1); j++)
                {
                    var letter = letters[i, j];

                    if (letter == null)
                    {
                        continue;
                    }

                    var startPos = new Vector2Int(i, j);

                    var p = print && letter == 'к' && startPos == new Vector2Int(3, 5);
                    
                    var path = FindBestPathWithNewLetter(letters, directions, startPos, newLetterPos, startPos, p);

                    if (maxPath is null || (path is not null && path.Count > maxPath.Count))
                    {
                        maxPath = path;
                    }
                }
            }
            
            return maxPath;
        }

        private List<Vector2Int> FindBestPathWithNewLetter
        (
            char?[,] letters,
            Vector2Int[, ] directions,
            Vector2Int startPos,
            Vector2Int newLetterPos,
            Vector2Int current,
            bool print,
            int depth = 0)
        {
            var letter = letters[current.x, current.y].Value;

            if (!Trie.HasLetter(letter))
            {
                return null;
            }

            List<Vector2Int> maxPath = null;
            
            Trie.AddLetter(letter);

            foreach (var d in _gridController.GetDirections(current))
            {
                var v = current + d;
                
                if (letters[v.x, v.y] == null)
                {
                    continue;
                }
                
                if (directions[v.x, v.y] != Vector2.zero)
                {
                    continue;
                }
                
                directions[current.x, current.y] = d;

                var path = FindBestPathWithNewLetter(letters, directions, startPos, newLetterPos, v, print, depth + 1);
                if (maxPath is null || (path is not null && path.Count > maxPath.Count))
                {
                    maxPath = path;
                }
            }

            if (maxPath is null && Trie.IsTerminal() && (directions[newLetterPos.x, newLetterPos.y] != Vector2.zero || current == newLetterPos))
            {
                var word = Trie.GetWord();
                if (_progressBoard.CanMake(word))
                {
                    maxPath = new List<Vector2Int>();
                    var c = startPos;
                    maxPath.Add(c);
                    while (c != current)
                    {
                        c += directions[c.x, c.y];
                        maxPath.Add(c);
                    }
                }
            }

            directions[current.x, current.y] = Vector2Int.zero;
            
            Trie.RemoveLetter();

            return maxPath;
        }
    }
}