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
        
        [Inject] private ThemeTutorialBoard _themeTutorialBoard;

        private Theme _theme;
        
        private GlossaryLoader<HashSet<string>> _loader;

        private HashSet<string> _glossary;

        private Tweener _shakeTweener;

        private void Awake()
        {
            Hide();
        }

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

        private HashSet<string> Glossary
        {
            get
            {
                _glossary ??= _loader.GetGlossary();
                return _glossary;
            }
        }

        public void SetTheme(Theme theme)
        {
            if (theme == null)
            {
                Hide();
                return;
            }
            
            Show();
            
            _themeTutorialBoard.ShowWithParam(theme);
            
            _theme = theme;
            
            themeImage.sprite = theme.Sprite;
            multiplierText.text = $"x{theme.Multiplier}";

            _glossary = null;
            _loader = new GlossaryLoader<HashSet<string>>();
            _ = _loader.Load(theme.Glossary);
        }

        private void Impact()
        {
            _shakeTweener.Kill();
            _shakeTweener = transform.DOShakePosition(0.5f, strength: 10f, vibrato: 10, randomness: 90, snapping: false, fadeOut: true);
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