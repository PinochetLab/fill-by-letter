using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Keyboards
{
    public class LetterKeyboard : MonoBehaviour
    {
        [SerializeField] private List<LetterKey> allKeys;
        [SerializeField] private GameObject keyboard;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform keyboardRt;
        [SerializeField] private RectTransform gridRt;
        [SerializeField] private CanvasGroup interactableCg;
        [SerializeField] private CanvasGroup keyboardCg;
        [SerializeField] private GridLayoutAdvanced gridLayoutAdvanced;

        [SerializeField] private float showDuration = 0.2f;
        [SerializeField] private float hideDuration = 0.2f;

        private List<LetterKey> _keys;

        private bool _show;
        private float _height;

        private string _alphabet;
        private string _wovels;

        private Sequence _showSequence;

        public void SetUp()
        {
            var alphabetEntry = new LocalizedAsset<Alphabet>
            {
                TableReference = "AlphabetTable",
                TableEntryReference = "Alphabet"
            };
            
            var handle = alphabetEntry.LoadAssetAsync();
            handle.WaitForCompletion();

            var alphabet = handle.Result;
            
            _alphabet = alphabet.AllLetters;
            _wovels = alphabet.Wovels;
            
            SetAlphabet();
            SetInteractable(false);
            _height = keyboardRt.rect.height;
            gridRt.anchoredPosition = Vector2.down * _height;
            canvasGroup.alpha = 0;
            SwitchOff();
            
            var aspectRatio = (float)Screen.width / Screen.height;
            var targetRatio = 1170f / 2532f;

            var ratio = aspectRatio / targetRatio;
            ratio = Mathf.Sqrt(ratio);

            var cellsPerLine = (int)(7 * ratio);

            gridLayoutAdvanced.CellsPerLine = cellsPerLine;
            gridLayoutAdvanced.ReplaceCells();
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(gridRt);
        }

        private void SetInteractable(bool interactable)
        {
            interactableCg.interactable = interactable;
        }

        private void SwitchOn()
        {
            keyboard.SetActive(true);
        }
        
        private void SwitchOff()
        {
            keyboard.SetActive(false);
        }

        public void Show(bool blackScreen = true)
        {
            if (_show)
            {
                return;
            }
            
            _showSequence.Kill();
            
            SwitchOn();

            _show = true;

            var deltaHeight = Mathf.Abs(gridRt.anchoredPosition.y);
            var duration =  showDuration * (deltaHeight / _height);

            _showSequence = DOTween.Sequence();
            _showSequence.Append(gridRt.DOAnchorPosY(0, duration));
            if (blackScreen)
            {
                _showSequence.Join(canvasGroup.DOFade(1, duration));
            }

            _showSequence.Join(keyboardCg.DOFade(1, duration));
            _showSequence.OnComplete(() => SetInteractable(true));
            _showSequence.SetUpdate(true);
            _showSequence.Play();
        }

        public void Hide()
        {
            if (!_show)
            {
                return;
            }

            SetInteractable(false);
            
            _showSequence.Kill();

            _show = false;
            
            var deltaHeight = Mathf.Abs(_height + gridRt.anchoredPosition.y);
            var duration =  showDuration * (deltaHeight / _height);

            _showSequence = DOTween.Sequence();
            _showSequence.Append(gridRt.DOAnchorPosY(-_height, duration));
            _showSequence.Join(canvasGroup.DOFade(0, duration));
            _showSequence.Join(keyboardCg.DOFade(0, duration));
            _showSequence.OnComplete(SwitchOff);
            _showSequence.SetUpdate(true);
            _showSequence.Play();
        }

        private void SetAlphabet()
        {
            _keys = allKeys.Take(_alphabet.Length).ToList();
            
            for (var i = 0; i < allKeys.Count; i++)
            {
                if (i < _alphabet.Length)
                {
                    var letter = _alphabet[i];
                    allKeys[i].SetLetter(letter, _wovels.Contains(letter));
                    allKeys[i].gameObject.SetActive(true);
                }
                else
                {
                    allKeys[i].gameObject.SetActive(false);
                }
            }
            
            keyboard.SetActive(true);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(gridRt);

            for (var i = 0; i < _alphabet.Length; i++)
            {
                allKeys[i].GetPosition();
            }
            
            keyboard.SetActive(false);
        }

        public Vector2 ShowLetter(char letter)
        {
            _keys.ForEach(k => k.Hide());

            var index = _alphabet.IndexOf(letter);

            var key = _keys[index];
            
            key.Show();

            return key.GetPosition();
        }

        public void ShowAll()
        {
            _keys.ForEach(k => k.Show());
        }
    }
}