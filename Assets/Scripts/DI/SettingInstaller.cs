using Errors;
using UnityEngine;
using Zenject;

namespace DI
{
    [CreateAssetMenu(fileName = "SettingInstaller", menuName = "Installers/SettingInstaller")]
    public class SettingInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private ErrorType wordIsAlreadyCollected;
        [SerializeField] private ErrorType wordDoesNotExist;
        [SerializeField] private ErrorType wordDoesNotContainNewLetter;
        [SerializeField] private ErrorType tapOnEmptyCellToPutLetter;
        
        [SerializeField] private ErrorRankColorPalette errorRankColorPalette;

        public override void InstallBindings()
        {
            /*Container.BindInstance(wordIsAlreadyCollected)
                .WithId(ErrorTypeNameMaster.WordIsAlreadyCollected)
                .AsSingle();

            Container.BindInstance(wordDoesNotExist)
                .WithId(ErrorTypeNameMaster.WordDoesNotExist)
                .AsSingle();

            Container.BindInstance(wordDoesNotContainNewLetter)
                .WithId(ErrorTypeNameMaster.WordDoesNotContainNewLetter)
                .AsSingle();

            Container.BindInstance(tapOnEmptyCellToPutLetter)
                .WithId(ErrorTypeNameMaster.TapOnEmptyCellToPutLetter)
                .AsSingle();*/
            
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
            
            Container.Bind<ErrorRankColorPalette>().FromInstance(errorRankColorPalette).AsSingle();
        }
    }
}