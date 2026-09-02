using System;
using JetBrains.Annotations;
using Tries;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Words
{
    public class TrieGlossaryLoader : MonoBehaviour
    {
        [SerializeField] private LocalizedAsset<TextAsset> glossary;
        
        private readonly GlossaryLoader<Trie> _glossaryLoader = new ();

        private Trie _trie;
        private GameTrie _gameTrie;
        private SolverTrie _solverTrie;

        public void Load()
        {
            LoadGlossary();
        }
        
        private async void LoadGlossary()
        {
            try
            {
                AsyncOperationHandle<TextAsset> handle = glossary.LoadAssetAsync();
                await handle.Task;

                TextAsset asset = handle.Result;
                
                await _glossaryLoader.Load(asset);
                //TextAsset glossaryText = await glossary.LoadAssetAsync().Task;
                //await _glossaryLoader.Load(glossary.LoadAsset());
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