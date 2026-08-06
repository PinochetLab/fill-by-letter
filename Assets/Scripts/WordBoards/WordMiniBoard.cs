using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace WordBoards
{
    public class WordMiniBoard : MonoBehaviour
    {
        [SerializeField] private WordBlock mainBlock;
        [SerializeField] private List<WordBlock> flyBlocks;
        [SerializeField] private RectTransform scoreBlock;

        private string _word;

        public void Reset()
        {
            _word = string.Empty;
            Clear();
        }

        private void Awake()
        {
            Clear();
        }

        public void AddLetter(char letter)
        {
            _word += letter;
            mainBlock.gameObject.SetActive(true);
            mainBlock.SetWord(_word);
        }
        
        public void RemoveLetter()
        {
            _word = _word[..^1];
            mainBlock.SetWord(_word);
            if (_word.Length == 0)
            {
                Clear();
            }
        }

        public void Send()
        {
            const float duration = 0.5f;
            
            var flyBlock = flyBlocks.Find(x => !x.gameObject.activeSelf);
            
            flyBlock.SetWord(_word);
            
            flyBlock.ResetTransform();
            
            flyBlock.gameObject.SetActive(true);
            
            var sequence = DOTween.Sequence();
            sequence.Append(flyBlock.transform.DOMove(scoreBlock.position, duration));
            sequence.Join(flyBlock.transform.DOScale(0, duration));
            sequence.OnComplete(() => flyBlock.gameObject.SetActive(false));
            sequence.SetUpdate(true);
            sequence.Play();
        }

        public void Clear()
        {
            _word = string.Empty;
            mainBlock.gameObject.SetActive(false);
        }
    }
}