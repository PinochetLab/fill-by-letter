using System;
using System.Collections.Generic;
using DG.Tweening;
using Grids;
using Keyboards;
using Levels;
using Money;
using UnityEngine;
using UnityEngine.Localization;
using Words;
using Yandex;
using Zenject;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private LocalizedAsset<LevelSet> levelSet;

        [Inject] private GridController _gridController;
        [Inject] private LetterKeyboard _letterKeyboard;
        [Inject] private TrieGlossaryLoader _trieGlossaryLoader;
        [Inject] private LanguageController _languageController;
        [Inject] private DataController _dataController;
        [Inject] private MoneyController _moneyController;

        private int _levelIndex;
        private LevelSet _levelSet;

        public void GoToNextLevel()
        {
            if (_levelIndex == 0)
            {
                GoTo();
            }
            else
            {
                var sequence = _gridController.Disappear();
                sequence.OnComplete(GoTo);
                sequence.Play();
            }
        }

        private void GoTo()
        {
            var level = _levelSet.Levels[_levelIndex];
            _gridController.SetLevel(_levelSet.Levels[_levelIndex], null);
            _dataController.SaveLevel(level);
            _dataController.SaveLevelIndex(_levelIndex);
            _dataController.SaveLevelProgress(null);
            _levelIndex++;
        }

        private void GoToLevel(int levelIndex, LevelProgress levelProgress)
        {
            _levelIndex = levelIndex;
            _gridController.SetLevel(_levelSet.Levels[_levelIndex], levelProgress);
            _levelIndex++;
        }

        private void Awake()
        {
            Application.targetFrameRate = 100;
            StartGame();
        }

        private void StartGame()
        {
            _languageController.SetUp();
            
            _letterKeyboard.SetUp();
            _trieGlossaryLoader.Load();
            
            _levelSet = levelSet.LoadAsset();

            var money = _dataController.GetMoney();
            
            _moneyController.SetUp(money);

            var level = _dataController.GetLevel();
            var levelIndex = _dataController.GetLevelIndex();

            if (level is null)
            {
                level = _levelSet.Levels[0];
                _dataController.SaveLevel(level);
                _dataController.SaveLevelProgress(null);
            }
            else
            {
                if (!_levelSet.Levels.Contains(level))
                {
                    levelIndex = _dataController.GetLevelIndex();

                    if (levelIndex < _levelSet.Levels.Count)
                    {
                        level = _levelSet.Levels[levelIndex];
                        _dataController.SaveLevel(level);
                        _dataController.SaveLevelProgress(null);
                    }
                    else
                    {
                        _levelIndex = 0;
                        _dataController.SaveLevelIndex(0);
                        level = _levelSet.Levels[0];
                        _dataController.SaveLevel(level);
                        _dataController.SaveLevelProgress(null);
                    }
                }
                else
                {
                    levelIndex = _levelSet.Levels.IndexOf(level);
                    _dataController.SaveLevelIndex(levelIndex);
                }
            }
            
            GoToLevel(levelIndex, _dataController.GetLevelProgress());
        }
    }
}