using System;
using UnityEngine;
using Yandex;
using Zenject;

namespace Money
{
    public class MoneyController : MonoBehaviour
    {
        private int _money = 500;
        
        [Inject] private MoneyBoard _moneyBoard;
        [Inject] private DataController _dataController;

        private void UpdateMoney()
        {
            _moneyBoard.UpdateMoney(_money);
        }

        public void SetUp(int money)
        {
            _money = money;
            UpdateMoney();
        }

        public void AddMoney(int amount)
        {
            _money += amount;
            _dataController.SaveMoney(_money);
            UpdateMoney();
        }

        public bool EnoughMoney(int amount)
        {
            return _money > amount;
        }

        public void SpendMoney(int amount)
        {
            _money -= amount;
            UpdateMoney();
        }
    }
}