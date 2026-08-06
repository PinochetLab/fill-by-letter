using System;
using Grids;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

namespace Boards
{
    public class CellTutorialBoard : AbstractBoard
    {
        [Header("Cell Settings")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private LetterBlock letterBlock;
        [SerializeField] private TMP_Text descriptionText;
        
        [SerializeField] private LocalizedAsset<CellTypeInfo> timeCoinInfo;
        [SerializeField] private LocalizedAsset<CellTypeInfo> letterCoinInfo;

        private CellTypeInfo _timeCoinInfo;
        private CellTypeInfo _letterCoinInfo;

        private void Awake()
        {
            _timeCoinInfo = timeCoinInfo.LoadAsset();
            _letterCoinInfo = letterCoinInfo.LoadAsset();
        }

        private CellTypeInfo GetInfo(CellType cellType)
        {
            return cellType switch
            {
                CellType.TimeCoin => _timeCoinInfo,
                CellType.LetterCoin => _letterCoinInfo,
                _ => throw new ArgumentOutOfRangeException(nameof(cellType), cellType, null)
            };
        }

        protected override void ProcessParam(object param)
        {
            if (param is not CellType cellType)
            {
                throw new NotImplementedException($"param={param} is not {nameof(CellType)}");
            }
            
            var info = GetInfo(cellType);
            
            titleText.text = info.Title;
            descriptionText.text = info.Description;
            
            letterBlock.SetType(cellType);
        }
    }
}