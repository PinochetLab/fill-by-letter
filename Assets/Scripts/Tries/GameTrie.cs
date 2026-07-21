using System.Collections.Generic;
using UnityEngine;

namespace Tries
{
    public class GameTrie
    {
        private readonly TrieNode _root;

        private TrieNode _current;

        private readonly List<TrieNode> _path = new ();

        private int _excessLetterCount;

        public GameTrie(TrieNode root)
        {
            _root = root;
            _current = _root;
        }

        public void Reset()
        {
            _excessLetterCount = 0;
            _current = _root;
            _path.Clear();
        }

        public void AddLetter(char letter)
        {
            if (_excessLetterCount > 0 || _current.IsLeaf || !_current.HasChild(letter))
            {
                _excessLetterCount++;
                return;
            }

            _path.Add(_current);
            _current = _current.GetChild(letter);
        }

        public void RemoveLetter()
        {
            if (_excessLetterCount > 0)
            {
                _excessLetterCount--;
                return;
            }
            
            _current = _path[^1];
            _path.RemoveAt(_path.Count - 1);
        }

        public bool IsInTerminalNode()
        {
            return _current.IsTerminal && _excessLetterCount == 0;
        }
    }
}