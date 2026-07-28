using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bonuses
{
    public class BonusTutorialBoard : MonoBehaviour
    {
        [SerializeField] private RectTransform body;
        [SerializeField] private RectTransform panel;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text descriptionText;

        [SerializeField] private Image mainImage;
        [SerializeField] private Image buttonImage;
        [SerializeField] private CanvasGroup backgroundCg;
        [SerializeField] private CanvasGroup panelCg;

        [SerializeField] private float showDuration = 0.5f;
        [SerializeField] private float hideDuration = 0.5f;

        private readonly HashSet<BonusTool> _tools = new();

        private AbstractBonusButton _bonusButton;
        private Sequence _fadeSequence;

        private Vector2 _panelPosition;

        private void Awake()
        {
            _panelPosition = panel.anchoredPosition;
            Close();
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
            body.gameObject.SetActive(false);
        }

        private void Show()
        {
            _fadeSequence.Kill();
            
            body.gameObject.SetActive(true);

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

        public void Use()
        {
            _bonusButton.Do();
            Hide();
        }

        public void OpenIfNeeded(AbstractBonusButton bonusButton, BonusTool tool, Color color)
        {
            if (_tools.Contains(tool))
            {
                bonusButton.Do();
            }
            else
            {

                Time.timeScale = 0;
                
                _bonusButton = bonusButton;
                _tools.Add(tool);
                
                mainImage.color = color;
                buttonImage.color = color;

                nameText.text = tool.ToolName;
                image.sprite = tool.Sprite;
                descriptionText.text = tool.Description;
                
                Show();
            }
        }
    }
}