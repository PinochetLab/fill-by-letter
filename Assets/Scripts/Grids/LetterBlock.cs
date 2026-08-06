using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using Money;
using Tutorials;
using Random = UnityEngine.Random;

namespace Grids
{
    public class LetterBlock : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerEnterHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text text;
        [SerializeField] private RectTransform rectTransform;

        [SerializeField] private List<Image> slicedImages;

        [SerializeField] private Image leftLink;
        [SerializeField] private Image rightLink;
        [SerializeField] private Image topLink;
        [SerializeField] private Image bottomLink;
        
        [SerializeField] private Image backlight;
        [SerializeField] private Image backlight2;
        
        [SerializeField] private GameObject selection;
        
        [SerializeField] private float hintDeltaScale = 0.1f;
        [SerializeField] private float scaleDuration = 0.5f;
        
        [SerializeField] private RectTransform timerRt;
        [SerializeField] private Image timerFilledImage;
        [SerializeField] private RectTransform filledCoin;
        [SerializeField] private RectMask2D rectMask;

        [SerializeField] private GameObject hideBlock;

        [SerializeField] private RectTransform specialLetter;
        [SerializeField] private TMP_Text specialLetterText;
        
        [SerializeField] private GameObject flag;

        [SerializeField] private BlockStateColorPalette bodyColorPalette;
        [SerializeField] private BlockStateColorPalette textColorPalette;
        
        private Dictionary<Vector2Int, Image> _links;
        private Dictionary<Vector2Int, Transform> _arrows;
        
        private Tweener _scaleTweener;
        private Sequence _timeCoinSequence;
        private Sequence _timeCoinDisappearSequence;
        private Sequence _specialLetterDisappearSequence;
        private bool _interactable = true;

        private BlockState _state;

        [Inject] private GridController _gridController;
        [Inject] private RewardSpawner _rewardSpawner;
        [Inject] private TutorialBoard _tutorialBoard;

        //private const float ChangeColorTime = 0.1f;
        
        public char Letter { get; private set; }

        private char? SpecialLetter { get; set; }

        public int X { get; private set; }
        public int Y { get; private set; }

        private bool _isDown;
        private Vector2 _lastMousePos;
        private float _mousePathDistance;
        
        private char? HintedLetter { get; set; }
        
        public Vector2Int Position => new(X, Y);

        public Vector2 GetScreenPosition()
        {
            var positions = new Vector3[4];
            rectTransform.GetWorldCorners(positions);
            return (positions[0] + positions[2]) / 2f;;
        }

        public BlockState State
        {
            get => _state;
            set
            {
                text.gameObject.SetActive(value != BlockState.EmptyNotAvailable);
                
                if (value == BlockState.EmptyAvailable)
                {
                    SetLetter(HintedLetter is not null ? HintedLetter.Value : '?');
                }

                if (value == BlockState.EmptySelected)
                {
                    SetLetter('?');
                }
                
                flag.SetActive(value == BlockState.Flag);
                
                backlight.gameObject.SetActive(value == BlockState.FilledNewPath);
                backlight2.gameObject.SetActive(value == BlockState.FilledNew);

                var bodyColor = bodyColorPalette.GetColor(value);
                
                /*if (value == BlockState.Filled)
                {
                    var v = Random.insideUnitSphere * 0.1f;
                    var r = Mathf.Clamp01(bodyColor.r + v.x);
                    var g = Mathf.Clamp01(bodyColor.g + v.y);
                    var b = Mathf.Clamp01(bodyColor.b + v.z);
                    bodyColor = new Color(r, g, b);
                }*/

                const float changeColorTime = 0.2f;
                
                //_links.Values.ToList().ForEach(l => l.DOColor(bodyColor, changeColorTime));
                _links.Values.ToList().ForEach(l => l.color = bodyColor);
                
                //image.DOColor(bodyColor, changeColorTime);
                image.color = bodyColor;

                var textColor = textColorPalette.GetColor(value);
                
                //text.DOColor(textColor, changeColorTime);
                text.color = textColor;

                _state = value;
            }
        }

