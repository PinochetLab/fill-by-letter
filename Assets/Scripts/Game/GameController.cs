using System;
using System.Collections.Generic;
using DG.Tweening;
using Grids;
using Levels;
using UnityEngine;
using Zenject;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private List<Level> levels;

        [Inject] private GridController _gridController;

        private int _levelIndex;

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
            _gridController.SetLevel(levels[_levelIndex]);
            _levelIndex++;
        }

        private void Awake()
        {
            Application.targetFrameRate = 300;
            GotToNextLevel();
        }
    }
}