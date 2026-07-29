using System.Collections.Generic;
using Boards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bonuses
{
    public class BonusTutorialBoard : AbstractBoard
    {
        [Header("Tool Settings")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text descriptionText;

        private readonly HashSet<BonusTool> _tools = new();

        private BonusButton _bonusButton;

        public void Use()
        {
            _bonusButton.Do();
            Hide();
        }

        public void OpenIfNeeded(BonusButton bonusButton, BonusTool tool)
        {
            if (_tools.Contains(tool))
            {
                bonusButton.Do();
            }
            else
            {

                _bonusButton = bonusButton;
                _tools.Add(tool);

                nameText.text = tool.ToolName;
                image.sprite = tool.BigSprite;
                descriptionText.text = tool.Description;
                
                Show();
            }
        }
    }
}