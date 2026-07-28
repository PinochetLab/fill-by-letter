using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Themes
{
    public class ThemeTutorialBoard : MonoBehaviour
    {
        [SerializeField] private string description = "Собирай слова по теме и получай <color=red>x{0}</color> очков!";

        [SerializeField] private GameObject body;
        [SerializeField] private RectTransform panel;
        [SerializeField] private TMP_Text themeNameText;
        [SerializeField] private Image themeImage;
        [SerializeField] private TMP_Text descriptionText;
        
        [SerializeField] private CanvasGroup backgroundCg;
        [SerializeField] private CanvasGroup panelCg;
        
        [SerializeField] private float showDuration = 0.5f;
        [SerializeField] private float hideDuration = 0.5f;
        
        private Sequence _fadeSequence;

        private Vector2 _panelPosition;

        private void Awake()
        {
            _panelPosition = panel.anchoredPosition;
            Close();
        }

        public void Show(Theme theme)
        {
            body.SetActive(true);
            themeNameText.text = theme.ThemeName;
            themeImage.sprite = theme.Sprite;
            descriptionText.text = string.Format(description, theme.Multiplier);
            
            _fadeSequence.Kill();

            backgroundCg.alpha = 0;
            panelCg.alpha = 0;

            var startPosition = _panelPosition + Vector2.down * 1000;
            //panel.anchoredPosition = startPosition;
            panel.localScale = Vector3.zero;
            
            _fadeSequence = DOTween.Sequence();
            
            _fadeSequence.Append(backgroundCg.DOFade(1, showDuration));
            _fadeSequence.Join(panelCg.DOFade(1, showDuration));
            //_fadeSequence.Join(panel.DOAnchorPos(_panelPosition, showDuration));
            _fadeSequence.Join(panel.DOScale(1, showDuration));
            _fadeSequence.SetUpdate(true);
            _fadeSequence.Play();
        }

        public void Hide()
        {
            _fadeSequence.Kill();
            
            backgroundCg.alpha = 1;
            panelCg.alpha = 1;
            
            var endPosition = _panelPosition + Vector2.down * 1000;
            //panel.anchoredPosition = _panelPosition;
            panel.localScale = Vector3.one;
            
            _fadeSequence = DOTween.Sequence();
            
            _fadeSequence.Append(backgroundCg.DOFade(0, showDuration));
            _fadeSequence.Join(panelCg.DOFade(0, showDuration));
            //_fadeSequence.Join(panel.DOAnchorPos(endPosition, showDuration));
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