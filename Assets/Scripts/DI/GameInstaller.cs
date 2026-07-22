using Bonuses;
using Grids;
using Keyboards;
using Progress;
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
        [SerializeField] private BonusController bonusController;
        [SerializeField] private HintLetterButton hintLetterButton;
        [SerializeField] private HintLetterPlaceButton hintLetterPlaceButton;
        [SerializeField] private HintWordButton hintWordButton;
        [SerializeField] private WordHinter wordHinter;

        public override void InstallBindings()
        {
            Container.Bind<GridController>().FromInstance(gridController).AsSingle();
            Container.Bind<LetterKeyboard>().FromInstance(letterKeyboard).AsSingle();
            Container.Bind<WordBoard>().FromInstance(wordBoard).AsSingle();
            Container.Bind<ProgressBoard>().FromInstance(progressBoard).AsSingle();
            Container.Bind<TrieGlossaryLoader>().FromInstance(trieGlossaryLoader).AsSingle();
            Container.Bind<TrieWordChecker>().FromInstance(trieWordChecker).AsSingle();
            Container.Bind<BonusController>().FromInstance(bonusController).AsSingle();
            Container.Bind<HintLetterButton>().FromInstance(hintLetterButton).AsSingle();
            Container.Bind<HintLetterPlaceButton>().FromInstance(hintLetterPlaceButton).AsSingle();
            Container.Bind<HintWordButton>().FromInstance(hintWordButton).AsSingle();
            Container.Bind<WordHinter>().FromInstance(wordHinter).AsSingle();
        }
    }
}