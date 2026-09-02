using Grids;
using Levels;
using YG;
using UnityEngine;
using Zenject;

namespace Yandex
{
    public class DataController : MonoBehaviour
    {
        [Inject] private LanguageController _languageController;
        
        private void Save()
        {
            YG2.SaveProgress();
        }

        private Data GetData()
        {
            return YG2.saves.LocalizedData.GetData(_languageController.GetLang());
        }

        public int GetMoney()
        {
            var data = GetData();
            return data.Money;
        }
        
        public int GetLevelIndex()
        {
            var data = GetData();
            return data.LevelIndex;
        }
        
        public Level GetLevel()
        {
            var data = GetData();
            return data.Level;
        }
        
        public LevelProgress GetLevelProgress()
        {
            var data = GetData();
            return data.LevelProgress;
        }

        public void SaveMoney(int money)
        {
            var data = GetData();
            data.Money = money;
            Save();
        }
        
        public void SaveLevelIndex(int levelIndex)
        {
            var data = GetData();
            data.LevelIndex = levelIndex;
            Save();
        }
        
        public void SaveLevel(Level level)
        {
            var data = GetData();
            data.Level = level;
            Save();
        }
        
        public void SaveLevelProgress(LevelProgress levelProgress)
        {
            var data = GetData();
            data.LevelProgress = levelProgress;
            Save();
        }
    }
}