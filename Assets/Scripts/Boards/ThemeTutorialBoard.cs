using System;
using Themes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Boards
{
    public class ThemeTutorialBoard : AbstractBoard
    {
        [Header("Theme Settings")]
        [SerializeField] private string description = "Собирай слова по теме и получай <color=red>x{0}</color> очков!";

        [SerializeField] private TMP_Text themeNameText;
        [SerializeField] private Image themeImage;
        [SerializeField] private TMP_Text descriptionText;

        protected override void ProcessParam(object param)
        {
            if (param is not Theme theme)
            {
                throw new NotImplementedException($"param={param} is not {nameof(Theme)}");
            }
            
            themeNameText.text = theme.ThemeName;
            themeImage.sprite = theme.Sprite;
            descriptionText.text = string.Format(description, theme.Multiplier);
        }
    }
}