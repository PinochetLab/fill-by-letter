using Grids;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace WordBoards
{
    public class WordBoard : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text wordText;
        [SerializeField] private TMP_Text selectLetterText;

        [SerializeField] private Button noButton;
        [SerializeField] private Button yesButton;
        
        [Inject] private GridController _gridController;

        private string _currentWord = string.Empty;

        private void Awake()
        {
            Hide();
            UpdateWord();
        }

        private void UpdateWord()
        {
            wordText.text = _currentWord;
            wordText.gameObject.SetActive(_currentWord.Length > 0);
            selectLetterText.gameObject.SetActive(_currentWord.Length == 0);
            yesButton.interactable = _currentWord.Length > 1;
        }

        public void AddLetter(char letter)
        {
            _currentWord += letter;
            UpdateWord();
        }

        public void RemoveLetter()
        {
            _currentWord = _currentWord.Remove(_currentWord.Length - 1);
            UpdateWord();
        }

        public void Clear()
        {
            _currentWord = string.Empty;
            UpdateWord();
        }

        public void Show()
        {
            panel.SetActive(true);
        }
        
        public void Hide()
        {
            panel.SetActive(false);
        }

        public void Yes()
        {
            _gridController.CompletePath();
        }
        
        public void No()
        {
            _gridController.OnClearWord();
        }
    }
}