using Game;
using Keyboards;
using YG;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Words;
using Zenject;

namespace Yandex
{
    public class LanguageController : MonoBehaviour
    {
        private string _lang;
        
        public void SetUp()
        {
            _lang = YG2.lang;
            YG2.GameReadyAPI();
            
            var locale = LocalizationSettings.AvailableLocales.GetLocale(_lang);
            LocalizationSettings.SelectedLocale = locale;
        }

        public string GetLang()
        {
            return _lang;
        }
    }
}