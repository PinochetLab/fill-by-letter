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
using Random = UnityEngine.Random;

namespace Grids
{
    public class LetterBlock : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text text;

        [SerializeField] private Image leftLink;
        [SerializeField] private Image rightLink;
        [SerializeField] private Image topLink;
        [SerializeField] private Image bottomLink;
        
        [SerializeField] private Image backlight;
        
        [SerializeField] private GameObject selection;
        
        [SerializeField] private float hintDeltaScale = 0.1f;
        [SerializeField] private float scaleDuration = 0.5f;
        
        [SerializeField] private GameObject timerGameObject;
        [SerializeField] private Image timerFilledImage;
        [SerializeField] private RectTransform filledCoin;

        [SerializeField] private CanvasGroup canvasGroup;

        [SerializeField] private GameObject specialLetter;
        [SerializeField] private TMP_Text specialLetterText;
        
        [SerializeField] private GameObject flag;

        [SerializeField] private BlockStateColorPalette bodyColorPalette;
        [SerializeField] private BlockStateColorPalette textColorPalette;
        
        private Dictionary<Vector2Int, Image> _links;

        private Image _activeLink;
        
        private Tweener _scaleTweener;
        private Sequence _timeCoinSequence;
        private Sequence _timeCoinDisappearSequence;
        private Sequence _specialLetterDisappearSequence;

        private BlockState _state;

        [Inject] private GridController _gridController;
        [Inject] private RewardSpawner _rewardSpawner;

        //private const float ChangeColorTime = 0.1f;
        
        public char Letter { get; private set; }

        private char? SpecialLetter { get; set; }

        public int X { get; private set; }
        public int Y { get; private set; }
        
        private char? HintedLetter { get; set; }
        
        public Vector2Int Position => new(X, Y);

