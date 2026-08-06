using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Grids;
using Keyboards;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using WordBoards;
using Zenject;

namespace Tutorials
{
    public class TutorialBoard : MonoBehaviour
    {
        [SerializeField] private GameObject screen;
        [SerializeField] private GameObject raycastBlock;
        [SerializeField] private RectTransform hand;
        [SerializeField] private TMP_Text text;
        [SerializeField] private CanvasGroup handCanvasGroup;
        [SerializeField] private Transform textBlock;
        [SerializeField] private CanvasGroup textCanvasGroup;
        
        [SerializeField] private LocalizedAsset<Tutorial> tutorial;
        
        public bool IsActive { get; private set; }
        public List<Vector2Int> Path { get; private set; }
        public bool OneLine { get; private set; }

        [Inject] private GridController _gridController;
        [Inject] private LetterKeyboard _letterKeyboard;
        [Inject] private WordBoard _wordBoard;

        private Tutorial _tutorial;
        private TutorialStep _step;
        private int _tutorialIndex;
        private LetterBlock[, ] _grid;
        private int _pathLetterCount;
        private readonly List<Vector2Int> _shownCells = new ();
        private bool _isSelectCellTextUsed;
        private bool _isTypeLetterTextUsed;
        private bool _isTypeWordTextUsed;
        private bool _isAddLetterTextUsed;
        private bool _isStartWordTextUsed;

        private bool _textFirstTime = true;

        private Sequence _handSequence;

        private void Awake()
        {
            screen.SetActive(false);
            raycastBlock.SetActive(false);
        }

        private void Update()
        {
            if (!_handSequence.IsActive())
            {
                return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                _handSequence.Pause();
                handCanvasGroup.alpha = 0;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _handSequence.Restart();
            }
        }

        private Sequence ShowText(float duration)
        {
            var d = duration * (1 - textCanvasGroup.alpha);
                
            var sequence = DOTween.Sequence();
            sequence.Append(textBlock.DOScale(1, d));
            sequence.Join(textCanvasGroup.DOFade(1, d));

            return sequence;
        }
        
        private Sequence HideText(float duration)
        {
            var d = duration * textCanvasGroup.alpha;
            
            var sequence = DOTween.Sequence();
            sequence.Append(textBlock.DOScale(0, d));
            sequence.Join(textCanvasGroup.DOFade(0, d));

            return sequence;
        }
        
        private void ChangeText(string s)
        {
            var sequence = DOTween.Sequence();
            if (!_textFirstTime)
            {
                sequence.Append(HideText(0.3f));
            }
            else
            {
                _textFirstTime = false;
            }
            sequence.AppendCallback(() => text.text = s);
            sequence.Append(ShowText(0.3f));
            sequence.SetUpdate(true);
            sequence.Play();
        }

        private void HintPosition(Vector2 position)
        {
            _handSequence.Kill();
            
            _handSequence = DOTween.Sequence();
            
            hand.position = position;
            hand.localScale = Vector3.one;
            handCanvasGroup.alpha = 0;

            _handSequence.AppendInterval(1f);
            
            _handSequence.Append(handCanvasGroup.DOFade(1f, 0.2f));
             
            _handSequence.Append(hand.DOScale(1.1f, 0.3f).SetLoops(4, LoopType.Yoyo));
            
            _handSequence.Append(handCanvasGroup.DOFade(0f, 0.2f));

            _handSequence.SetLoops(-1, LoopType.Restart);
            
            _handSequence.SetUpdate(true);

            _handSequence.Play();
        }

        private void HintPath(List<Vector2> path)
        {
            _handSequence.Kill();
            
            _handSequence = DOTween.Sequence();
            
            hand.position = path[0];
            handCanvasGroup.alpha = 0;
            
            _handSequence.AppendInterval(1f);

            var p = path.Select(c => (Vector3)c).ToArray();
            
            _handSequence.Append(handCanvasGroup.DOFade(1f, 0.2f));

            _handSequence.Append(hand.DOPath(p, 0.3f * path.Count).SetEase(Ease.Linear));
            
            _handSequence.Append(handCanvasGroup.DOFade(0f, 0.2f));
            
            _handSequence.SetLoops(-1, LoopType.Restart);
            
            _handSequence.SetUpdate(true);

            _handSequence.Play();
        }

        public void Show(LetterBlock[, ] grid, UnityAction onEnd)
        {
            Time.timeScale = 0;
            
            screen.SetActive(true);
            raycastBlock.SetActive(true);
            handCanvasGroup.alpha = 0;
            _grid = grid;
            _tutorial = tutorial.LoadAsset();
            
            foreach (var block in _grid)
            {
                block.Hide();
            }

            IsActive = true;
            
            ProcessNextStep();
        }

