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
        
        public override void InstallBindings()
        {
            Container.Bind<ErrorRankColorPalette>().FromInstance(errorRankColorPalette).AsSingle();
        }
    }
}