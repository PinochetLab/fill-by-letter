using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Zenject;

namespace Money
{
    public class TreasureBoard : MonoBehaviour
    {
        [SerializeField] private GameObject body;
        [SerializeField] private RectTransform chestBody;
        [SerializeField] private RectTransform chest;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private int openClickCount = 3;

        [SerializeField] private RectTransform chestBottom;
        [SerializeField] private RectTransform chestTop;
        [SerializeField] private RectTransform coinsRoot;
        [SerializeField] private List<RectTransform> coins;
        
        [SerializeField] private CanvasGroup hintCanvasGroup;
        [SerializeField] private CanvasGroup topChestCg;
        [SerializeField] private CanvasGroup bottomChestCg;
        [SerializeField] private float allGap = 0.5f;

        private int _coinCount;
        private int _clickCount;
        private bool _ready;
        private bool _opening;

        [Inject] private CoinTosser _coinTosser;
        [Inject] private MoneyBoard _moneyBoard;
        
        private Sequence _shakeSequence;
        private Sequence _openSequence;
        private Sequence _hintFadeSequence;
        private Sequence _collectCoinsSequence;

        private void Awake()
        {
            body.SetActive(false);
            hintCanvasGroup.alpha = 0;
        }

        public void OpenChest(RectTransform start, int coinCount)
        {
            Time.timeScale = 0;

            _coinCount = coinCount;
            
            chestBottom.gameObject.SetActive(false);
            chestTop.gameObject.SetActive(false);
            coinsRoot.gameObject.SetActive(false);
            chest.gameObject.SetActive(true);
            
            _hintFadeSequence.Kill();
            hintCanvasGroup.alpha = 0;
            
            _clickCount = 0;
            _ready = false;
            _opening = false;
            
            var startSize = start.sizeDelta;
            var startPosition = start.position;
            
            chest.rotation = Quaternion.identity;
            chest.anchoredPosition = Vector2.zero;
            
            var endSize = chestBody.sizeDelta;
            var endPosition = chestBody.position;
            
            chestBody.position = startPosition;
            chestBody.sizeDelta = startSize;
            
            body.SetActive(true);
            canvasGroup.alpha = 0;

            const float duration = 1f;
            
            var sequence = DOTween.Sequence();
            sequence.Append(chestBody.DOMove(endPosition, duration));
            sequence.Join(chestBody.DOSizeDelta(endSize, duration));
            sequence.Join(canvasGroup.DOFade(1, duration));
            sequence.OnComplete(OnAppear);
            sequence.SetUpdate(true);
            sequence.Play();
        }

        private void OnAppear()
        {
            _ready = true;
            hintCanvasGroup.alpha = 0;
            _hintFadeSequence = DOTween.Sequence();
            _hintFadeSequence.Append(hintCanvasGroup.DOFade(1, 1));
            _hintFadeSequence.SetUpdate(true);
            _hintFadeSequence.Play();
        }

        private void Shake()
        {
            _shakeSequence.Kill();
            
            _shakeSequence = DOTween.Sequence();
            
            const float duration = 0.5f;

            chest.rotation = Quaternion.identity;
            
            _shakeSequence.Append(chest.DOShakePosition(
                duration: duration,
                strength: new Vector3(30f, 30f, 0),
                vibrato: 15,
                randomness: 90,
                snapping: false,
                fadeOut: true
            ));

            _shakeSequence.Join(chest.DOShakeRotation(
                duration: duration,
                strength: new Vector3(0, 0, 20f),
                vibrato: 15,
                randomness: 90,
                fadeOut: true
            ));
            
            _shakeSequence.SetUpdate(true);

            _shakeSequence.Play();
        }

        private void GenerateCoins()
        {
            for (var i = 0; i < coins.Count; i++)
            {
                coins[i].gameObject.SetActive(i < _coinCount);
            }
            
            var rootSize = coinsRoot.rect.size;
            var coinSize = coins[0].rect.size;
            var field = rootSize - coinSize;
            
            for (var i = 0; i < _coinCount; i++)
            {
                var x = Random.Range(0, field.x) + coinSize.x / 2;
                var y = Random.Range(0, field.y) + coinSize.y / 2;
                var position = new Vector2(x, y);
                coins[i].anchoredPosition = position;
            }
        }

        private void Open()
        {
            chestBottom.gameObject.SetActive(true);
            chestTop.gameObject.SetActive(true);
            coinsRoot.gameObject.SetActive(true);
            chest.gameObject.SetActive(false);
            
            _hintFadeSequence.Kill();
            hintCanvasGroup.alpha = 1;
            _hintFadeSequence = DOTween.Sequence();
            _hintFadeSequence.Append(hintCanvasGroup.DOFade(0, 1));
            _hintFadeSequence.SetUpdate(true);
            _hintFadeSequence.Play();

            var distance = 2000;
            var duration = 0.6f;
            
            GenerateCoins();

            chestBottom.anchoredPosition = Vector2.zero;
            chestTop.anchoredPosition = Vector2.zero;

            bottomChestCg.alpha = 1;
            topChestCg.alpha = 1;
            
            _openSequence.Kill();
            
            _openSequence = DOTween.Sequence();
            _openSequence.Append(chestBottom
                .DOAnchorPos(chestBottom.anchoredPosition + Vector2.down * distance, duration));
            _openSequence.Join(chestTop
                .DOAnchorPos(chestTop.anchoredPosition + Vector2.up * distance, duration));
            _openSequence.Join(bottomChestCg.DOFade(0, duration));
            _openSequence.Join(topChestCg.DOFade(0, duration));
            _openSequence.Insert(0.2f, CollectCoins());
            _openSequence.SetUpdate(true);

            _openSequence.Play();
        }

        private Sequence CollectCoins()
        {
            _collectCoinsSequence = DOTween.Sequence();
            
            for (var i = 0; i < _coinCount; i++)
            {
                var delay = allGap * i / _coinCount;
                
                var s = DOTween.Sequence();

                s.Append(_coinTosser.TossCoin(coins[i], _moneyBoard.Coin));
                s.OnComplete(() => _moneyBoard.AddMoney(1));

                _collectCoinsSequence.Insert(delay, s);
                
                coins[i].gameObject.SetActive(false);
            }

            _collectCoinsSequence.Join(End());
            _collectCoinsSequence.SetUpdate(true);
            
            return _collectCoinsSequence;
        }

        private Sequence End()
        {
            canvasGroup.alpha = 1;

            const float duration = 1f;
            
            var sequence = DOTween.Sequence();
            sequence.Join(canvasGroup.DOFade(0, duration));
            sequence.OnComplete(Hide);
            sequence.SetUpdate(true);
            sequence.Play();

            return sequence;
        }

        private void Hide()
        {
            Time.timeScale = 1;
            body.SetActive(false);
        }

        public void Click()
        {
            if (!_ready)
            {
                return;
            }

            if (_clickCount < openClickCount)
            {
                _clickCount++;
                
                Shake();
            }
            else
            {
                if (!_opening)
                {
                    _opening = true;
                    Open();
                }
            }
        }
    }
}