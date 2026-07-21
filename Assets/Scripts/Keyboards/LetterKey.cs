using Grids;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Keyboards
{
    public class LetterKey : MonoBehaviour
    {
        [SerializeField] private TMP_Text letterText;
        [SerializeField] private Button button;

        [Inject] private GridController _gridController;

        private char _letter;

        public void SetLetter(char letter)
        {
            _letter = letter;
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