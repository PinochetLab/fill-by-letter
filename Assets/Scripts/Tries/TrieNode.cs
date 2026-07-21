using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tries
{
    public class TrieNode
    {
        [JsonProperty("C")]
        private readonly Dictionary<char, TrieNode> _children = new();
        
        [JsonProperty("T")]
        public bool IsTerminal { get; set; }

        [JsonIgnore]
        public IReadOnlyDictionary<char, TrieNode> Children => _children;
        
        public TrieNode GetChild(char c)
        {
            _children.TryGetValue(c, out var child);
            return child;
        }
        
        public bool HasChild(char c)
        {
            return _children.ContainsKey(c);
        }
        
        public void AddChild(char c, TrieNode node)
        {
            _children[c] = node;
        }
        
        [JsonIgnore]
        public bool IsLeaf => _children.Count == 0;
    }
}