using UnityEngine;

namespace Bonuses
{
    [CreateAssetMenu(fileName = "Bonus Tool", menuName = "Bonus Tool", order = 0)]
    public class BonusTool : ScriptableObject
    {
        [SerializeField] private string toolName;
        [SerializeField] private string actionName;
        [SerializeField] private Sprite bigSprite;
        [SerializeField] private Sprite smallSprite;
        [SerializeField] private int price;
        [SerializeField] private BonusType type;
        [SerializeField] private Color color;
        [TextArea(3, 5)]
        [SerializeField] private string description;
        
        public string ToolName => toolName;
        public string ActionName => actionName;
        public Sprite SmallSprite => smallSprite;
        public Sprite BigSprite => bigSprite;
        public int Price => price;
        public BonusType Type => type;
        public Color Color => color;
        public string Description => description;
    }
}