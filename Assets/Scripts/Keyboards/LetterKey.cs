using Grids;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Keyboards
{
    public class LetterKey : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Shadow shadow;
        [SerializeField] private TMP_Text letterText;
        [SerializeField] private Button button;
        [SerializeField] private Color consonantСolor;
        [SerializeField] private Color vowelColor;
        [SerializeField] private GameObject block;
        [SerializeField] private RectTransform rt;

        [Inject] private GridController _gridController;

        public char Letter { get; private set; }

        private Vector2? _position;

        public void SetLetter(char letter, bool isVowel)
        {
            Letter = letter;
            var color = isVowel ? vowelColor : consonantСolor;
            image.color = color;
            color *= 0.5f;
            color.a = 1;
            shadow.effectColor = color;
            letterText.text = letter.ToString();
        }

        private void SetInteractable(bool interactable)
        {
            button.interactable = interactable;
        }
        
        public void OnClick()
        {
            _gridController.TypeLetter(Letter);
        }

        public void Show()
        {
            block.SetActive(false);
            SetInteractable(true);
        }
        
        public void Hide()
        {
            block.SetActive(true);
            SetInteractable(false);
        }
        
        public Vector2 GetPosition()
        {
            if (_position is null)
            {
                var corners = new Vector3[4];
                rt.GetWorldCorners(corners);
                _position = (corners[0] + corners[2]) / 2;
            }

            return _position.Value;
        }
    }
}