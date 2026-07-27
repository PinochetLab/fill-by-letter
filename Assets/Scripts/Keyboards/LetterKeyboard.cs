using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Keyboards
{
    public class LetterKeyboard : MonoBehaviour
    {
        [SerializeField] private List<LetterKey> allKeys;
        [SerializeField] private GameObject grid;
        [SerializeField] private RectTransform gridRt;

        [SerializeField] private float showDuration = 0.2f;
        [SerializeField] private float hideDuration = 0.2f;

        private const string Alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";

        private List<LetterKey> _keys;

        private Sequence _showSequence;

        private void Start()
        {
            SetAlphabet(Alphabet);
            SetInteractable(false);
        }

        private void SetInteractable(bool interactable)
        {
            grid.SetActive(interactable);
            _keys.ForEach(k => k.SetInteractable(interactable));
        }

        public void Show()
        {
            _showSequence.Kill();
            
            SetInteractable(true);
            
            var height = gridRt.rect.height;
            var startPos = Vector2.down * height;
            gridRt.anchoredPosition = startPos;

            _showSequence = DOTween.Sequence();
            _showSequence.Append(gridRt.DOAnchorPosY(0, showDuration));
            _showSequence.Play();
        }

        public void Hide()
        {
            _showSequence.Kill();
            
            var height = gridRt.rect.height;
            gridRt.anchoredPosition = Vector2.zero;

            _showSequence = DOTween.Sequence();
            _showSequence.Append(gridRt.DOAnchorPosY(-height, showDuration));
            _showSequence.Play();
        }

        private void SetAlphabet(string alphabet)
        {
            _keys = allKeys.Take(alphabet.Length).ToList();
            
            for (var i = 0; i < allKeys.Count; i++)
            {
                if (i < alphabet.Length)
                {
                    allKeys[i].SetLetter(alphabet[i]);
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