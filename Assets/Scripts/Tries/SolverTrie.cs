using System.Collections.Generic;

namespace Tries
{
    public class SolverTrie
    {
        private readonly TrieNode _root;

        private TrieNode _current;
        
        private readonly List<TrieNode> _path = new ();

        private readonly List<char> _letters = new ();
        
        public SolverTrie(TrieNode root)
        {
            _root = root;
            _current = _root;
        }
        
        public void Reset()
        {
            _current = _root;
            _path.Clear();
            _letters.Clear();
        }

        public bool HasLetter(char letter)
        {
            return _current.HasChild(letter);
        }

        public void AddLetter(char letter)
        {
            _path.Add(_current);
            _letters.Add(letter);
            _current = _current.GetChild(letter);
        }

        public void RemoveLetter()
        {
            _current = _path[^1];
            _path.RemoveAt(_path.Count - 1);
            _letters.RemoveAt(_letters.Count - 1);
        }

        public bool IsTerminal()
        {
            return _current.IsTerminal;
        }

        public string GetWord()
        {
            return new string(_letters.ToArray());
        }
    }
}