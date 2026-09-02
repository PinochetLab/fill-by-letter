using Bonuses;
using Errors;
using Grids;
using Keyboards;
using Money;
using Progress;
using Themes;
using Boards;
using Game;
using Tutorials;
using UnityEngine;
using WordBoards;
using Words;
using Yandex;
using Zenject;

namespace DI
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameController gameController;
        [SerializeField] private GridController gridController;
        [SerializeField] private LetterKeyboard letterKeyboard;
        [SerializeField] private WordBoard wordBoard;
        [SerializeField] private ProgressBoard progressBoard;
        [SerializeField] private TrieGlossaryLoader trieGlossaryLoader;
        [SerializeField] private TrieWordChecker trieWordChecker;
        [SerializeField] private WordHinter wordHinter;
        [SerializeField] private ErrorBoard errorBoard;
        [SerializeField] private MoneyBoard moneyBoard;
        [SerializeField] private CoinTosser coinTosser;
        [SerializeField] private RewardSpawner rewardSpawner;
        [SerializeField] private BonusTutorialBoard bonusTutorialBoard;
        [SerializeField] private ThemeController themeController;
        [SerializeField] private ThemeTutorialBoard themeTutorialBoard;
        [SerializeField] private TreasureBoard treasureBoard;
        [SerializeField] private WordMiniBoard wordMiniBoard;
        [SerializeField] private CellTutorialBoard cellTutorialBoard;
        [SerializeField] private ShopBoard shopBoard;
        [SerializeField] private TutorialBoard tutorialBoard;
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private MoneyController moneyController;
        [SerializeField] private AdsController adsController;
        [SerializeField] private LanguageController languageController;
        [SerializeField] private DataController dataController;
        
        [Header("Bonus Buttons")]
        [SerializeField] private BonusButton letterButton;
        [SerializeField] private BonusButton cellButton;
        [SerializeField] private BonusButton wordButton;
        [SerializeField] private BonusButton replaceButton;
        [SerializeField] private BonusButton eraseButton;
        [SerializeField] private BonusButton flagButton;

        public override void InstallBindings()
        {
            Container.Bind<GameController>().FromInstance(gameController).AsSingle();
            Container.Bind<GridController>().FromInstance(gridController).AsSingle();
            Container.Bind<LetterKeyboard>().FromInstance(letterKeyboard).AsSingle();
            Container.Bind<WordBoard>().FromInstance(wordBoard).AsSingle();
            Container.Bind<ProgressBoard>().FromInstance(progressBoard).AsSingle();
            Container.Bind<TrieGlossaryLoader>().FromInstance(trieGlossaryLoader).AsSingle();
            Container.Bind<TrieWordChecker>().FromInstance(trieWordChecker).AsSingle();
            Container.Bind<WordHinter>().FromInstance(wordHinter).AsSingle();
            Container.Bind<ErrorBoard>().FromInstance(errorBoard).AsSingle();
            Container.Bind<MoneyBoard>().FromInstance(moneyBoard).AsSingle();
            Container.Bind<CoinTosser>().FromInstance(coinTosser).AsSingle();
            Container.Bind<RewardSpawner>().FromInstance(rewardSpawner).AsSingle();
            Container.Bind<BonusTutorialBoard>().FromInstance(bonusTutorialBoard).AsSingle();
            Container.Bind<ThemeController>().FromInstance(themeController).AsSingle();
            Container.Bind<ThemeTutorialBoard>().FromInstance(themeTutorialBoard).AsSingle();
            Container.Bind<TreasureBoard>().FromInstance(treasureBoard).AsSingle();
            Container.Bind<WordMiniBoard>().FromInstance(wordMiniBoard).AsSingle();
            Container.Bind<CellTutorialBoard>().FromInstance(cellTutorialBoard).AsSingle();
            Container.Bind<ShopBoard>().FromInstance(shopBoard).AsSingle();
            Container.Bind<TutorialBoard>().FromInstance(tutorialBoard).AsSingle();
            Container.Bind<BoardManager>().FromInstance(boardManager).AsSingle();
            Container.Bind<MoneyController>().FromInstance(moneyController).AsSingle();
            Container.Bind<AdsController>().FromInstance(adsController).AsSingle();
            Container.Bind<LanguageController>().FromInstance(languageController).AsSingle();
            Container.Bind<DataController>().FromInstance(dataController).AsSingle();

            Container.Bind<BonusButton>().WithId(BonusType.Letter).FromInstance(letterButton);
            Container.Bind<BonusButton>().WithId(BonusType.Cell).FromInstance(cellButton);
            Container.Bind<BonusButton>().WithId(BonusType.Word).FromInstance(wordButton);
            Container.Bind<BonusButton>().WithId(BonusType.Replace).FromInstance(replaceButton);
            Container.Bind<BonusButton>().WithId(BonusType.Erase).FromInstance(eraseButton);
            Container.Bind<BonusButton>().WithId(BonusType.Flag).FromInstance(flagButton);
        }
    }
}