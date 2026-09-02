using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

namespace Money
{
    public class Reward : MonoBehaviour
    {
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private RectTransform coin;
        [SerializeField] private RectTransform rt;

        [Inject] private CoinTosser _coinTosser;
        [Inject] private MoneyBoard _moneyBoard;
        [Inject] private MoneyController _moneyController;

        private int _money;

        public void SetUp(int money, Vector2 coinSize)
        {
            rt.sizeDelta = coinSize;
            _money = money;
            moneyText.text = money.ToString();
        }

        public Tween Move(Vector3 startPosition)
        {
            var endPosition = startPosition + Vector3.up * 200;
            var distance = Vector3.Distance(startPosition, endPosition);
            var duration = distance / 500f;
            
            transform.position = startPosition;
            
            var sequence = DOTween.Sequence();
            
            sequence.Append(transform.DOMove(endPosition, duration).SetEase(Ease.OutSine));
            sequence.SetUpdate(true);
            
            sequence.AppendCallback(EndMove);
            
            return sequence;
        }

        private void EndMove()
        {
            _coinTosser.TossCoin(coin, _moneyBoard.Coin).OnComplete(() => _moneyController.AddMoney(_money));
        }
    }
}