        public void SetUp(int x, int y, float size)
        {
            X = x;
            Y = y;

            const float bonusSize = 2.5f;

            timerRt.sizeDelta = Vector2.one * (size / bonusSize);
            specialLetter.sizeDelta = Vector2.one * (size / bonusSize);

            timerRt.rotation = Quaternion.identity;
            specialLetter.rotation = Quaternion.identity;

            rectMask.softness = Vector2Int.one * (int)size;

            var pixelsPerUnitMultiplier = 0.5f * (400 / size);
            
            slicedImages.ForEach(i => i.pixelsPerUnitMultiplier = pixelsPerUnitMultiplier);

            if (_links is null)
            {
                _links = new Dictionary<Vector2Int, Image>()
                {
                    { Vector2Int.left, leftLink },
                    { Vector2Int.right, rightLink },
                    { Vector2Int.up, bottomLink },
                    { Vector2Int.down, topLink },
                };

                var arrows = new List<Image> { leftLink, rightLink, bottomLink, topLink }
                    .Select(i => i.transform.GetChild(0)).ToList();
            
                _arrows = new Dictionary<Vector2Int, Transform>()
                {
                    { Vector2Int.left, arrows[0] },
                    { Vector2Int.right, arrows[1] },
                    { Vector2Int.up, arrows[2] },
                    { Vector2Int.down, arrows[3] },
                };
            }

            foreach (var arrow in _arrows.Values)
            {
                arrow.localScale = Vector3.one * (size / 200f);
            }
            
            specialLetter.gameObject.SetActive(false);
        }

        public void SetSpecialLetter(char letter)
        {
            SpecialLetter = letter;
            specialLetter.gameObject.SetActive(true);
            specialLetterText.text = letter.ToString();
        }

        public void SetTimer(float duration)
        {
            timerRt.gameObject.SetActive(true);

            timerFilledImage.fillAmount = 1;
            
            _timeCoinSequence.Kill();
            _timeCoinDisappearSequence.Kill();
            
            _timeCoinSequence = DOTween.Sequence()
                .Append(timerFilledImage.DOFillAmount(0, duration))
                .OnComplete(EndTimer)
                .Play();
        }

        private void EndTimer()
        {
            _timeCoinSequence.Kill();
            _timeCoinDisappearSequence.Kill();
            
            _timeCoinDisappearSequence = DOTween.Sequence()
                //.Append(timerRt.DOScale(1.3f, 0.25f).SetEase(Ease.OutBack))
                //.Append(timerRt.DORotate(Vector3.up * 90, 0.35f).SetEase(Ease.OutBack))
                .Append(timerRt.DOScale(0f, 0.35f).SetEase(Ease.InCubic))
                .OnComplete(() => timerRt.gameObject.SetActive(false))
                .Play();
        }
        
        private void EndSpecialLetter()
        {
            _specialLetterDisappearSequence.Kill();
            
            _specialLetterDisappearSequence = DOTween.Sequence()
                //.Append(specialLetter.DOScale(1.3f, 0.25f).SetEase(Ease.OutBack))
                //.Append(specialLetter.DORotate(Vector3.up * 90, 0.35f).SetEase(Ease.OutBack))
                .Append(specialLetter.DOScale(0f, 0.35f).SetEase(Ease.InCubic))
                .OnComplete(() => specialLetter.gameObject.SetActive(false))
                .Play();
        }

        public void SetLetter(char letter)
        {
            Letter = letter;
            text.text = letter.ToString();
        }

        public void StartHint()
        {
            transform.SetAsLastSibling();
            selection.SetActive(true);
            
            /*_scaleTweener = selectionCircle
                .DORotate(new Vector3(0, 0, 360), 5f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .SetUpdate(true);*/
            
            _scaleTweener.Kill();

            selection.transform.localScale = Vector3.one;
            
            _scaleTweener = selection.transform.DOScale(1 - hintDeltaScale, scaleDuration / 2)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }

