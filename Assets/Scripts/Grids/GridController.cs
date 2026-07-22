using System.Collections.Generic;
using System.Linq;
using AIs;
using Bonuses;
using JetBrains.Annotations;
using Keyboards;
using Progress;
using UnityEngine;
using UnityEngine.UI;
using WordBoards;
using Words;
using Zenject;

namespace Grids
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private float spacingRatio = 0.05f;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        
        [SerializeField] private List<LetterBlock> blocks;

        [SerializeField] private int size = 5;
        [SerializeField] private string word = "баран";

        [Inject] private LetterKeyboard _letterKeyboard;
        [Inject] private WordBoard _wordBoard;
        [Inject] private ProgressBoard _progressBoard;
        [Inject] private TrieWordChecker _trieWordChecker;
        [Inject] private BonusController _bonusController;
        [Inject] private WordHinter _wordHinter;
        
        [Inject] private HintLetterButton _hintLetterButton;
        [Inject] private HintLetterPlaceButton _hintLetterPlaceButton;
        [Inject] private HintWordButton _hintWordButton;
        
        [Inject] private DiContainer _container;

        public bool CanPath { get; private set; }
        
        private int _width;
        private int _height;
        private LetterBlock[,] _grid;
        private Solver _solver;
        
        private char? HintedLetter { get; set; }
        private Vector2Int? HintedPos { get; set; }

        private List<LetterBlock> _path = new ();

        private LetterBlock _selectedBlock;

        [CanBeNull] private Answer _answer;

        private Answer Answer
        {
            get
            {
                _answer ??= CalculateAnswer();
                return _answer;
            }
        }

        private Answer CalculateAnswer()
        {
            var letters = GetLetters();

            var availablePositions = _grid
                .Cast<LetterBlock>()
                .Where(b => b.State == BlockState.EmptyAvailable)
                .Select(b => b.Position).ToList();

            return _solver.FindBestWord(letters, availablePositions);
        }

        private void ClearAnswer()
        {
            _answer = null;
        }

        private void Awake()
        {
            _solver = _container.Instantiate<Solver>();
            BuildGrid(size, size);
            SetWord(word);
            _bonusController.ShowBonuses();
        }

        public void BuildGrid(int width, int height)
        {
            gridLayoutGroup.enabled = true;

            for (var i = 0; i < blocks.Count; i++)
            {
                blocks[i].transform.SetSiblingIndex(i);
            }
            
            var size = rectTransform.rect.width;

            gridLayoutGroup.constraintCount = width;
            
            var max = Mathf.Max(width, height);
            var sum = (max + (max - 1) * spacingRatio);
            var cellSize = size / sum;
            var spacing = cellSize * spacingRatio;
            
            gridLayoutGroup.cellSize = Vector2.one * cellSize;
            gridLayoutGroup.spacing = Vector2.one * spacing;
            
            _width = width;
            _height = height;
            _grid = new LetterBlock[width, height];

            for (var i = 0; i < blocks.Count; i++)
            {
                if (i < width * height)
                {
                    blocks[i].gameObject.SetActive(true);
                    
                    var x = i % width;
                    var y = i / width;

                    _grid[x, y] = blocks[i];
                    _grid[x, y].SetUp(x, y);
                    
                }
                else
                    blocks[i].gameObject.SetActive(false);
            }
        }

        public void SetWord(string word)
        {
            _progressBoard.AddStartWord(word);
            
            if (word.Length != _width)
            {
                Debug.LogError($"Word length {word.Length} is wrong");
                return;
            }

            var y = _height / 2;

            for (var i = 0; i < _width; i++)
            {
                _grid[i, y].SetLetter(word[i]);
            }
            
            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _width; j++)
                {
                    if (j == y)
                    {
                        _grid[i, j].State = BlockState.Filled;
                    }
                    else if (j == y - 1 || j == y + 1)
                    {
                        _grid[i, j].State = BlockState.EmptyAvailable;
                    }
                    else
                    {
                        _grid[i, j].State = BlockState.EmptyNotAvailable;
                    }
                }
            }
        }

        public void SelectBlock(LetterBlock block)
        {
            if (_path != null)
            {
                ClearPath();
            }
            
            if (_selectedBlock)
            {
                _selectedBlock.State = BlockState.EmptyAvailable;
            }
            
            _bonusController.HideBonuses();
            
            CanPath = false;
            _letterKeyboard.SetInteractable(true);
            _selectedBlock = block;
        }

        public bool AreNear(LetterBlock a, LetterBlock b)
        {
            return a.Y == b.Y && (a.X == b.X - 1 || a.X == b.X + 1)
                || a.X == b.X && (a.Y == b.Y - 1 || a.Y == b.Y + 1);
        }

        public void DeselectBlock()
        {
            _letterKeyboard.SetInteractable(false);
            _selectedBlock = null;
            CanPath = false;
            _bonusController.ShowBonuses();
        }

        private char?[,] GetLetters()
        {
            var letters = new char?[_grid.GetLength(0), _grid.GetLength(1)];

            for (var i = 0; i < _grid.GetLength(0); i++)
            {
                for (var j = 0; j < _grid.GetLength(1); j++)
                {
                    if (_grid[i, j].State is BlockState.EmptyNotAvailable or BlockState.EmptyAvailable)
                    {
                        letters[i, j] = null;
                    }
                    else
                    {
                        letters[i, j] = _grid[i, j].Letter;
                    }
                }
            }

            return letters;
        }

        public void TypeLetter(char letter)
        {
            _selectedBlock.SetLetter(letter);
            _selectedBlock.State = BlockState.FilledNew;
            _path.Clear();
            CanPath = true;
            _letterKeyboard.SetInteractable(false);
            _wordBoard.Show();
        }

        public bool TryAddToPath(LetterBlock block)
        {
            if (_path.Count != 0 && (!AreNear(_path.Last(), block) || _path.Contains(block)))
            {
                return false;
            }
            
            _trieWordChecker.AddLetter(block.Letter);
            _path.Add(block);
            _wordBoard.AddLetter(block.Letter);
            gridLayoutGroup.enabled = false;
            block.transform.SetAsFirstSibling();
            return true;

        }
        
        public bool TryRemoveFromPath(LetterBlock block)
        {
            if (_path.Count == 0)
            {
                return false;
            }

            if (block != _path.Last()) return false;
            
            _trieWordChecker.RemoveLetter();
            _wordBoard.RemoveLetter();
            
            _path.RemoveAt(_path.Count - 1);

            if (_path.Count == 0)
            {
                //ClearPath();
                //DeselectBlock();
            }
                
            return true;

        }

        private void ClearPath()
        {
            _trieWordChecker.Reset();
            
            _wordBoard.Clear();
            _wordBoard.Hide();
            
            foreach (var b in _path)
            {
                b.DePath();
            }
            
            _path.Clear();
        }

        public void OnClearWord()
        {
            ClearPath();
            _selectedBlock.State = BlockState.EmptyAvailable;
            DeselectBlock();
        }

        public bool TryGetAntiDirection(out Vector2Int antiDirection)
        {
            if (_path.Count < 2)
            {
                antiDirection = Vector2Int.zero;
                return false;
            }
            
            antiDirection = new Vector2Int(_path[^2].X - _path[^1].X, _path[^2].Y - _path[^1].Y);
            return true;
        }
        
        public List<Vector2Int> GetDirections(Vector2Int v)
        {
            var x = v.x;
            var y = v.y;
            
            var coordinates = new List<Vector2Int>();
            
            if (x > 0) 
            {
                coordinates.Add(Vector2Int.left);
            }
            if (x <  _width - 1)
            {
                coordinates.Add(Vector2Int.right);
            }
            if (y > 0)
            {
                coordinates.Add(Vector2Int.down);
            }
            if (y < _height - 1)
            {
                coordinates.Add(Vector2Int.up);
            }
            
            return coordinates;
        }
        
        public List<Vector2Int> GetNeighbours(Vector2Int v)
        {
            var x = v.x;
            var y = v.y;
            
            var coordinates = new List<Vector2Int>();
            
            if (x > 0) 
            {
                coordinates.Add(new Vector2Int(x - 1, y));
            }
            if (x <  _width - 1)
            {
                coordinates.Add(new Vector2Int(x + 1, y));
            }
            if (y > 0)
            {
                coordinates.Add(new Vector2Int(x, y - 1));
            }
            if (y < _height - 1)
            {
                coordinates.Add(new Vector2Int(x, y + 1));
            }
            
            return coordinates;
        }

        private List<LetterBlock> GetNeighbours(LetterBlock block)
        {
            return GetNeighbours(new Vector2Int(block.X, block.Y)).Select(v => _grid[v.x, v.y]).ToList();
        }

        public bool PathContainsNewLetter()
        {
            return _path.Contains(_selectedBlock);
        }

        public void CompletePath()
        {
            var word = string.Join("", _path.Select(b => b.Letter.ToString()));

            if (!_trieWordChecker.IsWordCorrect() || !_progressBoard.CanMake(word))
            {
                return;
            }
            
            var letterIndex = _path.IndexOf(_selectedBlock);
            
            _progressBoard.MakeWord(word, letterIndex);
            
            ClearPath();
            _selectedBlock.State = BlockState.Filled;

            foreach (var block in GetNeighbours(_selectedBlock))
            {
                if (block.State == BlockState.EmptyNotAvailable)
                {
                    block.State = BlockState.EmptyAvailable;
                }
            }
            
            DeselectBlock();

            SwitchOffBonuses();
            ClearAnswer();
        }

        private void SwitchOffBonuses()
        {
            _hintLetterButton.SwitchOff();
            _hintLetterPlaceButton.SwitchOff();
            _hintWordButton.SwitchOff();
        }
        
        public void StartHintLetter()
        {
            var letter = Answer.Letter;

            HintedLetter = letter;

            if (HintedPos is null)
            {
                foreach (var block in _grid)
                {
                    if (block.State == BlockState.EmptyAvailable)
                    {
                        block.StartHintLetter(letter);
                    }
                }
            }
            else
            {
                var p = HintedPos.Value;
                _grid[p.x, p.y].StartHintLetter(letter);
            }
        }

        public void StopHintLetter()
        {
            HintedLetter = null;
            
            foreach (var block in _grid)
            {
                if (block.State == BlockState.EmptyAvailable)
                {
                    block.StopHintLetter();
                }
            }
        }
        
        public void StartHintLetterPlace()
        {
            var p = Answer.LetterPos;

            HintedPos = p;

            if (HintedLetter is not null)
            {
                foreach (var block in _grid)
                {
                    if (block.State == BlockState.EmptyAvailable)
                    {
                        block.StopHintLetter();
                    }
                }
                
                _grid[p.x, p.y].StartHintLetter(HintedLetter.Value);
            }
            
            gridLayoutGroup.enabled = false;

            _grid[p.x, p.y].StartHint();
        }

        public void StopHintLetterPlace()
        {
            var p = Answer.LetterPos;

            HintedPos = null;
            
            _grid[p.x, p.y].StopHint();
            
            if (HintedLetter is not null)
            {
                foreach (var block in _grid)
                {
                    if (block.State == BlockState.EmptyAvailable)
                    {
                        block.StartHintLetter(HintedLetter.Value);
                    }
                }
            }
        }

        public void StartHintWord()
        {
            _wordHinter.StartHint(Answer.Word);
        }
        
        public void StopHintWord()
        {
            _wordHinter.StopHint();
        }
    }
}
