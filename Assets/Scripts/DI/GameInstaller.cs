using Bonuses;
using Errors;
using Grids;
using Keyboards;
using Money;
using Progress;
using Themes;
using UnityEngine;
using WordBoards;
using Words;
using Zenject;

namespace DI
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GridController gridController;
        [SerializeField] private LetterKeyboard letterKeyboard;
        [SerializeField] private WordBoard wordBoard;
        [SerializeField] private ProgressBoard progressBoard;
        [SerializeField] private TrieGlossaryLoader trieGlossaryLoader;
        [SerializeField] private TrieWordChecker trieWordChecker;
        [SerializeField] private HintLetterButton hintLetterButton;
        [SerializeField] private HintLetterPlaceButton hintLetterPlaceButton;
        [SerializeField] private HintWordButton hintWordButton;
        [SerializeField] private WordHinter wordHinter;
        [SerializeField] private ErrorBoard errorBoard;
        [SerializeField] private MoneyBoard moneyBoard;
        [SerializeField] private CoinTosser coinTosser;
        [SerializeField] private RewardSpawner rewardSpawner;
        [SerializeField] private ReplaceButton replaceButton;
        [SerializeField] private EraseButton eraseButton;
        [SerializeField] private FlagButton flagButton;
        [SerializeField] private BonusTutorialBoard bonusTutorialBoard;
        [SerializeField] private ThemeController themeController;
        [SerializeField] private ThemeTutorialBoard themeTutorialBoard;
        [SerializeField] private TreasureBoard treasureBoard;
        [SerializeField] private WordMiniBoard wordMiniBoard;

        public override void InstallBindings()
        {
            Container.Bind<GridController>().FromInstance(gridController).AsSingle();
            Container.Bind<LetterKeyboard>().FromInstance(letterKeyboard).AsSingle();
            Container.Bind<WordBoard>().FromInstance(wordBoard).AsSingle();
            Container.Bind<ProgressBoard>().FromInstance(progressBoard).AsSingle();
            Container.Bind<TrieGlossaryLoader>().FromInstance(trieGlossaryLoader).AsSingle();
            Container.Bind<TrieWordChecker>().FromInstance(trieWordChecker).AsSingle();
            Container.Bind<HintLetterButton>().FromInstance(hintLetterButton).AsSingle();
            Container.Bind<HintLetterPlaceButton>().FromInstance(hintLetterPlaceButton).AsSingle();
            Container.Bind<HintWordButton>().FromInstance(hintWordButton).AsSingle();
            Container.Bind<WordHinter>().FromInstance(wordHinter).AsSingle();
            Container.Bind<ErrorBoard>().FromInstance(errorBoard).AsSingle();
            Container.Bind<MoneyBoard>().FromInstance(moneyBoard).AsSingle();
            Container.Bind<CoinTosser>().FromInstance(coinTosser).AsSingle();
            Container.Bind<RewardSpawner>().FromInstance(rewardSpawner).AsSingle();
            Container.Bind<ReplaceButton>().FromInstance(replaceButton).AsSingle();
            Container.Bind<EraseButton>().FromInstance(eraseButton).AsSingle();
            Container.Bind<FlagButton>().FromInstance(flagButton).AsSingle();
            Container.Bind<BonusTutorialBoard>().FromInstance(bonusTutorialBoard).AsSingle();
            Container.Bind<ThemeController>().FromInstance(themeController).AsSingle();
            Container.Bind<ThemeTutorialBoard>().FromInstance(themeTutorialBoard).AsSingle();
            Container.Bind<TreasureBoard>().FromInstance(treasureBoard).AsSingle();
            Container.Bind<WordMiniBoard>().FromInstance(wordMiniBoard).AsSingle();
        }
    }
}