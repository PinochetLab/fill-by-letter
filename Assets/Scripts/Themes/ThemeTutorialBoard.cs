using Boards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Themes
{
    public class ThemeTutorialBoard : AbstractBoard
    {
        [Header("Theme Settings")]
        [SerializeField] private string description = "Собирай слова по теме и получай <color=red>x{0}</color> очков!";

        [SerializeField] private TMP_Text themeNameText;
        [SerializeField] private Image themeImage;
        [SerializeField] private TMP_Text descriptionText;

        public void SetTheme(Theme theme)
        {
            themeNameText.text = theme.ThemeName;
            themeImage.sprite = theme.Sprite;
            descriptionText.text = string.Format(description, theme.Multiplier);
        }
    }
}