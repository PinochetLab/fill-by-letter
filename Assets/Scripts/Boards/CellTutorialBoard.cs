using System;
using Grids;
using TMPro;
using UnityEngine;
using Zenject;

namespace Boards
{
    public class CellTutorialBoard : AbstractBoard
    {
        [Header("Cell Settings")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private LetterBlock letterBlock;
        [SerializeField] private TMP_Text descriptionText;
        
        [Inject(Id = CellType.TimeCoin)] private CellTypeInfo _timeCoinInfo;
        [Inject(Id = CellType.LetterCoin)] private CellTypeInfo _letterCoinInfo;

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