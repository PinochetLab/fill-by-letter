using UnityEngine;

namespace Grids
{
    [CreateAssetMenu(fileName = "Cell Type Info", menuName = "Cell Type Info", order = 0)]
    public class CellTypeInfo : ScriptableObject
    {
        [SerializeField] private CellType cellType;
        [SerializeField] private string title;
        [TextArea(3, 5)]
        [SerializeField] private string description;
        
        public CellType CellType => cellType;
        public string Title => title;
        public string Description => description;
    }
}