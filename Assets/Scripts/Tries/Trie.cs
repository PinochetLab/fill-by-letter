using Newtonsoft.Json;
using UnityEngine;

namespace Tries
{
    public class Trie
    {
        [JsonProperty("R")]
        private TrieNode _root = new();

        public GameTrie GetGameTrie()
        {
            return new GameTrie(_root);
        }
        
        public SolverTrie GetSolverTrie()
        {
            return new SolverTrie(_root);
        }

        public void Clear()
        {
            _root = new TrieNode();
        }
        
        public void Insert(string word)
        {
            if (string.IsNullOrEmpty(word))
                return;

            var current = _root;
            
            foreach (char c in word)
            {
                if (!current.HasChild(c))
                {
                    current.AddChild(c, new TrieNode());
                }
                current = current.GetChild(c);
            }
            
            current.IsTerminal = true;
        }
    }
}