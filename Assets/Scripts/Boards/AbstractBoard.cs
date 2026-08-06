using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Boards
{
    public abstract class AbstractBoard : MonoBehaviour
    {
        [Header("Appear Settings")]
        [SerializeField] private GameObject screen;
        [SerializeField] private RectTransform board;
        [SerializeField] private CanvasGroup boardCg;
        [SerializeField] private CanvasGroup backCg;
        [SerializeField] private float duration = 0.5f;
        
        [Inject] private BoardManager _boardManager;
        
        protected RectTransform Board => board;

        private void Awake()
        {
            backCg.alpha = 0;
            boardCg.alpha = 0;
        }

        private Sequence _fadeSequence;

        public void ShowWithParam(object param = null, bool blackScreen = true)
        {
            _boardManager.Show(this, param, blackScreen);
        }

        public void Open(object param, bool blackScreen)
        {
            ProcessParam(param);
            Show(blackScreen);
        }

        protected abstract void ProcessParam(object param);
        
        private void Show(bool blackScreen)
        {
            //LayoutRebuilder.ForceRebuildLayoutImmediate(board);
            
            screen.SetActive(true);
            backCg.blocksRaycasts = true;
            boardCg.interactable = true;
            
            _fadeSequence.Kill();
            
            Time.timeScale = 0;

            board.localScale = Vector3.zero;

            _fadeSequence = DOTween.Sequence();
            
            _fadeSequence.Append(board.DOScale(1, duration));
            if (blackScreen)
            {
                _fadeSequence.Join(backCg.DOFade(1, duration));
                _fadeSequence.Join(boardCg.DOFade(1, duration));
            }
            _fadeSequence.SetUpdate(true);
            _fadeSequence.Play();
        }

        protected void Hide()
        {
            boardCg.interactable = false;
            backCg.blocksRaycasts = false;
            
            _fadeSequence.Kill();
            
            board.localScale = Vector3.one;
            
            _fadeSequence = DOTween.Sequence();
            
            _fadeSequence.Append(board.DOScale(0, duration));
            _fadeSequence.Join(backCg.DOFade(0, duration));
            _fadeSequence.Join(boardCg.DOFade(0, duration));
            _fadeSequence.OnComplete(EndHiding);
            _fadeSequence.SetUpdate(true);
            _fadeSequence.Play();
        }

        public void Close()
        {
            Hide();
        }
        
        private void EndHiding()
        {
            _boardManager.Dequeue();
            Time.timeScale = 1;
            screen.SetActive(false);
            board.localScale = Vector3.one;
        }
    }
}