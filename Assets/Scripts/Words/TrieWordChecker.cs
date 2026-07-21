using System;
using Tries;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Words
{
    public class TrieWordChecker : MonoBehaviour
    {
        [SerializeField] private AssetReference trieJson;
        
        [Inject] private TrieGlossaryLoader _trieGlossaryLoader;
        
        private GameTrie _gameTrie;

        private GameTrie GameTrie => _trieGlossaryLoader.GameTrie;

        public void Reset()
        {
            GameTrie.Reset();
        }

        public void AddLetter(char letter)
        {
            GameTrie.AddLetter(letter);
        }
        
        public void RemoveLetter()
        {
            GameTrie.RemoveLetter();
        }

        public bool IsWordCorrect()
        {
            return GameTrie.IsInTerminalNode();
        }
    }
}