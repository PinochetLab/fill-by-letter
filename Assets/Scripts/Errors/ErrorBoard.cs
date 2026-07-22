using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Errors
{
    public class ErrorBoard : MonoBehaviour
    {
        [SerializeField] private GameObject body;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TMP_Text errorText;
        
        [Inject] private ErrorRankColorPalette _errorRankColorPalette;

        private Tween _fadeTweener;

        private void Awake()
        {
            body.SetActive(false);
        }

        public void PopUp(ErrorType errorType, params object[] ps)
        {
            if (_fadeTweener != null && _fadeTweener.IsActive())
            {
                _fadeTweener.Kill();
            }

            body.SetActive(true);
            canvasGroup.alpha = 1;
            var color = _errorRankColorPalette.GetColor(errorType.Rank);
            backgroundImage.color = color;
            ps = ps.Select(p => $"<color=black>{p}</color>").ToArray();
            errorText.text = string.Format(errorType.Text, ps);
            
            var sequence = DOTween.Sequence();
    
            sequence.Append(body.transform.DOScale(1.1f, 0.2f));
    
            sequence.Append(body.transform.DOScale(1f, 0.2f));
    
            sequence.AppendInterval(2f);
    
            sequence.Append(canvasGroup.DOFade(0, 1f));
    
            sequence.OnComplete(() => body.SetActive(false));
    
            _fadeTweener = sequence;
        }
    }
}