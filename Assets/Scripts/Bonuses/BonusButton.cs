using System;
using Grids;
using TMPro;
using Boards;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Bonuses
{
    public class BonusButton : MonoBehaviour
    {
        [Header("Graphics")]
        [SerializeField] private Button button;
        [SerializeField] private Shadow shadow;
        [SerializeField] private Outline selection;
        [SerializeField] private GameObject priceBoard;
        [SerializeField] private RectTransform priceBoardRt;
        [SerializeField] private TMP_Text actionText;
        [SerializeField] private Image toolImage;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private Image mainImage;
        [SerializeField] private Image priceImage;
        
        [Header("Tools")]
        [SerializeField] private BonusTool bonusTool;
        
        [Inject] private GridController _gridController;
        
        [Inject] private BonusTutorialBoard _bonusTutorialBoard;

        private bool _isOn;
        private bool _wasUsed;

        private void Start()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(priceBoardRt);
        }

        [ContextMenu("Update Tool")]
        private void UpdateTool()
        {
            if (!bonusTool)
            {
                actionText.text = string.Empty;
                toolImage.gameObject.SetActive(false);
                priceText.text = "0";
                mainImage.color = Color.gray;
                priceImage.color = Color.gray;
            }
            else
            {
                actionText.text = bonusTool.ActionName;
                toolImage.gameObject.SetActive(true);
                toolImage.sprite = bonusTool.SmallSprite;
                priceText.text = bonusTool.Price.ToString();

                var a = bonusTool.Color;
                var b = bonusTool.Color;

                a.a = 0.5f;
                b *= 0.7f;
                
                mainImage.color = a;
                priceImage.color = b;
            }
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.EditorUtility.SetDirty(actionText);
            UnityEditor.EditorUtility.SetDirty(priceText);
            UnityEditor.EditorUtility.SetDirty(mainImage);
            UnityEditor.EditorUtility.SetDirty(priceImage);
            UnityEditor.EditorUtility.SetDirty(toolImage);
#endif
        }

        private void Awake()
        {
            UpdateGraphics();
        }

        public void SetInteractable(bool interactable)
        {
            button.interactable = interactable;
        }

        private void UpdateGraphics()
        {
            priceBoard.SetActive(!_isOn);
            shadow.enabled = !_isOn;
            selection.enabled = _isOn;
        }

        private void UpdateState()
        {
            UpdateGraphics();
            
            if (_isOn)
            {
                OnOn();
            }
            else
            {
                OnOff();
            }
        }

        private void OnOn()
        {
            switch (bonusTool.Type)
            {
                case BonusType.Letter:
                    _gridController.StartHintLetter();
                    break;
                case BonusType.Cell:
                    _gridController.StartHintCell();
                    break;
                case BonusType.Word:
                    _gridController.StartHintWord();
                    break;
                case BonusType.Replace:
                    _gridController.StartReplace();
                    break;
                case BonusType.Erase:
                    _gridController.StartErase();
                    break;
                case BonusType.Flag:
                    _gridController.StartFlag();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnOff()
        {
            switch (bonusTool.Type)
            {
                case BonusType.Letter:
                    _gridController.StopHintLetter();
                    break;
                case BonusType.Cell:
                    _gridController.StopHintCell();
                    break;
                case BonusType.Word:
                    _gridController.StopHintWord();
                    break;
                case BonusType.Replace:
                    _gridController.StopReplace();
                    break;
                case BonusType.Erase:
                    _gridController.StopErase();
                    break;
                case BonusType.Flag:
                    _gridController.StopFlag();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public void SwitchOff()
        {
            _isOn = false;
            UpdateGraphics();
        }
        
        public void OnClick()
        {
            if (!_wasUsed)
            {
                _wasUsed = true;
                _bonusTutorialBoard.ShowWithParam(bonusTool);
            }
            else
            {
                _isOn = !_isOn;
                UpdateState();
            }
        }
    }
}