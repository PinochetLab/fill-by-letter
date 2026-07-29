using DG.Tweening;
using UnityEngine;

namespace Boards
{
    public abstract class AbstractBoard : MonoBehaviour
    {
        [Header("Appear Settings")]
        [SerializeField] private GameObject body;
        [SerializeField] private Transform panel;
        [SerializeField] private CanvasGroup backgroundCg;
        [SerializeField] private CanvasGroup panelCg;
        [SerializeField] private float showDuration = 0.5f;
        [SerializeField] private float hideDuration = 0.5f;
        
        private Sequence _fadeSequence;
        
        private void Awake()
        {
            Close();
        }

        public void Show()
        {
            body.SetActive(true);
            
            _fadeSequence.Kill();
            
            Time.timeScale = 0;

            backgroundCg.alpha = 0;
            panelCg.alpha = 0;

            panel.localScale = Vector3.zero;
            
            _fadeSequence = DOTween.Sequence();
            
            _fadeSequence.Append(backgroundCg.DOFade(1, showDuration));
            _fadeSequence.Join(panelCg.DOFade(1, showDuration));
            _fadeSequence.Join(panel.DOScale(1, showDuration));
            _fadeSequence.SetUpdate(true);
            _fadeSequence.Play();
        }

        public void Hide()
        {
            _fadeSequence.Kill();
            
            backgroundCg.alpha = 1;
            panelCg.alpha = 1;
            
            panel.localScale = Vector3.one;
            
            _fadeSequence = DOTween.Sequence();
            
            _fadeSequence.Append(backgroundCg.DOFade(0, showDuration));
            _fadeSequence.Join(panelCg.DOFade(0, showDuration));
            _fadeSequence.Join(panel.DOScale(0, showDuration));
            _fadeSequence.OnComplete(Close);
            _fadeSequence.SetUpdate(true);
            _fadeSequence.Play();
        }
        
        private void Close()
        {
            Time.timeScale = 1;
            body.SetActive(false);
        }
    }
}