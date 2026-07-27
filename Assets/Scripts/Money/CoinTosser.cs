using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Money
{
    public class CoinTosser : MonoBehaviour
    {
        [SerializeField] private List<RectTransform> coins;

        private RectTransform TakeCoin()
        {
            var coin = coins.Find(c => !c.gameObject.activeSelf);

            if (coin == null)
            {
                throw new NotImplementedException("Not enough coins");
            }
            
            coin.gameObject.SetActive(true);
            return coin;
        }

        private void ReleaseCoin(RectTransform coin)
        {
            coin.gameObject.SetActive(false);
        }

        public Tween TossCoin(RectTransform a, RectTransform b)
        {
            var startSize = a.rect.size;
            var endSize = b.rect.size;
            
            var startPos = a.position;
            var endPos = b.position;
            
            var coin = TakeCoin();
            
            coin.DOMove(endPos, 1f).SetEase(Ease.OutQuad);
            
            coin.position = startPos;
            coin.sizeDelta = startSize;

            var duration = 1f;
            
            var sequence = DOTween.Sequence();
            
            var arcHeight = 800f;
            var midPoint = (startPos + endPos) / 2;
            midPoint.y += arcHeight + Random.Range(-0.5f, 0.5f);
            
            Vector3[] path = { startPos, midPoint, endPos };
            
            //sequence.Append(coin.DOPath(path, duration, PathType.CatmullRom).SetEase(Ease.OutQuad));
            sequence.Append(coin.DOMove(endPos, duration).SetEase(Ease.OutQuad));
            
            sequence.Join(coin.DOSizeDelta(endSize, duration).SetEase(Ease.InOutQuad));
            
            sequence.SetUpdate(true);
            
            sequence.AppendCallback(() => ReleaseCoin(coin));
    
            return sequence;
        }
    }
}