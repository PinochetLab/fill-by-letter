using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Boards;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Words;
using Zenject;

namespace Themes
{
    public class ThemeController : MonoBehaviour, IPointerClickHandler
    {
        [Header("Theme Settings")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image themeImage;
        [SerializeField] private TMP_Text multiplierText;
        [SerializeField] private TMP_Text effectText;
        
        [Inject] private ThemeTutorialBoard _themeTutorialBoard;

        private Theme _theme;
        
        private GlossaryLoader<List<string>> _loader;

        private List<string> _glossary;

        private Sequence _shakeSequence;

        private void Hide()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
        }

        private void Show()
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
        }

        private List<string> Glossary
        {
            get
            {
                _glossary ??= _loader.GetGlossary();
                return _glossary;
            }
        }

        public void ShowTutorial()
        {
            if (_theme is not null)
            {
                _themeTutorialBoard.ShowWithParam(_theme);
            }
        }

        public void SetTheme(Theme theme)
        {
            if (theme is null)
            {
                _theme = null;
                Hide();
                return;
            }
            
            effectText.gameObject.SetActive(false);
            
            Show();
            
            _theme = theme;
            
            themeImage.sprite = theme.Sprite;
            multiplierText.text = $"x{theme.Multiplier}";
            effectText.text = $"x{theme.Multiplier}";

            _glossary = null;
            _loader = new GlossaryLoader<List<string>>();
            _ = _loader.Load(theme.Glossary);
        }

        private void Impact()
        {
            _shakeSequence.Kill();
            
            _shakeSequence = DOTween.Sequence();

            _shakeSequence.Append(transform.DOShakePosition(
                duration: 0.6f, // Дольше = заметнее
                strength: 25f, // Огромная амплитуда
                vibrato: 30, // Очень много вибраций
                randomness: 100, // Максимальный хаос
                snapping: false,
                fadeOut: true
            ));
            
            effectText.gameObject.SetActive(true);
            effectText.rectTransform.anchoredPosition = Vector2.zero;
            effectText.rectTransform.localScale = Vector3.one;
            effectText.alpha = 1f;

            const float duration = 1f;

            _shakeSequence.Join(effectText.rectTransform.DOAnchorPos(Vector2.up * 300, duration));
            _shakeSequence.Join(effectText.rectTransform.DOScale(1.6f, duration));
            _shakeSequence.Join(effectText.DOFade(0f, duration).SetEase(Ease.InExpo));

            _shakeSequence.SetUpdate(true);
            _shakeSequence.Play();
        }

        public int GetScore(string word)
        {
            if (!_theme)
            {
                return word.Length;
            }
            if (Glossary.Contains(word))
            {
                Impact();
                return word.Length * _theme.Multiplier;
            }
            return word.Length;
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            _themeTutorialBoard.ShowWithParam(_theme);
        }
    }
}