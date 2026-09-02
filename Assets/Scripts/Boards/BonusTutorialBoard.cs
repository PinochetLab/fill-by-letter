using System;
using Bonuses;
using Money;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yandex;
using YG;
using Zenject;

namespace Boards
{
    public class BonusTutorialBoard : AbstractBoard
    {
        [Header("Tool Settings")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text descriptionText;
        
        [SerializeField] private GameObject moneyContent;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private GameObject adsContent;

        [Inject] private MoneyController _moneyController;
        [Inject] private AdsController _adsController;

        private BonusButton _button;
        private BonusTool _tool;
        private bool _isForMoney;

        protected override void ProcessParam(object param)
        {
            if (param is not BonusButton button)
            {
                throw new NotImplementedException($"param={param} is not {nameof(BonusTool)}");
            }
            
            _button = button;

            _tool = button.BonusTool;
            
            nameText.text = _tool.ToolName;
            image.sprite = _tool.BigSprite;
            descriptionText.text = _tool.Description;

            _isForMoney = _moneyController.EnoughMoney(_tool.Price);

            if (_isForMoney)
            {
                adsContent.SetActive(false);
                moneyContent.SetActive(true);
                priceText.text = _tool.Price.ToString();
            }
            else
            {
                adsContent.SetActive(true);
                moneyContent.SetActive(false);
            }
        }

        public void Click()
        {
            if (_isForMoney)
            {
                _moneyController.SpendMoney(_tool.Price);
                Use();
            }
            else
            {
                _adsController.ShowRewarded("bonus", Use); 
            };
        }

        private void Use()
        {
            _button.SwitchOn();
            Close();
        }
    }
}