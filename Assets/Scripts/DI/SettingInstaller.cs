using Errors;
using Grids;
using UnityEngine;
using Zenject;

namespace DI
{
    [CreateAssetMenu(fileName = "SettingInstaller", menuName = "Installers/SettingInstaller")]
    public class SettingInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private ErrorRankColorPalette errorRankColorPalette;
        
        [Header("Errors")]
        [SerializeField] private ErrorType wordIsAlreadyCollected;
        [SerializeField] private ErrorType wordDoesNotExist;
        [SerializeField] private ErrorType wordDoesNotContainNewLetter;
        [SerializeField] private ErrorType tapOnEmptyCellToPutLetter;

        [Header("Cells")]
        [SerializeField] private CellTypeInfo timeCoin;
        [SerializeField] private CellTypeInfo letterCoin;
        
        public override void InstallBindings()
        {
            Container.Bind<ErrorRankColorPalette>().FromInstance(errorRankColorPalette).AsSingle();

            InstallErrorBindings();

            InstallCellTypeInfoBindings();
        }

        private void InstallErrorBindings()
        {
            Container.Bind<ErrorType>()
                .WithId(ErrorTypeNameMaster.WordIsAlreadyCollected)
                .FromInstance(wordIsAlreadyCollected);
            
            Container.Bind<ErrorType>()
                .WithId(ErrorTypeNameMaster.WordDoesNotExist)
                .FromInstance(wordDoesNotExist);
            
            Container.Bind<ErrorType>()
                .WithId(ErrorTypeNameMaster.WordDoesNotContainNewLetter)
                .FromInstance(wordDoesNotContainNewLetter);
            
            Container.Bind<ErrorType>()
                .WithId(ErrorTypeNameMaster.TapOnEmptyCellToPutLetter)
                .FromInstance(tapOnEmptyCellToPutLetter);
        }

        private void InstallCellTypeInfoBindings()
        {
            Container.Bind<CellTypeInfo>().WithId(CellType.TimeCoin).FromInstance(timeCoin);
            Container.Bind<CellTypeInfo>().WithId(CellType.LetterCoin).FromInstance(letterCoin);
        }
    }
}