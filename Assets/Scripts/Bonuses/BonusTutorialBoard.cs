using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bonuses
{
    public class BonusTutorialBoard : MonoBehaviour
    {
        [SerializeField] private GameObject body;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text descriptionText;

        [SerializeField] private Image mainImage;
        [SerializeField] private Image buttonImage;

        private readonly HashSet<BonusTool> _tools = new();

        private AbstractBonusButton _bonusButton;

        private void Awake()
        {
            Close();
        }

        public void Close()
        {
            body.SetActive(false);
            Time.timeScale = 1;
        }

        public void Use()
        {
            _bonusButton.Do();
            Close();
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
                body.SetActive(true);
                
                mainImage.color = color;
                buttonImage.color = color;

                nameText.text = tool.ToolName;
                image.sprite = tool.Sprite;
                descriptionText.text = tool.Description;
            }
        }
    }
}