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

        [Inject] private GridController _gridController;

        private char _letter;

        public void SetLetter(char letter, bool isVowel)
        {
            _letter = letter;
            var color = isVowel ? vowelColor : consonantСolor;
            image.color = color;
            color *= 0.5f;
            shadow.effectColor = color;
            letterText.text = letter.ToString();
        }

        public void SetInteractable(bool interactable)
        {
            button.interactable = interactable;
        }
        
        public void OnClick()
        {
            _gridController.TypeLetter(_letter);
        }
    }
}