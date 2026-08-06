using System.Collections.Generic;
using Money;
using Themes;
using UnityEngine;
using UnityEngine.Localization;

namespace Levels
{
    [CreateAssetMenu(fileName = "Level", menuName = "Level", order = 0)]
    public class Level : ScriptableObject
    {
        [SerializeField] private int size;
        [SerializeField] private string word;
        
        [SerializeField] private int firstScoreGoal;
        [SerializeField] private int secondScoreGoal;

        [SerializeField] private List<TimeCoin> timeCoins;
        [SerializeField] private List<LetterCoin> letterCoins;
        
        [SerializeField] private LocalizedAsset<Theme> theme;
        
        public int Size => size;
        
        public string Word => word;
        
        public int FirstScoreGoal => firstScoreGoal;
        
        public int SecondScoreGoal => secondScoreGoal;
        
        public List<TimeCoin> TimeCoins => timeCoins;
        
        public List<LetterCoin> LetterCoins => letterCoins;
        
        public LocalizedAsset<Theme> Theme => theme;
    }
}