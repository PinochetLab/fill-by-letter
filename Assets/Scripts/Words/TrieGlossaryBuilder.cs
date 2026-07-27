#if UNITY_EDITOR

using Tries;

namespace Words
{
    public class TrieGlossaryBuilder : AbstractGlossaryBuilder
    {
        private readonly Trie _trie = new();
        
        protected override string FolderName => "Trie";

        protected override void Init()
        {
            _trie.Clear();
        }

        protected override object Object => _trie;
        
        protected override void AddWord(string word)
        {
            _trie.Insert(word);
        }
    }
}

#endif