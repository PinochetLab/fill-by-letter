using System.Collections.Generic;
using Levels;
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
        
        [SerializeField] private Slider scoreSlider;
        [SerializeField] private Slider firstScoreGoalSlider;
        [SerializeField] private Slider secondScoreGoalSlider;
        [SerializeField] private TMP_Text maxScoreText;

        [SerializeField] private List<Color> letterColors;

        private readonly HashSet<string> _forbiddenWords = new ();

        private int _score;
        private int _firstScoreGoal;
        private int _secondScoreGoal;
        private int _letterColorIndex;

        private readonly List<MadeWordBlock> _madeWords = new();

        public void SetUp(Level level)
        {
            _firstScoreGoal = level.FirstScoreGoal;
            _secondScoreGoal = level.SecondScoreGoal;

            maxScoreText.text = _secondScoreGoal.ToString();
            
            scoreSlider.maxValue = _firstScoreGoal;

            _score = 0;
            UpdateScore();
            
            firstScoreGoalSlider.maxValue = _secondScoreGoal;
            firstScoreGoalSlider.value = _firstScoreGoal;
            
            secondScoreGoalSlider.maxValue = _secondScoreGoal;
            secondScoreGoalSlider.value = _secondScoreGoal;
            
            blocks.ForEach(b => b.gameObject.SetActive(false));
            _letterColorIndex = Random.Range(0, letterColors.Count);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        private void UpdateScore()
        {
            scoreText.text = _score.ToString();
            scoreSlider.value = _score;
        }

        public void AddStartWord(string word)
        {
            _forbiddenWords.Add(word);
        }

        public bool CanMake(string word)
        {
            return !_forbiddenWords.Contains(word);
        }

        public void MakeWord(string word, int score, int letterIndex)
        {
            _forbiddenWords.Add(word);
            _score += score;
            UpdateScore();
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
            var letterColor = letterColors[_letterColorIndex];
            _letterColorIndex = (_letterColorIndex + 1) % letterColors.Count;
            block.gameObject.SetActive(true);
            block.SetWord(word, letterIndex, letterColor);
            LayoutRebuilder.ForceRebuildLayoutImmediate(wordsRt);
        }
    }
}