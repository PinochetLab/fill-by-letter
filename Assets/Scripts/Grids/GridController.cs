using System.Collections.Generic;
using System.Linq;
using AIs;
using Bonuses;
using Cysharp.Threading.Tasks;
using Errors;
using JetBrains.Annotations;
using Keyboards;
using Levels;
using Progress;
using Themes;
using Boards;
using DG.Tweening;
using Game;
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
        [SerializeField] private GameObject raycastBlock; 
        [SerializeField] private float appearDuration = 1f; 
        [SerializeField] private float startRatio = 0.7f; 
        
        [SerializeField] private List<LetterBlock> blocks;

        [Inject] private GameController _gameController;
        [Inject] private LetterKeyboard _letterKeyboard;
        [Inject] private WordBoard _wordBoard;
        [Inject] private ProgressBoard _progressBoard;
        [Inject] private TrieWordChecker _trieWordChecker;
        [Inject] private WordHinter _wordHinter;
        [Inject] private ErrorBoard _errorBoard;
        
        [Inject(Id = BonusType.Letter)] private BonusButton _letterButton;
        [Inject(Id = BonusType.Cell)] private BonusButton _cellButton;
        [Inject(Id = BonusType.Word)] private BonusButton _wordButton;
        [Inject(Id = BonusType.Replace)] private BonusButton _replaceButton;
        [Inject(Id = BonusType.Erase)] private BonusButton _eraseButton;
        [Inject(Id = BonusType.Flag)] private BonusButton _flagButton;
        
        [Inject] private ThemeController _themeController;
        [Inject] private WordMiniBoard _wordMiniBoard;
        
        [Inject] private CellTutorialBoard _cellTutorialBoard;
        
        [Inject] private DiContainer _container;

        [Inject(Id = "WordIsAlreadyCollected")]
        private ErrorType _errorTypeWordIsAlreadyCollected;
        
        [Inject(Id = "WordDoesNotExist")]
        private ErrorType _errorTypeWordDoesNotExist;
        
        [Inject(Id = "WordDoesNotContainNewLetter")]
        private ErrorType _errorTypeWordDoesNotContainNewLetter;

        public bool CanPath { get; private set; }

        public List<LetterBlock> Path { get; } = new ();

        private int _size;
        private LetterBlock[,] _grid;
        private Solver _solver;
        private Level _level;
        private int _filledCellCount;
        private int _needToFill;

        private HashSet<CellType> _types = new ();
        public bool Drag { get; private set; }

        private bool _pathChanged;
        
        private char? HintedLetter { get; set; }
        private Vector2Int? HintedPos { get; set; }

        private LetterBlock _selectedBlock;

        public bool Replace { get; private set; }
        
        public Vector2Int? ReplacePlace { get; private set; }
        
        public bool Erase { get; private set; }
        
        public bool Flag { get; private set; }

        [CanBeNull] private Answer _answer;

        public int Center => _size / 2;

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

        public Sequence Disappear()
        {
            raycastBlock.SetActive(true);
            
            var sequence = AppearDisappear(false);
            
            sequence.SetUpdate(true);

            return sequence;
        }
        
        private Sequence Appear()
        {
            var sequence = AppearDisappear(true);

            sequence.OnComplete(OnAppear);

            return sequence;
        }

        private Sequence AppearDisappear(bool appear)
        {
            raycastBlock.SetActive(true);
            
            var sequence = DOTween.Sequence();
            var isEmpty = true;

            var size = _level.Size;

            var dMax = 2 * size - 1;

            var duration = appearDuration / dMax;
            var gap = duration * startRatio;

            for (var d = 0; d < dMax; d++)
            {
                var i = d;
                var j = 0;
                if (d > size - 1)
                {
                    i = size - 1;
                    j = d - size + 1;
                }

                var s = DOTween.Sequence();
                var delay = d == 0 ? 0 : duration * d - gap;
                s.AppendInterval(delay);

                var q = DOTween.Sequence();
                var empty = true;
                
                while (i >= 0 && j < size)
                {
                    var block = appear ? _grid[i, j] : _grid[_size - i - 1, _size - j - 1];
                    var seq = appear ? block.Appear(duration) : block.Disappear(duration);
                    if (empty)
                    {
                        q.Append(seq);
                        empty = false;
                    }
                    else
                    {
                        q.Join(seq);
                    }
                    i--;
                    j++;
                }

                s.Append(q);
                
                if (isEmpty)
                {
                    sequence.Append(s);
                    isEmpty = false;
                }
                else
                {
                    sequence.Join(s);
                }
            }

            return sequence;
        }

        private void OnAppear()
        {
            raycastBlock.SetActive(false);
            
            if (_level.TimeCoins.Count > 0 && !_types.Contains(CellType.TimeCoin))
            {
                _types.Add(CellType.TimeCoin);
                _cellTutorialBoard.ShowWithParam(CellType.TimeCoin);
            }
            
            if (_level.LetterCoins.Count > 0 && !_types.Contains(CellType.LetterCoin))
            {
                _types.Add(CellType.LetterCoin);
                _cellTutorialBoard.ShowWithParam(CellType.LetterCoin);
            }

            _themeController.ShowTutorial();
        }

        public void SetLevel(Level level)
        {
            _filledCellCount = 0;
            _needToFill = level.Size * (level.Size - 1);
            
            _level = level;
            
            _solver = _container.Instantiate<Solver>();
            
            BuildGrid(level.Size);
            SetWord(level.Word);
            _progressBoard.SetUp(level);
            _themeController.SetTheme(level.Theme);
        }

        public void BuildGrid(int size)
        {
            gridLayoutGroup.enabled = false;
            
            for (var i = 0; i < blocks.Count; i++)
            {
                blocks[i].transform.SetSiblingIndex(i);
            }
            
            gridLayoutGroup.enabled = true;
            
            var s = rectTransform.rect.width;

            gridLayoutGroup.constraintCount = size;
            
            var max = Mathf.Max(size, size);
            var sum = (max + (max - 1) * spacingRatio);
            var cellSize = s / sum;
            var spacing = cellSize * spacingRatio;
            
            gridLayoutGroup.cellSize = Vector2.one * cellSize;
            gridLayoutGroup.spacing = Vector2.one * spacing;
            
            _size = size;
            _grid = new LetterBlock[size, size];

            for (var i = 0; i < blocks.Count; i++)
            {
                if (i < size * size)
                {
                    blocks[i].gameObject.SetActive(true);
                    
                    var x = i % size;
                    var y = i / size;

                    _grid[x, y] = blocks[i];
                    _grid[x, y].SetUp(x, y, cellSize);
                    
                }
                else
                    blocks[i].gameObject.SetActive(false);
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

            var sequence = Appear();
            sequence.Play();
        }

        public void SetWord(string word)
        {
            if (word.Length != _size)
            {
                Debug.LogError($"Word length {word.Length} is wrong");
                return;
            }

            var y = Center;

            for (var i = 0; i < _size; i++)
            {
                _grid[i, y].SetLetter(word[i]);
            }
            
            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
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

            foreach (var timeCoin in _level.TimeCoins)
            {
                var cell = timeCoin.Position;
                _grid[cell.x, cell.y].SetTimer(timeCoin.Duration);
            }
            
            foreach (var letterCoin in _level.LetterCoins)
            {
                var pos = letterCoin.Position;
                _grid[pos.x, pos.y].SetSpecialLetter(letterCoin.Letter);
            }
        }

        public void SelectBlock(LetterBlock block)
        {
            if (Path != null)
            {
                ClearPath();
                _wordBoard.Hide();
            }
            
            if (_selectedBlock)
            {
                _selectedBlock.State = BlockState.EmptyAvailable;
            }
            
            CanPath = false;
            _letterKeyboard.Show();
            _selectedBlock = block;
        }

        public bool AreNear(LetterBlock a, LetterBlock b)
        {
            return a.Y == b.Y && (a.X == b.X - 1 || a.X == b.X + 1)
                || a.X == b.X && (a.Y == b.Y - 1 || a.Y == b.Y + 1);
        }

        public bool TryDeselect()
        {
            if (_selectedBlock && !CanPath)
            {
                _selectedBlock.State = BlockState.EmptyAvailable;
                DeselectBlock();
                return true;
            }
            return false;
        }

        public void DeselectBlock()
        {
            _letterKeyboard.Hide();
            _selectedBlock = null;
            CanPath = false;
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
            if (ReplacePlace is not null)
            {
                var p = ReplacePlace.Value;
                _grid[p.x, p.y].SetLetter(letter);
                _letterKeyboard.Hide();
                _replaceButton.SwitchOff();
                StopReplace();
                SwitchOffBonuses();
            }
            else
            {
                _selectedBlock.SetLetter(letter);
                _selectedBlock.State = BlockState.FilledNew;
                Path.Clear();
                CanPath = true;
                _letterKeyboard.Hide();
                _wordBoard.Show();
            }
        }

        public bool TryAddToPath(LetterBlock block)
        {
            if (Path.Count == 0)
            {
                _pathChanged = false;
                Drag = true;
            }
            
            if (Path.Count != 0 && (!AreNear(Path.Last(), block) || Path.Contains(block)))
            {
                return false;
            }
            
            if (Path.Count > 0)
            {
                _pathChanged = true;
            }
            
            _wordMiniBoard.AddLetter(block.Letter);
            _trieWordChecker.AddLetter(block.Letter);
            Path.Add(block);
            _wordBoard.AddLetter(block.Letter);
            gridLayoutGroup.enabled = false;
            block.transform.SetAsFirstSibling();
            
            if (HintedPos != null)
            {
                var p = HintedPos.Value;
                _grid[p.x, p.y].transform.SetAsLastSibling();
            }
            return true;

        }

        private void Update()
        {
            if (Drag && Input.GetMouseButtonUp(0))
            {
                Drag = false;
                
                if (_pathChanged)
                {
                    CompletePath();
                    ClearPath();
                }
            }
        }
        
        public bool TryRemoveFromPath(LetterBlock block)
        {
            if (Path.Count == 0)
            {
                return false;
            }

            if (block != Path.Last())
            {
                return false;
            }
            
            _trieWordChecker.RemoveLetter();
            _wordMiniBoard.RemoveLetter();
            _wordBoard.RemoveLetter();
            
            Path.RemoveAt(Path.Count - 1);

            if (Path.Count == 0)
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
            _wordMiniBoard.Clear();
            
            foreach (var b in Path)
            {
                b.DePath();
            }
            
            Path.Clear();
        }

        public void OnClearWord()
        {
            ClearPath();
            _wordBoard.Hide();
            _selectedBlock.State = BlockState.EmptyAvailable;
            DeselectBlock();
        }

        public bool TryGetAntiDirection(out Vector2Int antiDirection)
        {
            if (Path.Count < 2)
            {
                antiDirection = Vector2Int.zero;
                return false;
            }
            
            antiDirection = new Vector2Int(Path[^2].X - Path[^1].X, Path[^2].Y - Path[^1].Y);
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
            if (x <  _size - 1)
            {
                coordinates.Add(Vector2Int.right);
            }
            if (y > 0)
            {
                coordinates.Add(Vector2Int.down);
            }
            if (y < _size - 1)
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
            if (x < _size - 1)
            {
                coordinates.Add(new Vector2Int(x + 1, y));
            }
            if (y > 0)
            {
                coordinates.Add(new Vector2Int(x, y - 1));
            }
            if (y < _size - 1)
            {
                coordinates.Add(new Vector2Int(x, y + 1));
            }
            
            return coordinates;
        }

        private List<LetterBlock> GetNeighbours(LetterBlock block)
        {
            return GetNeighbours(new Vector2Int(block.X, block.Y)).Select(v => _grid[v.x, v.y]).ToList();
        }

        private bool PathContainsNewLetter()
        {
            return Path.Contains(_selectedBlock);
        }

        public void CompletePath()
        {
            var word = string.Join("", Path.Select(b => b.Letter.ToString()));

            if (!PathContainsNewLetter())
            {
                _errorBoard.PopUp(_errorTypeWordDoesNotContainNewLetter, 
                    word.ToUpperInvariant(), 
                    _selectedBlock.Letter.ToString().ToUpperInvariant());
                return;
            }

            if (!_trieWordChecker.IsWordCorrect())
            {
                _errorBoard.PopUp(_errorTypeWordDoesNotExist, word.ToUpperInvariant());
                return;
            }

            if (!_progressBoard.CanMake(word))
            {
                _errorBoard.PopUp(_errorTypeWordIsAlreadyCollected, word.ToUpperInvariant());
                return;
            }
            
            var letterIndex = Path.IndexOf(_selectedBlock);

            var score = _themeController.GetScore(word);
            
            _wordMiniBoard.Send();
            
            _progressBoard.MakeWord(word, score, letterIndex);
            
            _wordHinter.StopHint();
            
            ClearPath();
            _wordBoard.Hide();
            _selectedBlock.State = BlockState.Filled;
            _selectedBlock.Complete();

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

            _filledCellCount++;
            if (_filledCellCount >= _needToFill)
            {
                _gameController.GotToNextLevel();
            }
        }

        private void SwitchOffBonuses()
        {
            _letterButton.SwitchOff();
            _cellButton.SwitchOff();
            _wordButton.SwitchOff();
            
            StopHintLetter();
            StopHintCell();
            StopHintWord();
        }
        
        public void StartHintLetter()
        {
            _replaceButton.SetInteractable(false);
            _eraseButton.SetInteractable(false);
            _flagButton.SetInteractable(false);
            
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
            if (!HintedLetter.HasValue)
            {
                return;
            }
            
            HintedLetter = null;
            
            foreach (var block in _grid)
            {
                if (block.State == BlockState.EmptyAvailable)
                {
                    block.StopHintLetter();
                }
            }
            
            _replaceButton.SetInteractable(true);
            _eraseButton.SetInteractable(true);
            _flagButton.SetInteractable(true);
        }
        
        public void StartHintCell()
        {
            _replaceButton.SetInteractable(false);
            _eraseButton.SetInteractable(false);
            _flagButton.SetInteractable(false);
            
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

        public void StopHintCell()
        {
            if (!HintedPos.HasValue)
            {
                return;
            }
            
            var p = HintedPos.Value;

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
            
            _replaceButton.SetInteractable(true);
            _eraseButton.SetInteractable(true);
            _flagButton.SetInteractable(true);
        }

        public void StartHintWord()
        {
            _replaceButton.SetInteractable(false);
            _eraseButton.SetInteractable(false);
            _flagButton.SetInteractable(false);
            
            _wordHinter.StartHint(Answer.Word);
        }
        
        public void StopHintWord()
        {
            _wordHinter.StopHint();
            
            _replaceButton.SetInteractable(true);
            _eraseButton.SetInteractable(true);
            _flagButton.SetInteractable(true);
        }

        public void StartReplace()
        {
            gridLayoutGroup.enabled = false;
            
            _eraseButton.SetInteractable(false);
            _flagButton.SetInteractable(false);
            _letterButton.SetInteractable(false);
            _cellButton.SetInteractable(false);
            _wordButton.SetInteractable(false);
            
            foreach (var block in _grid)
            {
                if (block.State != BlockState.Filled)
                {
                    block.Hide();
                }
                else
                {
                    block.StartHint();
                }
            }
            
            Time.timeScale = 0f;

            Replace = true;
            ReplacePlace = null;
        }

        public void StopReplace()
        {
            foreach (var block in _grid)
            {
                block.Show();
                block.StopHint();
            }

            Time.timeScale = 1f;

            Replace = false;
            ReplacePlace = null;
            
            _eraseButton.SetInteractable(true);
            _flagButton.SetInteractable(true);
            _letterButton.SetInteractable(true);
            _cellButton.SetInteractable(true);
            _wordButton.SetInteractable(true);
        }

        public void ChooseReplace(LetterBlock block)
        {
            foreach (var b in _grid)
            {
                b.Hide();
                b.StopHint();
            }
            block.Show();
            block.StartHint();
            
            block.SetLetter('?');
            ReplacePlace = block.Position;
            _letterKeyboard.Show();
        }
        
        public void StartErase()
        {
            gridLayoutGroup.enabled = false;
            
            _replaceButton.SetInteractable(false);
            _flagButton.SetInteractable(false);
            _letterButton.SetInteractable(false);
            _cellButton.SetInteractable(false);
            _wordButton.SetInteractable(false);
            
            foreach (var block in _grid)
            {
                if (block.State != BlockState.Filled)
                {
                    block.Hide();
                }
                else
                {
                    block.StartHint();
                }
            }
            
            Time.timeScale = 0f;

            Erase = true;
        }

        public void StopErase()
        {
            foreach (var block in _grid)
            {
                block.Show();
                block.StopHint();
            }

            Time.timeScale = 1f;

            Erase = false;
            
            _replaceButton.SetInteractable(true);
            _flagButton.SetInteractable(true);
            _letterButton.SetInteractable(true);
            _cellButton.SetInteractable(true);
            _wordButton.SetInteractable(true);
        }

        public void ChooseErase(LetterBlock block)
        {
            block.State = BlockState.EmptyAvailable;
            _filledCellCount--;

            StopErase();
            
            _eraseButton.SwitchOff();
            
            SwitchOffBonuses();
        }
        
        public void StartFlag()
        {
            gridLayoutGroup.enabled = false;
            
            _replaceButton.SetInteractable(false);
            _eraseButton.SetInteractable(false);
            _letterButton.SetInteractable(false);
            _cellButton.SetInteractable(false);
            _wordButton.SetInteractable(false);
            
            foreach (var block in _grid)
            {
                if (block.State is BlockState.EmptyNotAvailable or BlockState.EmptyAvailable)
                {
                    block.StartHint();
                }
                else
                {
                    block.Hide();
                }
            }
            
            Time.timeScale = 0f;

            Flag = true;
        }

        public void StopFlag()
        {
            foreach (var block in _grid)
            {
                block.Show();
                block.StopHint();
            }

            Time.timeScale = 1f;

            Flag = false;
            
            _replaceButton.SetInteractable(true);
            _eraseButton.SetInteractable(true);
            _letterButton.SetInteractable(true);
            _cellButton.SetInteractable(true);
            _wordButton.SetInteractable(true);
        }

        public void ChooseFlag(LetterBlock block)
        {
            block.State = BlockState.Flag;

            StopFlag();
            
            _flagButton.SwitchOff();
            
            SwitchOffBonuses();
            
            _filledCellCount++;
            
            if (_filledCellCount >= _needToFill)
            {
                _gameController.GotToNextLevel();
            }
        }
    }
}