        public Sequence Appear(float duration)
        {
            transform.localScale = Vector3.zero;

            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(1, duration));
            //sequence.SetUpdate(true);

            return sequence;
            
        }
        
        public Sequence Disappear(float duration)
        {
            transform.localScale = Vector3.one;

            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(0, duration));

            return sequence;
        }

        public void StopHint()
        {
            selection.SetActive(false);
            _scaleTweener.Kill();
        }

        public void Hide()
        {
            _interactable = false;
            hideBlock.SetActive(true);
        }
        
        public void Show()
        {
            _interactable = true;
            hideBlock.SetActive(false);
        }

        public void Complete(bool flag = false)
        {
            if (_timeCoinSequence.IsActive())
            {
                if (!flag)
                {
                    _timeCoinSequence?.Kill();
                    _rewardSpawner.SpawnReward(filledCoin.position, timerRt.rect.size, 10);
                    timerRt.gameObject.SetActive(false);
                }
                else
                {
                    EndTimer();
                }
            }

            if (SpecialLetter is not null)
            {
                if (!flag && Letter == SpecialLetter)
                {
                    _rewardSpawner.SpawnReward(filledCoin.position, timerRt.rect.size, 10);
                    specialLetter.gameObject.SetActive(false);
                }
                else
                {
                    EndSpecialLetter();
                }
            }
        }

        public void StartHintLetter(char letter)
        {
            HintedLetter = letter;
            SetLetter(letter);
        }
        
        public void StopHintLetter()
        {
            HintedLetter = null;
            SetLetter('?');
        }

        public void SetType(CellType type)
        {
            timerRt.gameObject.SetActive(false);

            text.text = string.Empty;
            
            switch (type)
            {
                case CellType.TimeCoin:
                    timerRt.gameObject.SetActive(true);
                    //State = BlockState.EmptyAvailable;
                    timerFilledImage.fillAmount = 0.75f;
                    break;
                case CellType.LetterCoin:
                    specialLetter.gameObject.SetActive(true);
                    //State = BlockState.EmptyAvailable;
                    specialLetterText.text = "В";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            
            SetLetter('?');
        }

        public void DePath()
        {
            _links.Values.ToList().ForEach(l => l.gameObject.SetActive(false));
            switch (State)
            {
                case BlockState.FilledPath when _gridController.CanPath:
                    State = BlockState.Filled;
                    break;
                case BlockState.FilledNewPath when _gridController.CanPath:
                    State = BlockState.FilledNew;
                    break;
            }
        }

        public void AddDirection(Vector2Int direction, bool isLast)
        {
            var link = _links[direction];
            link.gameObject.SetActive(true);
            var arrow = _arrows[direction];
            arrow.gameObject.SetActive(isLast);
        }
        
        public void RemoveDirection(Vector2Int direction)
        {
            var link = _links[direction];
            link.gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_tutorialBoard.IsActive && !_tutorialBoard.OneLine)
            {
                return;
            }
            
            if (!_interactable || !_gridController.CanPath || _gridController.Path.Count > 0)
            {
                return;
            }
            
            switch (State)
            {
                case BlockState.Filled:
                    if (_gridController.TryAddToPath(this))
                    {
                        State = BlockState.FilledPath;

                        _isDown = true;
                        _lastMousePos = Input.mousePosition;
                        _mousePathDistance = 0;
                        _gridController.Drag = true;
                    }
                    break;
                case BlockState.FilledNew:
                    if (_gridController.TryAddToPath(this))
                    {
                        State = BlockState.FilledNewPath;
                        
                        _isDown = true;
                        _lastMousePos = Input.mousePosition;
                        _mousePathDistance = 0;
                        _gridController.Drag = true;
                    }
                    break;
            }
        }

        private void Update()
        {
            if (!_isDown)
            {
                return;
            }
            
            if (!Input.GetMouseButtonUp(0))
            {
                var currentPos = Input.mousePosition;
                _mousePathDistance += Vector2.Distance(_lastMousePos, currentPos);
                _lastMousePos =currentPos;
            }
            else
            {
                _isDown = false;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_interactable || _gridController.TryDeselect())
            {
                return;
            }
            
            if (_tutorialBoard.IsActive && State == BlockState.EmptyAvailable)
            {
                _gridController.SelectBlock(this);
                State = BlockState.EmptySelected;
                return;
            }
            
            if (_tutorialBoard.IsActive && _tutorialBoard.OneLine)
            {
                return;
            }
            
            if (_tutorialBoard.IsActive && !_tutorialBoard.OneLine)
            {
                switch (State)
                {
                    case BlockState.Filled when _gridController.CanPath:
                        if (_gridController.TryAddToPath(this))
                        {
                            State = BlockState.FilledPath;
                        }
                        break;
                    case BlockState.FilledNew when _gridController.CanPath:
                        if (_gridController.TryAddToPath(this))
                        {
                            State = BlockState.FilledNewPath;
                        }
                        break;
                }
                return;
            }
            
            if (_gridController.Flag)
            {
                switch (State)
                {
                    case BlockState.EmptyNotAvailable:
                    case BlockState.EmptyAvailable:
                        _gridController.ChooseFlag(this);
                        break;
                }

                return;
            }
            
            if (_gridController.Erase)
            {
                switch (State)
                {
                    case BlockState.Filled:
                        _gridController.ChooseErase(this);
                        break;
                }

                return;
            }
            
            if (_gridController.Replace && _gridController.ReplacePlace is null)
            {
                switch (State)
                {
                    case BlockState.Filled:
                        _gridController.ChooseReplace(this);
                        break;
                }

                return;
            }
            
            switch (State)
            {
                case BlockState.EmptyAvailable when !_gridController.CanPath:
                    _gridController.SelectBlock(this);
                    State = BlockState.EmptySelected;
                    break;
                case BlockState.Filled when _gridController.CanPath && _gridController.Path.Count > 0:
                    if (_gridController.TryAddToPath(this))
                    {
                        State = BlockState.FilledPath;
                    }
                    break;
                case BlockState.FilledNew when _gridController.CanPath && _gridController.Path.Count > 0:
                    if (_gridController.TryAddToPath(this))
                    {
                        State = BlockState.FilledNewPath;
                    }
                    break;
                case BlockState.FilledPath when _gridController.CanPath && (!_isDown || _mousePathDistance > 10):
                    if (_gridController.TryRemoveFromPath(this))
                    {
                        State = BlockState.Filled;
                    }
                    break;
                case BlockState.FilledNewPath when _gridController.CanPath && (!_isDown || _mousePathDistance > 10):
                    if (_gridController.TryRemoveFromPath(this))
                    {
                        State = BlockState.FilledNew;
                    }
                    break;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_interactable || !_gridController.Drag || _gridController.Path.Count == 0)
            {
                return;
            }
            
            if (_tutorialBoard.IsActive && !_tutorialBoard.OneLine)
            {
                return;
            }

            switch (State)
            {
                case BlockState.Filled:
                    if (_gridController.TryAddToPath(this))
                    {
                        State = BlockState.FilledPath;
                    }
                    break;
                case BlockState.FilledNew:
                    if (_gridController.TryAddToPath(this))
                    {
                        State = BlockState.FilledNewPath;
                    }
                    break;
                case BlockState.FilledPath:
                case BlockState.FilledNewPath:
                    if (_gridController.Path.Count > 1 && this == _gridController.Path[^2])
                    {
                        var last = _gridController.Path.Last();
                        if (_gridController.TryRemoveFromPath(last))
                        {
                            last.DePath();
                        }
                    }
                    break;
            }
        }
    }
}