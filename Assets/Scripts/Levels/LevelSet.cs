using System.Collections.Generic;
using UnityEngine;

namespace Levels
{
    [CreateAssetMenu(fileName = "Level Set", menuName = "Level Set", order = 0)]
    public class LevelSet : ScriptableObject
    {
        [SerializeField] private List<Level> levels;

        public List<Level> Levels => levels;
    }
}