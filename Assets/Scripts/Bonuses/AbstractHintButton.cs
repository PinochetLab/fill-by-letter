using Grids;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Bonuses
{
    public abstract class AbstractHintButton : MonoBehaviour
    {
        [SerializeField] private Shadow shadow;
        [SerializeField] private Outline selection;
        [SerializeField] private GameObject priceBoard;
        
        [Inject] protected GridController GridController;

        private bool _isOn;
        
        private void Awake()
        {
            UpdateGraphics();
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
            UpdateState();
        }
        
        public void OnClick()
        {
            _isOn = !_isOn;
            UpdateState();
        }
    }
}