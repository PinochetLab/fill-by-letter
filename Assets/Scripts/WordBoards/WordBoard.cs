using Boards;
using Grids;
using TMPro;
using Tutorials;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace WordBoards
{
    public class WordBoard : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private GameObject blackScreenBlock;
        //[SerializeField] private GameObject background;
        [SerializeField] private TMP_Text wordText;
        [SerializeField] private TMP_Text selectLetterText;

        [SerializeField] private Button noButton;
        [SerializeField] private Button yesButton;
        
        [SerializeField] private RectTransform noRt;
        [SerializeField] private RectTransform yesRt;
        
        [Inject] private GridController _gridController;
        [Inject] private TutorialBoard _tutorialBoard;

        private string _currentWord = string.Empty;

        private void UpdateWord()
        {
            wordText.text = _currentWord;
            wordText.gameObject.SetActive(_currentWord.Length > 0);
            selectLetterText.gameObject.SetActive(_currentWord.Length == 0);
            if (!_tutorialBoard.IsActive)
            {
                yesButton.interactable = _currentWord.Length > 1;
            }
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

        public void Show(bool blackScreen = true)
        {
            panel.SetActive(true);
            blackScreenBlock.SetActive(blackScreen);
            //background.SetActive(true);
        }
        
        public void Hide()
        {
            panel.SetActive(false);
            blackScreenBlock.SetActive(false);
            //background.SetActive(false);
        }

        public void Yes()
        {
            if (_tutorialBoard.IsActive)
            {
                _tutorialBoard.OnYes();
            }
            _gridController.CompletePath();
        }
        
        public void No()
        {
            if (_tutorialBoard.IsActive)
            {
                _tutorialBoard.OnNo();
            }
            _gridController.OnClearWord();
        }

        public void ShowYes()
        {
            yesButton.interactable = true;
        }
        
        public void HideYes()
        {
            yesButton.interactable = false;
        }
        
        public void ShowNo()
        {
            noButton.interactable = true;
        }
        
        public void HideNo()
        {
            noButton.interactable = false;
        }

        public Vector2 GetYesPosition()
        {
            var corners = new Vector3[4];
            yesRt.GetWorldCorners(corners);
            return (corners[0] + corners[2]) / 2;
        }
        
        public Vector2 GetNoPosition()
        {
            var corners = new Vector3[4];
            noRt.GetWorldCorners(corners);
            return (corners[0] + corners[2]) / 2;
        }
    }
}