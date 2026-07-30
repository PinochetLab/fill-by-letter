using System;
using Bonuses;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Boards
{
    public class BonusTutorialBoard : AbstractBoard
    {
        [Header("Tool Settings")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text descriptionText;

        protected override void ProcessParam(object param)
        {
            if (param is not BonusTool tool)
            {
                throw new NotImplementedException($"param={param} is not {nameof(BonusTool)}");
            }
            
            nameText.text = tool.ToolName;
            image.sprite = tool.BigSprite;
            descriptionText.text = tool.Description;
        }
    }
}