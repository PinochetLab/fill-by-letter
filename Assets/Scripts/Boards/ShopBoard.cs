using UnityEngine;
using UnityEngine.UI;

namespace Boards
{
    public class ShopBoard : AbstractBoard
    {
        [Header("Shop Options")]
        [SerializeField] private ScrollRect scrollRect;
        
        protected override void ProcessParam(object _)
        {
            scrollRect.normalizedPosition = new Vector2(0, 1);
        }
    }
}