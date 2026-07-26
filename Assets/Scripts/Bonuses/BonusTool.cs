using UnityEngine;

namespace Bonuses
{
    [CreateAssetMenu(fileName = "Bonus Tool", menuName = "Bonus Tool", order = 0)]
    public class BonusTool : ScriptableObject
    {
        [SerializeField] private string toolName;
        [SerializeField] private Sprite sprite;
        [TextArea(3, 5)]
        [SerializeField] private string description;
        
        public string ToolName => toolName;
        public Sprite Sprite => sprite;
        public string Description => description;
    }
}