        private void Awake()
        {
            selection.SetActive(false);
            flag.SetActive(false);
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

                if (value == BlockState.Flag)
                {
                    text.gameObject.SetActive(false);
                    flag.SetActive(true);

                    if (_timeCoinSequence is not null && _timeCoinSequence.IsPlaying())
                    {
                        EndTimer();
                    }

                    if (SpecialLetter is not null)
                    {
                        EndSpecialLetter();
                    } 
                }
                
                backlight.gameObject.SetActive(value == BlockState.FilledNewPath);

                var bodyColor = bodyColorPalette.GetColor(value);
                
                if (value == BlockState.Filled)
                {
                    var v = Random.insideUnitSphere * 0.1f;
                    var r = Mathf.Clamp01(bodyColor.r + v.x);
                    var g = Mathf.Clamp01(bodyColor.g + v.y);
                    var b = Mathf.Clamp01(bodyColor.b + v.z);
                    bodyColor = new Color(r, g, b);
                }

                const float changeColorTime = 0.2f;
                
                _links.Values.ToList().ForEach(l => l.DOColor(bodyColor, changeColorTime));
                //_links.Values.ToList().ForEach(l => l.color = bodyColor);
                
                image.DOColor(bodyColor, changeColorTime);
                //image.color = bodyColor;

                var textColor = textColorPalette.GetColor(value);
                
                text.DOColor(textColor, changeColorTime);
                //text.color = textColor;

                _state = value;
            }
        }

        public void SetUp(int x, int y)
        {
            X = x;
            Y = y;

            _links = new Dictionary<Vector2Int, Image>()
            {
                { new Vector2Int(-1, 0), leftLink },
                { new Vector2Int(1, 0), rightLink },
                { new Vector2Int(0, 1), bottomLink },
                { new Vector2Int(0, -1), topLink },
            };
            
            specialLetter.SetActive(false);
        }

        public void SetSpecialLetter(char letter)
        {
            SpecialLetter = letter;
            specialLetter.SetActive(true);
            specialLetterText.text = letter.ToString();
        }

        public void SetTimer()
        {
            var duration = 30f * Mathf.Abs(Y - _gridController.Center);
            
            timerGameObject.SetActive(true);

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
                .Append(timerGameObject.transform.DOScale(1.3f, 0.25f).SetEase(Ease.OutBack))
                .Append(timerGameObject.transform.DOScale(0f, 0.35f).SetEase(Ease.InCubic))
                .OnComplete(() => timerGameObject.SetActive(false))
                .Play();
        }
        
        private void EndSpecialLetter()
        {
            _specialLetterDisappearSequence.Kill();
            
            _specialLetterDisappearSequence = DOTween.Sequence()
                .Append(specialLetter.transform.DOScale(1.3f, 0.25f).SetEase(Ease.OutBack))
                .Append(specialLetter.transform.DOScale(0f, 0.35f).SetEase(Ease.InCubic))
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

        public void PopUp()
        {
            transform.DOKill();
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.2f);
        }

        public void StopHint()
        {
            selection.SetActive(false);
            _scaleTweener.Kill();
        }

        public void Hide()
        {
            canvasGroup.enabled = true;
        }
        
        public void Show()
        {
            canvasGroup.enabled = false;
        }

        public void Complete()
        {
            if (_timeCoinSequence is not null && _timeCoinSequence.IsPlaying())
            {
                _timeCoinSequence?.Kill();
                _rewardSpawner.SpawnReward(filledCoin.position, 10);
                timerGameObject.gameObject.SetActive(false);
            }

            if (SpecialLetter is not null)
            {
                if (Letter == SpecialLetter)
                {
                    _rewardSpawner.SpawnReward(filledCoin.position, 10);
                    specialLetter.SetActive(false);
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

        public void DePath()
        {
            switch (State)
            {
                case BlockState.FilledPath when _gridController.CanPath:
                    State = BlockState.Filled;
                    if (_activeLink)
                    {
                        _activeLink.gameObject.SetActive(false);
                        _activeLink = null;
                    }
                    break;
                case BlockState.FilledNewPath when _gridController.CanPath:
                    State = BlockState.FilledNew;
                    if (_activeLink)
                    {
                        _activeLink.gameObject.SetActive(false);
                        _activeLink = null;
                    }
                    break;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
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
                case BlockState.EmptyAvailable:
                    _gridController.SelectBlock(this);
                    State = BlockState.EmptySelected;
                    break;
                case BlockState.EmptySelected:
                    _gridController.DeselectBlock();
                    State = BlockState.EmptyAvailable;
                    break;
                case BlockState.Filled when _gridController.CanPath:
                    if (_gridController.TryAddToPath(this))
                    {
                        State = BlockState.FilledPath;
                        
                        if (_gridController.TryGetAntiDirection(out var antiDirection))
                        {
                            _activeLink = _links[antiDirection];
                            _activeLink.gameObject.SetActive(true);
                        }
                    }
                    break;
                case BlockState.FilledNew when _gridController.CanPath:
                    if (_gridController.TryAddToPath(this))
                    {
                        State = BlockState.FilledNewPath;
                        
                        if (_gridController.TryGetAntiDirection(out var antiDirection))
                        {
                            _activeLink = _links[antiDirection];
                            _activeLink.gameObject.SetActive(true);
                        }
                    }
                    break;
                case BlockState.FilledPath when _gridController.CanPath:
                    if (_gridController.TryRemoveFromPath(this))
                    {
                        State = BlockState.Filled;
                        if (_activeLink)
                        {
                            _activeLink.gameObject.SetActive(false);
                            _activeLink = null;
                        }
                    }
                    break;
                case BlockState.FilledNewPath when _gridController.CanPath:
                    if (_gridController.TryRemoveFromPath(this))
                    {
                        State = BlockState.FilledNew;
                        if (_activeLink)
                        {
                            _activeLink.gameObject.SetActive(false);
                            _activeLink = null;
                        }
                    }
                    break;
            }
        }
    }
}