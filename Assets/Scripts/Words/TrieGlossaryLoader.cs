using System;
using JetBrains.Annotations;
using Tries;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Words
{
    public class TrieGlossaryLoader : MonoBehaviour
    {
        [SerializeField] private AssetReference trieJson;
        
        private readonly GlossaryLoader<Trie> _glossaryLoader = new ();

        private Trie _trie;
        private GameTrie _gameTrie;
        private SolverTrie _solverTrie;
        
        private async void Awake()
        {
            try
            {
                await _glossaryLoader.Load(trieJson);
            }
            catch (Exception e)
            {
                // TODO handle exception
            }
        }

        private void LoadTries()
        {
            _trie = new Trie();
            _trie = _glossaryLoader.GetGlossary();
            _gameTrie = _trie.GetGameTrie();
            _solverTrie = _trie.GetSolverTrie();
        }

        public Trie Trie
        {
            get
            {
                if (_trie == null)
                {
                    LoadTries();
                }
                return _trie;
            }
        }
        
        public GameTrie GameTrie
        {
            get
            {
                if (_trie == null)
                {
                    LoadTries();
                }
                return _gameTrie;
            }
        }
        
        public SolverTrie SolverTrie
        {
            get
            {
                if (_trie == null)
                {
                    LoadTries();
                }
                return _solverTrie;
            }
        }
    }
}