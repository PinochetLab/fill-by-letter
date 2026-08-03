using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Boards
{
    public abstract class AbstractBoard : MonoBehaviour
    {
        [Header("Appear Settings")]
        [SerializeField] private GameObject screen;
        [SerializeField] private Transform board;
        [SerializeField] private CanvasGroup boardCg;
        [SerializeField] private CanvasGroup backCg;
        [SerializeField] private float duration = 0.5f;

        private static readonly Queue<AbstractBoard> BoardQueue = new ();
        private static readonly Queue<object> ParamQueue = new ();

        private bool _isShown;
        
        
        
        private Sequence _fadeSequence;
        
        private void Awake()
        {
            EndHiding();
        }

        public void ShowWithParam(object param = null)
        {
            //Debug.Log($"Showing param: {param} for {this}");
            ParamQueue.Enqueue(param);
            BoardQueue.Enqueue(this);
        }

        private void Update()
        {
            if (!_isShown && BoardQueue.Count > 0 && BoardQueue.Peek() == this)
            {
                //Debug.Log($"this: {this}, BoardQueue.Peek(): {BoardQueue.Peek()}, ParamQueue.Peek(): {ParamQueue.Peek()}");
                var param =  ParamQueue.Dequeue();
                _isShown = true;
                ProcessParam(param);
                Show();
            }
        }

        protected abstract void ProcessParam(object param);
        
        private void Show()
        {
            screen.SetActive(true);
            backCg.blocksRaycasts = true;
            boardCg.interactable = true;
            
            _fadeSequence.Kill();
            
            Time.timeScale = 0;

            backCg.alpha = 0;
            boardCg.alpha = 0;

            board.localScale = Vector3.zero;
            
            _fadeSequence = DOTween.Sequence();
            
            _fadeSequence.Append(board.DOScale(1, duration));
            _fadeSequence.Join(backCg.DOFade(1, duration));
            _fadeSequence.Join(boardCg.DOFade(1, duration));
            _fadeSequence.SetUpdate(true);
            _fadeSequence.Play();
        }

        protected void Hide()
        {
            boardCg.interactable = false;
            backCg.blocksRaycasts = false;
            
            _fadeSequence.Kill();
            
            backCg.alpha = 1;
            boardCg.alpha = 1;
            
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
            _isShown = false;
            if (BoardQueue.Count > 0)
            {
                BoardQueue.Dequeue();
            }
            Time.timeScale = 1;
            screen.SetActive(false);
        }
    }
}