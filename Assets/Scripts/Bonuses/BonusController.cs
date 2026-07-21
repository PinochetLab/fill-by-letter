using UnityEngine;

namespace Bonuses
{
    public class BonusController : MonoBehaviour
    {
        [SerializeField] private GameObject bonusesPanel;

        public void ShowBonuses()
        {
            bonusesPanel.SetActive(true);
        }

        public void HideBonuses()
        {
            bonusesPanel.SetActive(false);
        }
    }
}