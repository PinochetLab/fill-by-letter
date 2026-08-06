using System;
using System.Collections.Generic;
using DG.Tweening;
using Levels;
using Money;
using TMPro;
using Tutorials;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace Progress
{
    public class ProgressBoard : MonoBehaviour
    {
        [SerializeField] private List<MadeWordBlock> blocks;
        [SerializeField] private RectTransform wordsRt;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private GameObject emptyText;
        
        [SerializeField] private Slider scoreSlider;
        [SerializeField] private Slider firstScoreGoalSlider;
        [SerializeField] private Slider secondScoreGoalSlider;
        
        [SerializeField] private RectTransform firstScoreGoalChest;
        [SerializeField] private RectTransform secondScoreGoalChest;

        [SerializeField] private List<Color> letterColors;

        private readonly HashSet<string> _forbiddenWords = new ();

        private int _score;
        private int _firstScoreGoal;
        private int _secondScoreGoal;
        private int _letterColorIndex;
        private float _speed;

        private bool _firstScoreGoalReached;
        private bool _secondScoreGoalReached;

        private Sequence _sliderSequence;

        [Inject] private TreasureBoard _treasureBoard;
        [Inject] private TutorialBoard _tutorialBoard;

        private readonly List<MadeWordBlock> _madeWords = new();

        public void SetUp(Level level)
        {
            _firstScoreGoalReached = false;
            _secondScoreGoalReached = false;
            
            firstScoreGoalChest.gameObject.SetActive(true);
            secondScoreGoalChest.gameObject.SetActive(true);
            
            _firstScoreGoal = level.FirstScoreGoal;
            _secondScoreGoal = level.SecondScoreGoal;

            var maxValue = _secondScoreGoal * 1.3f;

            _speed = _secondScoreGoal;
            
            _sliderSequence.Kill();
            
            _forbiddenWords.Clear();
            _forbiddenWords.Add(level.Word);
            
            scoreSlider.value = 0;
            scoreSlider.maxValue = maxValue;

            _score = 0;
            UpdateScore();
            emptyText.SetActive(true);
            
            firstScoreGoalSlider.maxValue = maxValue;
            firstScoreGoalSlider.value = _firstScoreGoal;
            
            secondScoreGoalSlider.maxValue = maxValue;
            secondScoreGoalSlider.value = _secondScoreGoal;
            
            blocks.ForEach(b => b.gameObject.SetActive(false));
            _letterColorIndex = Random.Range(0, letterColors.Count);
        }

        private void AddScore(int score)
        {
            scoreText.text = _score.ToString();
            scoreSlider.value = _score;
            
            var newScore = _score + score;
            
            var delta = Mathf.Abs(newScore - _score);

            var duration = delta / _speed;
            
            _sliderSequence.Kill();

            _sliderSequence = DOTween.Sequence();

            _sliderSequence.Append(scoreSlider.DOValue(newScore, duration));
            if (_tutorialBoard.IsActive)
            {
                _sliderSequence.SetUpdate(false);
            }
            
            _score = newScore;
            UpdateScore();
        }

        private void UpdateScore()
        {
            scoreText.text = _score.ToString();
        }

        private void Update()
        {
            if (!_firstScoreGoalReached && scoreSlider.value >= _firstScoreGoal)
            {
                _firstScoreGoalReached = true;
                _treasureBoard.OpenChest(firstScoreGoalChest, Random.Range(8, 13));
                firstScoreGoalChest.gameObject.SetActive(false);
            }
            
            if (!_secondScoreGoalReached && scoreSlider.value >= _secondScoreGoal)
            {
                _secondScoreGoalReached = true;
                _treasureBoard.OpenChest(secondScoreGoalChest, Random.Range(18, 23));
                secondScoreGoalChest.gameObject.SetActive(false);
            }
        }

        public bool CanMake(string word)
        {
            return !_forbiddenWords.Contains(word);
        }

        public void MakeWord(string word, int score, int letterIndex)
        {
            emptyText.SetActive(false);
            _forbiddenWords.Add(word);
            AddScore(score);
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
        }
    }
}