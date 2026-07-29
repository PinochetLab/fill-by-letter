using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Keyboards
{
    public class LetterKeyboard : MonoBehaviour
    {
        [SerializeField] private List<LetterKey> allKeys;
        [SerializeField] private GameObject keyboard;
        [SerializeField] private List<CanvasGroup> canvasGroups;
        [SerializeField] private RectTransform keyboardRt;
        [SerializeField] private RectTransform gridRt;

        [SerializeField] private float showDuration = 0.2f;
        [SerializeField] private float hideDuration = 0.2f;

        private const string Alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        private const string Wovels = "аеёиоуыэюя";

        private List<LetterKey> _keys;

        private bool _show;
        private float _height;

        private Sequence _showSequence;

        private void Awake()
        {
            SetAlphabet(Alphabet);
            SetInteractable(false);
            _height = keyboardRt.rect.height;
            gridRt.anchoredPosition = Vector2.down * _height;
            canvasGroups.ForEach(c => c.alpha = 0);
            SwitchOff();
        }

        private void SetInteractable(bool interactable)
        {
            _keys.ForEach(k => k.SetInteractable(interactable));
        }

        private void SwitchOn()
        {
            keyboard.SetActive(true);
        }
        
        private void SwitchOff()
        {
            keyboard.SetActive(false);
        }

        public void Show()
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
            
            foreach (var canvasGroup in canvasGroups)
            {
                _showSequence.Join(canvasGroup.DOFade(1, duration));
            }
            
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
            
            foreach (var canvasGroup in canvasGroups)
            {
                _showSequence.Join(canvasGroup.DOFade(0, duration));
            }
            
            _showSequence.OnComplete(SwitchOff);
            _showSequence.SetUpdate(true);
            _showSequence.Play();
        }

        private void SetAlphabet(string alphabet)
        {
            _keys = allKeys.Take(alphabet.Length).ToList();
            
            for (var i = 0; i < allKeys.Count; i++)
            {
                if (i < alphabet.Length)
                {
                    var letter = alphabet[i];
                    allKeys[i].SetLetter(letter, Wovels.Contains(letter));
                    allKeys[i].gameObject.SetActive(true);
                }
                else
                {
                    allKeys[i].gameObject.SetActive(false);
                }
            }
        }
    }
}