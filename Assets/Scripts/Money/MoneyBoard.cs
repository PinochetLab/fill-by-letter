using System;
using Boards;
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
        [Inject] private ShopBoard _shopBoard;
        [Inject] private MoneyController _moneyController;

        public void UpdateMoney(int money)
        {
            moneyText.text = money.ToString();
        }
        
        public RectTransform Coin => coin;

        public void OnClick()
        {
            _shopBoard.ShowWithParam();
        }
    }
}