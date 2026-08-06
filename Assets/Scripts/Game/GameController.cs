using System;
using System.Collections.Generic;
using DG.Tweening;
using Grids;
using Levels;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private LocalizedAsset<LevelSet> levelSet;

        [Inject] private GridController _gridController;

        private int _levelIndex;
        private LevelSet _levelSet;

        public void GotToNextLevel()
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
            _gridController.SetLevel(_levelSet.Levels[_levelIndex]);
            _levelIndex++;
        }

        private void Awake()
        {
            Application.targetFrameRate = 100;
            _levelSet = levelSet.LoadAsset();
            GotToNextLevel();
        }
    }
}