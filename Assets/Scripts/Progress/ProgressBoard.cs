using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Progress
{
    public class ProgressBoard : MonoBehaviour
    {
        [SerializeField] private List<MadeWordBlock> blocks;
        [SerializeField] private RectTransform wordsRt;
        [SerializeField] private TMP_Text scoreText;

        private HashSet<string> _forbiddenWords = new ();

        private int _score;

        private readonly List<MadeWordBlock> _madeWords = new();

        private void Awake()
        {
            blocks.ForEach(b => b.gameObject.SetActive(false));
        }

        private void Start()
        {
            UpdateScoreText();
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        private void UpdateScoreText()
        {
            scoreText.text = _score.ToString();
        }

        public void AddStartWord(string word)
        {
            _forbiddenWords.Add(word);
        }

        public bool CanMake(string word)
        {
            return !_forbiddenWords.Contains(word);
        }

        public void MakeWord(string word, int letterIndex)
        {
            _forbiddenWords.Add(word);
            _score += word.Length;
            UpdateScoreText();
            MadeWordBlock block;
            if (_madeWords.Count < blocks.Count)
            {
                block = blocks[_madeWords.Count];
                _madeWords.Add(block);
            }
            else
            {
                block = _madeWords[0];
                _madeWords.RemoveAt(0);
                _madeWords.Add(block);
                block.transform.SetAsLastSibling();
            }
            block.gameObject.SetActive(true);
            block.SetWord(word, letterIndex);
            LayoutRebuilder.ForceRebuildLayoutImmediate(wordsRt);
        }
    }
}