using System.Collections.Generic;
using Money;
using UnityEngine;

namespace Levels
{
    [CreateAssetMenu(fileName = "Level", menuName = "Level", order = 0)]
    public class Level : ScriptableObject
    {
        [SerializeField] private int size;
        [SerializeField] private string word;
        
        [SerializeField] private int firstScoreGoal;
        [SerializeField] private int secondScoreGoal;

        [SerializeField] private List<Vector2Int> timeCoins;
        [SerializeField] private List<LetterCoinInfo> letterCoins;
        
        public int Size => size;
        
        public string Word => word;
        
        public int FirstScoreGoal => firstScoreGoal;
        
        public int SecondScoreGoal => secondScoreGoal;
        
        public List<Vector2Int> TimeCoins => timeCoins;
        
        public List<LetterCoinInfo> LetterCoins => letterCoins;
    }
}