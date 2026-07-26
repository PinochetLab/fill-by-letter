using Grids;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Bonuses
{
    public abstract class AbstractBonusButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Shadow shadow;
        [SerializeField] private Outline selection;
        [SerializeField] private GameObject priceBoard;
        [SerializeField] private BonusTool bonusTool;
        [SerializeField] private Image mainImage;
        
        [Inject] protected GridController GridController;
        
        [Inject] private BonusTutorialBoard _bonusTutorialBoard;

        private bool _isOn;
        
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

        protected abstract void OnOn();
        protected abstract void OnOff();
        
        public void SwitchOff()
        {
            _isOn = false;
            UpdateGraphics();
        }

        public void Do()
        {
            _isOn = !_isOn;
            UpdateState();
        }
        
        public void OnClick()
        {
            _bonusTutorialBoard.OpenIfNeeded(this, bonusTool, mainImage.color);
        }
    }
}