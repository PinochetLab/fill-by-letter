using System;
using Themes;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Boards
{
    public class ThemeTutorialBoard : AbstractBoard
    {
        [Header("Theme Settings")]
        [SerializeField] private TMP_Text themeNameText;
        [SerializeField] private Image themeImage;
        [SerializeField] private TMP_Text descriptionText;
        
        [SerializeField] private LocalizedString themeDescription;

        private string _themeDescription;

        private void Awake()
        {
            _themeDescription = themeDescription.GetLocalizedString();
        }

        protected override void ProcessParam(object param)
        {
            if (param is not Theme theme)
            {
                throw new NotImplementedException($"param={param} is not {nameof(Theme)}");
            }
            
            themeNameText.text = theme.ThemeName;
            themeImage.sprite = theme.Sprite;
            descriptionText.text = string.Format(_themeDescription, theme.Multiplier);
        }
    }
}