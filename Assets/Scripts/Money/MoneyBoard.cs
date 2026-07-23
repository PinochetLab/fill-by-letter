using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace Money
{
    public class MoneyBoard : MonoBehaviour
    {
        [SerializeField] private RectTransform coin;
        [SerializeField] private TMP_Text moneyText;

        [Inject] private CoinTosser _coinTosser;

        private int _money = 175;

        private void Awake()
        {
            UpdateMoneyText();
        }
        
        public RectTransform Coin => coin;

        private void UpdateMoneyText()
        {
            moneyText.text = _money.ToString();
        }

        public void AddMoney(int amount)
        {
            _money += amount;
            UpdateMoneyText();
        }
    }
}