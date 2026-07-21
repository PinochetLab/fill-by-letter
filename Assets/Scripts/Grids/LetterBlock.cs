using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;

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
        
        [SerializeField] private Transform hint;
        [SerializeField] private float hintDeltaScale = 0.1f;
        [SerializeField] private float scaleDuration = 0.5f;

        [SerializeField] private BlockStateColorPalette bodyColorPalette;
        [SerializeField] private BlockStateColorPalette textColorPalette;
        
        private Dictionary<Vector2Int, Image> _links;

        private Image _activeLink;
        
        private Tweener _scaleTweener;

        private BlockState _state;

        [Inject] private GridController _gridController;

        //private const float ChangeColorTime = 0.1f;
        
        public char Letter { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }
        
        private char? HintedLetter { get; set; }
        
        public Vector2Int Position => new(X, Y);

        private void Awake()
        {
            hint.gameObject.SetActive(false);
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
                
                backlight.gameObject.SetActive(value == BlockState.FilledNewPath);

                var bodyColor = bodyColorPalette.GetColor(value);
                
                //_links.Values.ToList().ForEach(l => l.DOColor(bodyColor, ChangeColorTime));
                _links.Values.ToList().ForEach(l => l.color = bodyColor);
                
                //image.DOColor(bodyColor, ChangeColorTime);
                image.color = bodyColor;

                var textColor = textColorPalette.GetColor(value);
                
                //text.DOColor(textColor, ChangeColorTime);
                text.color = textColor;

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
        }

        public void SetLetter(char letter)
        {
            Letter = letter;
            text.text = letter.ToString();
        }

        public void StartHint()
        {
            transform.SetAsLastSibling();
            hint.gameObject.SetActive(true);
            _scaleTweener = hint.DOScale(Vector3.one * (1 + hintDeltaScale), scaleDuration / 2)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void StopHint()
        {
            hint.gameObject.SetActive(false);
            _scaleTweener.Kill();
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