        private LetterBlock GetBlock(Vector2Int cell)
        {
            return _grid[cell.x, cell.y];
        }
        
        private void HideShownCells()
        {
            _shownCells.ForEach(c => _grid[c.x, c.y].Hide());
            _shownCells.Clear();
        }

        private void ShowCell(Vector2Int cell)
        {
            _shownCells.Add(cell);
            _grid[cell.x, cell.y].Show();
        }

        private void ProcessNextStep()
        {
            if (_tutorialIndex == _tutorial.Steps.Count)
            {
                End();
                return;
            }
            
            _step = _tutorial.Steps[_tutorialIndex];
            
            Path = _step.Path;
            OneLine = _step.OneLine;
            _pathLetterCount = 0;
            
            _tutorialIndex++;
            
            SelectCell();
        }

        private void End()
        {
            Time.timeScale = 1f;
            
            foreach (var block in _grid)
            {
                block.Show();
            }
            
            _letterKeyboard.ShowAll();
            
            _wordBoard.ShowYes();
            _wordBoard.ShowNo();
            
            screen.SetActive(false);
            raycastBlock.SetActive(false);
            
            _gridController.OnEndTutorial();

            IsActive = false;
        }

        private void SelectCell()
        {
            var cell = _step.LetterCell;
            var block = GetBlock(cell);
            
            HideShownCells();

            ShowCell(cell);

            var position = block.GetScreenPosition();
            
            HintPosition(position);

            if (!_isSelectCellTextUsed)
            {
                _isSelectCellTextUsed = true;
                ChangeText(_tutorial.SelectCell1Text);
            }
            else
            {
                ChangeText(_tutorial.SelectCell2Text);
            }
        }

        public void OnOpenKeyboard()
        {
            HideShownCells();
            
            var position = _letterKeyboard.ShowLetter(_step.Letter);
            
            HintPosition(position);
            
            if (!_isTypeLetterTextUsed)
            {
                _isTypeLetterTextUsed = true;
                var s = string.Format(_tutorial.TypeLetter1Text, _step.Letter.ToString().ToUpperInvariant());
                ChangeText(s);
            }
            else
            {
                var s = string.Format(_tutorial.TypeLetter2Text, _step.Letter.ToString().ToUpperInvariant());
                ChangeText(s);
            }
        }

        public void OnTypeLetter()
        {
            if (OneLine)
            {
                ExplainWord();
            }
            else
            {
                ExplainLetter();
            }
        }

        private void ExplainWord()
        {
            HideShownCells();
            
            Path.ForEach(ShowCell);

            var positions = Path.Select(c => GetBlock(c).GetScreenPosition()).ToList();
            
            HintPath(positions);
            
            _wordBoard.HideYes();
            _wordBoard.HideNo();

            var word = new string(Path.Select(c => _grid[c.x, c.y].Letter).ToArray()).ToUpperInvariant();
            
            var s  = string.Format(_tutorial.WriteWordText, word);
            ChangeText(s);
        }

        private void ExplainLetter()
        {
            var cell = Path[_pathLetterCount];
            
            ShowCell(cell);
            
            HintPosition(GetBlock(cell).GetScreenPosition());

            if (_pathLetterCount == 0)
            {
                if (!_isStartWordTextUsed)
                {
                    _isStartWordTextUsed = true;
                    var s = _tutorial.StartWord1Text;
                    ChangeText(s);
                }
                else
                {
                    var s = _tutorial.StartWord2Text;
                    ChangeText(s);
                }
            }
            else
            {
                if (!_isAddLetterTextUsed)
                {
                    _isAddLetterTextUsed = true;
                    var s = _tutorial.AddLetter1Text;
                    ChangeText(s);
                }
                else
                {
                    var s = _tutorial.AddLetter2Text;
                    ChangeText(s);
                }
            }
        }

        public void OnAddLetter()
        {
            _pathLetterCount++;
            if (_pathLetterCount == _step.Path.Count)
            {
                if (_step.Cancel)
                {
                    ExplainNo();
                }
                else
                {
                    ExplainYes();
                }
            }
            else
            {
                ExplainLetter();
            }
        }

        private void ExplainYes()
        {
            _wordBoard.HideNo();
            _wordBoard.ShowYes();
            
            HintPosition(_wordBoard.GetYesPosition());
            
            var s = _tutorial.OkText;
            ChangeText(s);
        }

        private void ExplainNo()
        {
            _wordBoard.HideYes();
            _wordBoard.ShowNo();
            
            HintPosition(_wordBoard.GetNoPosition());
            
            var s = _tutorial.CancelText;
            ChangeText(s);
        }
        
        public void OnYes()
        {
            ProcessNextStep();
        }

        public void OnNo()
        {
            ProcessNextStep();
        }

        public void OnTypeWord()
        {
            ProcessNextStep();
        }
    }
}